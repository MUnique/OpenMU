// <copyright file="ActorProtocolHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using System.Reflection;
using System.Text.Json;
using System.Threading;

/// <summary>
/// Speaks the newline-delimited JSON protocol of the control endpoint: one request object per line,
/// one response object per line, plus a streaming mode for <c>events --follow</c>.
/// </summary>
/// <remarks>
/// Deliberately free of sockets, so the whole protocol can be tested without opening a port.
/// </remarks>
public sealed class ActorProtocolHandler
{
    private readonly IActorRegistry _registry;
    private readonly BotsController _bots;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActorProtocolHandler"/> class.
    /// </summary>
    /// <param name="registry">The actor registry.</param>
    /// <param name="bots">The population bot switch.</param>
    public ActorProtocolHandler(IActorRegistry registry, BotsController bots)
    {
        this._registry = registry;
        this._bots = bots;
    }

    /// <summary>
    /// Gets the version this endpoint answers <c>ping</c> with.
    /// </summary>
    public static string Version { get; } =
        typeof(ActorProtocolHandler).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "0.0.0";

    /// <summary>
    /// Handles one request line.
    /// </summary>
    /// <param name="line">The received line.</param>
    /// <param name="writer">Where the response lines go.</param>
    /// <param name="cancellationToken">Cancelled when the connection goes away or the server stops.</param>
    /// <returns>The task.</returns>
    public async ValueTask HandleLineAsync(string line, Func<string, ValueTask> writer, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        string? id = null;
        try
        {
            using var document = JsonDocument.Parse(line);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                await WriteFailureAsync(writer, null, ActorErrorCodes.BadRequest, "A request must be a JSON object.").ConfigureAwait(false);
                return;
            }

            var request = document.RootElement;
            id = GetString(request, "id");
            var command = GetString(request, "cmd");
            if (string.IsNullOrEmpty(command))
            {
                await WriteFailureAsync(writer, id, ActorErrorCodes.BadRequest, "The request has no 'cmd'.").ConfigureAwait(false);
                return;
            }

            await this.DispatchAsync(command, request, id, writer, cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            await WriteFailureAsync(writer, id, ActorErrorCodes.BadRequest, $"The request is not valid JSON: {ex.Message}").ConfigureAwait(false);
        }
        catch (ArgumentException ex)
        {
            await WriteFailureAsync(writer, id, ActorErrorCodes.BadRequest, ex.Message).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await WriteFailureAsync(writer, id, ActorErrorCodes.Failed, ex.Message).ConfigureAwait(false);
        }
    }

    private static string? GetString(JsonElement request, string name)
        => request.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    /// <summary>
    /// Reads an optional integer field and checks that it lies within the allowed range.
    /// </summary>
    /// <remarks>
    /// A value outside the range is an <see cref="ArgumentException"/>, which the caller answers with
    /// <see cref="ActorErrorCodes.BadRequest"/> - never a wrapped cast: a typo like <c>"x":300</c>
    /// must be refused instead of walking to x=44.
    /// </remarks>
    private static long? GetNumber(JsonElement request, string name, long min, long max)
    {
        if (!request.TryGetProperty(name, out var value))
        {
            return null;
        }

        if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt64(out var number))
        {
            throw new ArgumentException($"'{name}' must be an integer.");
        }

        if (number < min || number > max)
        {
            throw new ArgumentException($"'{name}' must be between {min} and {max}, but is {number}.");
        }

