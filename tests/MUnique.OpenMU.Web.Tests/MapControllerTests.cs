// <copyright file="MapControllerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Web.Map;
using MUnique.OpenMU.Web.Map.Map;

/// <summary>
/// Tests for the <see cref="MapController"/> of the live map.
/// </summary>
[TestFixture]
public class MapControllerTests
{
    private const string MapIdentifier = "map_0_app0";

    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    private readonly Guid _mapId = Guid.NewGuid();

    /// <summary>
    /// Tests that a view plugin call of the game logic doesn't wait for the browser.
    /// </summary>
    /// <returns>The task of this test.</returns>
    [Test]
    public async Task ViewPlugInDoesNotWaitForTheBrowserAsync()
    {
        var answer = new TaskCompletionSource<IJSVoidResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var browser = new Mock<IJSRuntime>();
        browser.Setup(r => r.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<object?[]>()))
            .Returns(() => new ValueTask<IJSVoidResult>(answer.Task));
        var gameServer = new Mock<IObservableGameServer>();
        var controller = this.CreateController(browser.Object, gameServer.Object, new CircuitConnectionState());

        var call = controller.ViewPlugIns.GetPlugIn<IObjectsOutOfScopePlugIn>()!.ObjectsOutOfScopeAsync([CreateObject(1)]);

        Assert.That(call.IsCompleted, Is.True);
        answer.TrySetResult(null!);
        await controller.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that the calls are dropped while the browser is disconnected, and that the map is
    /// cleared and populated again when it's back.
    /// </summary>
    /// <returns>The task of this test.</returns>
    [Test]
    public async Task MapIsResynchronizedAfterReconnectAsync()
    {
        var browser = new RecordingBrowser();
        var gameServer = new Mock<IObservableGameServer>();
        var registered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        gameServer.Setup(s => s.RegisterMapObserverAsync(this._mapId, It.IsAny<ILocateable>()))
            .Returns(() =>
            {
                registered.TrySetResult();
                return ValueTask.CompletedTask;
            });
        var circuitState = new CircuitConnectionState();
        var controller = this.CreateController(browser.Object, gameServer.Object, circuitState);
        var outOfScopePlugIn = controller.ViewPlugIns.GetPlugIn<IObjectsOutOfScopePlugIn>()!;

        await circuitState.OnConnectionDownAsync(null!, CancellationToken.None).ConfigureAwait(false);
        await outOfScopePlugIn.ObjectsOutOfScopeAsync([CreateObject(1)]).ConfigureAwait(false);
        await circuitState.OnConnectionUpAsync(null!, CancellationToken.None).ConfigureAwait(false);

        Assert.That(await Task.WhenAny(registered.Task, Task.Delay(Timeout)).ConfigureAwait(false), Is.SameAs(registered.Task));
        gameServer.Verify(s => s.UnregisterMapObserverAsync(this._mapId, It.IsAny<ushort>()), Times.Once);
        Assert.That(SpinWait.SpinUntil(() => !browser.Identifiers.IsEmpty, Timeout), Is.True);
        Assert.That(browser.Identifiers, Is.EqualTo(new[] { $"{MapIdentifier}.world.clear" }), "the removal was dropped while the browser was away");

        await controller.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that disposing the controller twice unregisters the map observer only once.
    /// </summary>
    /// <returns>The task of this test.</returns>
    [Test]
    public async Task DisposeIsIdempotentAsync()
    {
        var browser = new RecordingBrowser();
        var gameServer = new Mock<IObservableGameServer>();
        var controller = this.CreateController(browser.Object, gameServer.Object, new CircuitConnectionState());

        await controller.DisposeAsync().ConfigureAwait(false);
        await controller.DisposeAsync().ConfigureAwait(false);

        gameServer.Verify(s => s.UnregisterMapObserverAsync(this._mapId, It.IsAny<ushort>()), Times.Once);
        Assert.That(browser.Identifiers.Count(identifier => identifier == "DisposeMap"), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the disposal succeeds when the browser is already gone.
    /// </summary>
    [Test]
    public void DisposeToleratesADisconnectedBrowser()
    {
        var browser = new Mock<IJSRuntime>();
        browser.Setup(r => r.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<object?[]>()))
            .Throws(new JSDisconnectedException("gone"));
        browser.Setup(r => r.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<object?[]>()))
            .Throws(new JSDisconnectedException("gone"));
        var controller = this.CreateController(browser.Object, new Mock<IObservableGameServer>().Object, new CircuitConnectionState());

        Assert.DoesNotThrowAsync(async () => await controller.DisposeAsync().ConfigureAwait(false));
    }

    private static IIdentifiable CreateObject(ushort id)
    {
        var obj = new Mock<IIdentifiable>();
        obj.Setup(o => o.Id).Returns(id);
        return obj.Object;
    }

    private MapController CreateController(IJSRuntime browser, IObservableGameServer gameServer, CircuitConnectionState circuitState)
    {
        return new MapController(browser, NullLoggerFactory.Instance, MapIdentifier, gameServer, this._mapId, circuitState);
    }

    /// <summary>
    /// A browser which records the identifiers of the called functions.
    /// </summary>
    private sealed class RecordingBrowser
    {
        private readonly Mock<IJSRuntime> _mock = new();

        public RecordingBrowser()
        {
            this._mock
                .Setup(r => r.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<object?[]>()))
                .Returns((string identifier, CancellationToken _, object?[] _) =>
                {
                    this.Identifiers.Enqueue(identifier);
                    return ValueTask.FromResult<IJSVoidResult>(null!);
                });
            this._mock
                .Setup(r => r.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<object?[]>()))
                .Returns((string identifier, object?[] _) =>
                {
                    this.Identifiers.Enqueue(identifier);
                    return ValueTask.FromResult<IJSVoidResult>(null!);
                });
        }

        public IJSRuntime Object => this._mock.Object;

        public ConcurrentQueue<string> Identifiers { get; } = new();
    }
}
