// <copyright file="ActorProtocolHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.TestActors;

using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using MUnique.OpenMU.GameLogic.TestActors;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

/// <summary>
/// Tests the line protocol without opening a socket.
/// </summary>
[TestFixture]
public class ActorProtocolHandlerTests
{
    /// <summary>
    /// A ping is answered with the build version.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task PingIsAnsweredAsync()
    {
        var (handler, lines) = CreateHandler();

        await handler.HandleLineAsync("""{"id":"1","cmd":"ping"}""", lines.WriteAsync, CancellationToken.None).ConfigureAwait(false);

        var response = lines.Single();
        Assert.That(response.GetProperty("ok").GetBoolean(), Is.True);
        Assert.That(response.GetProperty("id").GetString(), Is.EqualTo("1"));
        Assert.That(response.GetProperty("version").GetString(), Is.Not.Empty);
    }

    /// <summary>
    /// A line which is not a JSON object gets an error response - and the caller keeps the
    /// connection, because nothing throws.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task MalformedLineIsAnsweredWithAnErrorAsync()
    {
        var (handler, lines) = CreateHandler();

        await handler.HandleLineAsync("this is not json", lines.WriteAsync, CancellationToken.None).ConfigureAwait(false);
        await handler.HandleLineAsync("[1,2,3]", lines.WriteAsync, CancellationToken.None).ConfigureAwait(false);
        await handler.HandleLineAsync("""{"cmd":"ping"}""", lines.WriteAsync, CancellationToken.None).ConfigureAwait(false);

        Assert.That(lines.Count, Is.EqualTo(3));
        Assert.That(lines[0].GetProperty("ok").GetBoolean(), Is.False);
        Assert.That(lines[0].GetProperty("code").GetString(), Is.EqualTo(ActorErrorCodes.BadRequest));
        Assert.That(lines[1].GetProperty("code").GetString(), Is.EqualTo(ActorErrorCodes.BadRequest));
        Assert.That(lines[2].GetProperty("ok").GetBoolean(), Is.True, "the connection still works after a malformed line");
    }

    /// <summary>
    /// An unknown command is refused instead of being ignored.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task UnknownCommandIsRefusedAsync()
    {
        var (handler, lines) = CreateHandler();

        await handler.HandleLineAsync("""{"id":"7","cmd":"nonsense"}""", lines.WriteAsync, CancellationToken.None).ConfigureAwait(false);

        Assert.That(lines.Single().GetProperty("ok").GetBoolean(), Is.False);
        Assert.That(lines.Single().GetProperty("id").GetString(), Is.EqualTo("7"));
    }

    /// <summary>
    /// A command for an account which is not animated names the actor in the error.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task CommandForAnUnknownActorIsRefusedAsync()
    {
        var (handler, lines) = CreateHandler();

        await handler.HandleLineAsync("""{"cmd":"state","actor":"test9"}""", lines.WriteAsync, CancellationToken.None).ConfigureAwait(false);

        Assert.That(lines.Single().GetProperty("code").GetString(), Is.EqualTo(ActorErrorCodes.UnknownActor));
        Assert.That(lines.Single().GetProperty("error").GetString(), Does.Contain("test9"));
    }

    /// <summary>
    /// The registry is asked to spawn, and the failure it reports is passed through with its code.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task SpawnPassesTheRegistryResultThroughAsync()
    {
        var registry = new Mock<IActorRegistry>();
        registry.Setup(r => r.List()).Returns([]);
        registry.Setup(r => r.SpawnAsync(0, "test1", null))
            .Returns(ValueTask.FromResult(ActorCommandResult.Failure(ActorErrorCodes.InUse, "The account 'test1' is already in use by a bot.")));
        var lines = new ResponseCollector();
        var handler = new ActorProtocolHandler(registry.Object, CreateBotsController());

        await handler.HandleLineAsync("""{"cmd":"spawn","actor":"test1"}""", lines.WriteAsync, CancellationToken.None).ConfigureAwait(false);

        Assert.That(lines.Single().GetProperty("code").GetString(), Is.EqualTo(ActorErrorCodes.InUse));
        registry.Verify(r => r.SpawnAsync(0, "test1", null), Times.Once);
    }

    /// <summary>
    /// The actor's event history is returned as one array of flattened events.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task EventsReturnTheHistorySinceASequenceAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        actor.EventLog.Append("chat", new ActorEventField("message", "hello"));
        var registry = new Mock<IActorRegistry>();
        registry.Setup(r => r.Find("test1")).Returns(actor);
        var lines = new ResponseCollector();
        var handler = new ActorProtocolHandler(registry.Object, CreateBotsController());

        await handler.HandleLineAsync("""{"cmd":"events","actor":"test1","since":0}""", lines.WriteAsync, CancellationToken.None).ConfigureAwait(false);

        var events = lines.Single().GetProperty("events").EnumerateArray().ToList();
        Assert.That(events, Is.Not.Empty);
        Assert.That(events.Select(e => e.GetProperty("seq").GetInt64()), Is.Ordered.Ascending);
        Assert.That(events.Last().GetProperty("type").GetString(), Is.EqualTo("chat"));
        Assert.That(events.Last().GetProperty("message").GetString(), Is.EqualTo("hello"));
        Assert.That(events.Last().GetProperty("utc").GetString(), Is.Not.Empty);
    }

    private static (ActorProtocolHandler Handler, ResponseCollector Lines) CreateHandler()
    {
        var registry = new Mock<IActorRegistry>();
        registry.Setup(r => r.List()).Returns([]);
        registry.Setup(r => r.Find(It.IsAny<string>())).Returns((ScriptedPlayer?)null);
        return (new ActorProtocolHandler(registry.Object, CreateBotsController()), new ResponseCollector());
    }

    private static BotsController CreateBotsController()
    {
        var locator = new Mock<IGameServerContextLocator>();
        locator.Setup(l => l.GetContexts()).Returns([]);
        return new BotsController(locator.Object, new NullLogger<BotsController>());
    }

    private sealed class ResponseCollector : List<JsonElement>
    {
        public ValueTask WriteAsync(string line)
        {
            this.Add(JsonDocument.Parse(line).RootElement.Clone());
            return ValueTask.CompletedTask;
        }
    }
}
