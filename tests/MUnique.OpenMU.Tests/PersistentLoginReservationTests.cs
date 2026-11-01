// <copyright file="PersistentLoginReservationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using global::Dapr.Client;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.CentralServer.Host;

/// <summary>Verifies distributed reservations expire while ordinary login state remains durable.</summary>
[TestFixture]
public sealed class PersistentLoginReservationTests
{
    /// <summary>Only a reservation gets a TTL; release and every normal account write clear it.</summary>
    [Test]
    public async Task ReservationExpiryDoesNotExpireNormalLoginStateAsync()
    {
        int? current = -1;
        var etag = "version";
        var writes = new List<(int Value, string Ttl)>();
        var client = new Mock<DaprClient>();
        client.Setup(c => c.GetStateAndETagAsync<int?>("login-state", "account", ConsistencyMode.Strong, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => (current, etag));
        client.Setup(c => c.TrySaveStateAsync("login-state", "account", It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, int value, string _, StateOptions? _, IReadOnlyDictionary<string, string> metadata, CancellationToken _) =>
            {
                current = value;
                writes.Add((value, metadata["ttlInSeconds"]));
                return true;
            });
        client.Setup(c => c.TrySaveStateAsync("login-state", "account", It.IsAny<byte>(), It.IsAny<string>(), null, It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, byte value, string _, StateOptions? _, IReadOnlyDictionary<string, string> metadata, CancellationToken _) =>
            {
                current = value;
                writes.Add((value, metadata["ttlInSeconds"]));
                return true;
            });
        client.Setup(c => c.SaveStateAsync<int?>("login-state", "account", -1, It.IsAny<StateOptions>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .Returns((string _, string _, int? value, StateOptions _, IReadOnlyDictionary<string, string> metadata, CancellationToken _) =>
            {
                current = value;
                etag = "initialized";
                writes.Add((value!.Value, metadata["ttlInSeconds"]));
                return Task.CompletedTask;
            });
        client.Setup(c => c.TrySaveStateAsync<int?>("login-state", "account", -1, It.IsAny<string>(), null, It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, int? value, string _, StateOptions? _, IReadOnlyDictionary<string, string> metadata, CancellationToken _) =>
            {
                current = value;
                writes.Add((value!.Value, metadata["ttlInSeconds"]));
                return true;
            });
        var server = new PersistentLoginServer(NullLogger<PersistentLoginServer>.Instance, client.Object);
        Assert.That(await server.TryReserveAccountAsync("account", 1000).ConfigureAwait(false), Is.True);
        Assert.That(writes.Last(), Is.EqualTo((1000, "300")));
        await server.ReleaseAccountReservationAsync("account", 1001).ConfigureAwait(false);
        Assert.That(current, Is.EqualTo(1000));
        await server.ReleaseAccountReservationAsync("account", 1000).ConfigureAwait(false);
        Assert.That(writes.Last(), Is.EqualTo((-1, "-1")));
        Assert.That(await server.TryReserveAccountAsync("account", 1000).ConfigureAwait(false), Is.True);
        Assert.That(await server.TryLoginAsync("account", 1).ConfigureAwait(false), Is.False);
        // Simulate Dapr evicting the lease after the admin host died.
        current = null;
        etag = string.Empty;
        Assert.That(await server.TryLoginAsync("account", 1).ConfigureAwait(false), Is.True);
        Assert.That(writes.TakeLast(2).Select(w => w.Ttl), Is.All.EqualTo("-1"));
        await server.ReleaseAccountReservationAsync("account", 1000).ConfigureAwait(false);
        Assert.That(current, Is.EqualTo(1));
        Assert.That(await server.TryLoginAsync("account", 2).ConfigureAwait(false), Is.False);
        await server.LogOffAsync("account", 1).ConfigureAwait(false);
        Assert.That(writes.Last(), Is.EqualTo((-1, "-1")));
    }
}
