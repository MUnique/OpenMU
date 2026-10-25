// <copyright file="AdministrationDeliveryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Net.Http;
using System.Threading;
using global::Dapr.Client;
using Moq;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.GameServer.Host;
using MUnique.OpenMU.Interfaces;

/// <summary>Verifies confirmed Dapr routing and duplicate-delivery behavior for live writes.</summary>
[TestFixture]
public sealed class AdministrationDeliveryTests
{
    /// <summary>Concurrent redelivery returns the original result and executes a grant once.</summary>
    [Test]
    public async Task ConcurrentDeliveryRunsOnceAsync()
    {
        var deliveries = new AdministrationDeliveries();
        var request = new GameServerAdministrationRequest { ServerId = 4, CharacterName = "Hero", Item = new(0, 1) };
        var calls = 0;
        var finish = new TaskCompletionSource<GameServerAdminResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        ValueTask<GameServerAdminResult> ExecuteAsync()
        {
            Interlocked.Increment(ref calls);
            return new(finish.Task);
        }

        var first = deliveries.RunAsync("GiveItemAsync", request, ExecuteAsync).AsTask();
        var second = deliveries.RunAsync("GiveItemAsync", request, ExecuteAsync).AsTask();
        finish.SetResult(GameServerAdminResult.Success);
        Assert.That((await first.ConfigureAwait(false)).Succeeded, Is.True);
        Assert.That((await second.ConfigureAwait(false)).Succeeded, Is.True);
        Assert.That(calls, Is.EqualTo(1));
        request.Item = new(0, 2);
        Assert.ThrowsAsync<ArgumentException>(async () => await deliveries.RunAsync("GiveItemAsync", request, ExecuteAsync).ConfigureAwait(false));
    }

    /// <summary>A lost response cannot cause a retry of an execution which threw after mutating state.</summary>
    [Test]
    public async Task FailedDeliveryIsNotReexecutedAsync()
    {
        var deliveries = new AdministrationDeliveries();
        var request = new GameServerAdministrationRequest();
        var calls = 0;
        ValueTask<GameServerAdminResult> ExecuteAsync()
        {
            calls++;
            throw new InvalidOperationException("Lost response after a possible mutation.");
        }

        Assert.ThrowsAsync<InvalidOperationException>(async () => await deliveries.RunAsync("GiveItemAsync", request, ExecuteAsync).ConfigureAwait(false));
        Assert.ThrowsAsync<InvalidOperationException>(async () => await deliveries.RunAsync("GiveItemAsync", request, ExecuteAsync).ConfigureAwait(false));
        Assert.That(calls, Is.EqualTo(1));
    }

    /// <summary>The controller refuses a request addressed to a different server.</summary>
    [Test]
    public void ControllerRejectsWrongOwner()
    {
        var server = new Mock<IGameServer>();
        server.SetupGet(s => s.Id).Returns(4);
        var live = server.As<IGameServerAdministration>();
        var controller = new GameServerAdministrationController(server.Object, new AdministrationDeliveries());
        Assert.ThrowsAsync<ArgumentException>(async () => await controller.SetLevelAsync(new() { ServerId = 9, CharacterName = "Hero", Level = 100 }).ConfigureAwait(false));
        live.Verify(s => s.SetLevelAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>The client invokes the exact game-server app and returns the confirmed response.</summary>
    [Test]
    public async Task ClientInvokesUniqueAppIdAsync()
    {
        var dapr = new Mock<DaprClient>();
        var id = Guid.NewGuid();
        using var request = new HttpRequestMessage();
        dapr.Setup(c => c.CreateInvokeMethodRequest(HttpMethod.Post, "game-server-4", "administration/SetLevelAsync", It.IsAny<IReadOnlyCollection<KeyValuePair<string, string>>>(), It.IsAny<GameServerAdministrationRequest>()))
            .Returns(request);
        dapr.Setup(c => c.InvokeMethodAsync<GameServerAdminResult>(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GameServerAdminResult(false, "Character left."));
        var client = new GameServerClient(dapr.Object, new ServerStateData { Id = 4, Type = ServerType.GameServer });
        var result = await client.SetLevelAsync("Hero", id, 100, false, CancellationToken.None).ConfigureAwait(false);
        Assert.That(result.Succeeded, Is.False);
        dapr.Verify(c => c.CreateInvokeMethodRequest(HttpMethod.Post, "game-server-4", "administration/SetLevelAsync", It.IsAny<IReadOnlyCollection<KeyValuePair<string, string>>>(),
            It.Is<GameServerAdministrationRequest>(r => r.ServerId == 4 && r.CharacterName == "Hero" && r.CharacterId == id && r.Level == 100 && r.RequestId != Guid.Empty)), Times.Once);
        dapr.Verify(c => c.InvokeMethodAsync<GameServerAdminResult>(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