        return number;
    }

    private static int? GetInt32(JsonElement request, string name, int min = 0, int max = int.MaxValue)
        => (int?)GetNumber(request, name, min, max);

    private static byte? GetByte(JsonElement request, string name)
        => (byte?)GetNumber(request, name, byte.MinValue, byte.MaxValue);

    private static ushort? GetUInt16(JsonElement request, string name)
        => (ushort?)GetNumber(request, name, ushort.MinValue, ushort.MaxValue);

    private static bool GetBool(JsonElement request, string name)
        => request.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.True;

    private static string? GetTarget(JsonElement request)
    {
        if (request.TryGetProperty("target", out var value))
        {
            return value.ValueKind switch
            {
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.TryGetInt64(out var number) ? number.ToString() : null,
                _ => null,
            };
        }

        return null;
    }

    private static ValueTask WriteFailureAsync(Func<string, ValueTask> writer, string? id, string code, string error)
        => writer(ActorJson.WriteResponse(id, ActorCommandResult.Failure(code, error)));

    private static ValueTask WriteResultAsync(Func<string, ValueTask> writer, string? id, ActorCommandResult result)
        => writer(ActorJson.WriteResponse(id, result));

    private async ValueTask DispatchAsync(string command, JsonElement request, string? id, Func<string, ValueTask> writer, CancellationToken cancellationToken)
    {
        switch (command)
        {
            case "ping":
                await WriteResultAsync(writer, id, ActorCommandResult.Success(
                    new ActorEventField("version", Version),
                    new ActorEventField("actors", (await this._registry.ListAsync().ConfigureAwait(false)).Count))).ConfigureAwait(false);
                return;

            case "spawn":
                var spawned = await this._registry.SpawnAsync(
                    GetInt32(request, "server") ?? 0,
                    GetString(request, "actor") ?? string.Empty,
                    GetByte(request, "slot")).ConfigureAwait(false);
                await WriteResultAsync(writer, id, spawned).ConfigureAwait(false);
                return;

            case "stop":
                await WriteResultAsync(writer, id, await this._registry.StopAsync(GetString(request, "actor") ?? string.Empty).ConfigureAwait(false)).ConfigureAwait(false);
                return;

            case "list":
                var actors = await this._registry.ListAsync().ConfigureAwait(false);
                await WriteResultAsync(writer, id, ActorCommandResult.Success(new ActorEventField(
                    "actors",
                    actors.Select(a => ActorState.Summary(a.AccountLoginName ?? string.Empty, a)).ToList()))).ConfigureAwait(false);
                return;

            case "bots":
                await WriteResultAsync(
                    writer,
                    id,
                    await this._bots.HandleAsync(GetString(request, "action") ?? "status", GetInt32(request, "count")).ConfigureAwait(false)).ConfigureAwait(false);
                return;

            default:
                await this.DispatchActorCommandAsync(command, request, id, writer, cancellationToken).ConfigureAwait(false);
                return;
        }
    }

    private async ValueTask DispatchActorCommandAsync(string command, JsonElement request, string? id, Func<string, ValueTask> writer, CancellationToken cancellationToken)
    {
        var loginName = GetString(request, "actor") ?? string.Empty;
        if (await this._registry.FindAsync(loginName).ConfigureAwait(false) is not { } actor)
        {
            await WriteFailureAsync(writer, id, ActorErrorCodes.UnknownActor, $"No actor animates '{loginName}'.").ConfigureAwait(false);
            return;
        }

        switch (command)
        {
            case "state":
                await WriteResultAsync(writer, id, ActorCommandResult.Success(
                    new ActorEventField("state", ActorState.Full(loginName, actor)))).ConfigureAwait(false);
                return;

            case "nearby":
                await WriteResultAsync(writer, id, ActorCommandResult.Success(
                    new ActorEventField("objects", ActorState.Nearby(actor)))).ConfigureAwait(false);
                return;

            case "halt":
                var interrupted = actor.Intelligence?.Halt() ?? false;
                await WriteResultAsync(writer, id, ActorCommandResult.Success(
                    new ActorEventField("interrupted", interrupted))).ConfigureAwait(false);
                return;

            case "events":
                await this.StreamEventsAsync(actor, request, id, writer, cancellationToken).ConfigureAwait(false);
                return;

            default:
                await this.ExecuteActorCommandAsync(command, request, actor, id, writer).ConfigureAwait(false);
                return;
        }
    }

    private async ValueTask ExecuteActorCommandAsync(string command, JsonElement request, ScriptedPlayer actor, string? id, Func<string, ValueTask> writer)
    {
        ActorCommand? actorCommand = command switch
        {
            "walk" => new WalkCommand(GetByte(request, "x") ?? 0, GetByte(request, "y") ?? 0),
            "attack" => new AttackCommand(
                GetTarget(request) ?? string.Empty,
                GetInt32(request, "times", min: 1) ?? 1,
                GetInt32(request, "interval") ?? 0),
            "skill" => new SkillCommand(GetUInt16(request, "skill") ?? 0, GetTarget(request) ?? string.Empty),
            "say" => new SayCommand(GetString(request, "text") ?? string.Empty),
            "pickup" => new PickupCommand(GetUInt16(request, "id") ?? 0),
            "warp" => new WarpCommand(GetInt32(request, "gate") ?? -1),
            _ => null,
        };

        if (actorCommand is null)
        {
            await WriteFailureAsync(writer, id, ActorErrorCodes.BadRequest, $"Unknown command '{command}'.").ConfigureAwait(false);
            return;
        }

        if (actor.Intelligence is not { } intelligence)
        {
            await WriteFailureAsync(writer, id, ActorErrorCodes.NotReady, "The actor is not (yet) able to act.").ConfigureAwait(false);
            return;
        }

        await WriteResultAsync(writer, id, await intelligence.ExecuteAsync(actorCommand).ConfigureAwait(false)).ConfigureAwait(false);
    }

    private async ValueTask StreamEventsAsync(ScriptedPlayer actor, JsonElement request, string? id, Func<string, ValueTask> writer, CancellationToken cancellationToken)
    {
        var since = GetNumber(request, "since", 0, long.MaxValue) ?? 0;
        if (!GetBool(request, "follow"))
        {
            await WriteResultAsync(writer, id, ActorCommandResult.Success(
                new ActorEventField("events", actor.EventLog.Since(since).Select(ActorJson.ToDictionary).ToList()),
                new ActorEventField("last_seq", actor.EventLog.LastSequence))).ConfigureAwait(false);
            return;
        }

        // Stream mode: this connection belongs to the follower until it goes away.
        using var subscription = actor.EventLog.Subscribe(since);
        await WriteResultAsync(writer, id, ActorCommandResult.Success(new ActorEventField("follow", true))).ConfigureAwait(false);
        await foreach (var actorEvent in subscription.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            await writer(ActorJson.WriteEvent(actorEvent)).ConfigureAwait(false);
        }
    }
}
