// <copyright file="AdminChangeConfirmation.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Nito.AsyncEx;

/// <summary>Owns bounded, short-lived confirmations and serializes administrative configuration writes.</summary>
/// <remarks>Confirmations are local to this admin host. Restarts or routing to another replica require a new preview.</remarks>
public sealed class AdminChangeConfirmation
{
    private readonly ConcurrentDictionary<string, Confirmation> _pending = new(StringComparer.Ordinal);
    private readonly TimeProvider _time;

    /// <summary>Initializes a new instance of the <see cref="AdminChangeConfirmation"/> class.</summary>
    /// <param name="time">The clock, overridable for expiry tests.</param>
    public AdminChangeConfirmation(TimeProvider time) => this._time = time;

    /// <summary>Gets the configuration write gate. Acquire before loading state and hold through audit completion.</summary>
    internal AsyncLock Gate { get; } = new();

    /// <summary>Issues a single-use preview bound to the caller, operation, arguments and complete relevant state.</summary>
    /// <param name="actor">The authorized caller.</param>
    /// <param name="operation">The invariant operation name.</param>
    /// <param name="arguments">The exact validated operation arguments.</param>
    /// <param name="change">The safe proposed diff and state fingerprints.</param>
    internal JsonObject Preview(AdminActor actor, string operation, object arguments, JsonObject change)
    {
        lock (this._pending)
        {
            return this.CreatePreview(actor, operation, arguments, change);
        }
    }

    /// <summary>Consumes a confirmation atomically; mismatches, expiry and replays fail closed.</summary>
    /// <param name="actor">The authorized caller.</param>
    /// <param name="operation">The invariant operation name.</param>
    /// <param name="arguments">The exact validated operation arguments.</param>
    /// <param name="change">The safe proposed diff and state fingerprints.</param>
    /// <param name="token">The single-use confirmation token.</param>
    internal void Consume(AdminActor actor, string operation, object arguments, JsonObject change, string? token)
    {
        if (token is null || !this._pending.TryRemove(token, out var confirmation)
            || confirmation.ExpiresAt <= this._time.GetUtcNow()
            || confirmation.Digest != Binding(actor, operation, arguments, change))
        {
            throw new AdminOperationException("The confirmation is invalid, expired, already used, or the state changed. Request a new preview.");
        }
    }

    private static string Binding(AdminActor actor, string operation, object arguments, JsonObject change)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { actor.Id, actor.Frontend, operation, arguments, change }))));

    private JsonObject CreatePreview(AdminActor actor, string operation, object arguments, JsonObject change)
    {
        foreach (var entry in this._pending.Where(p => p.Value.ExpiresAt <= this._time.GetUtcNow()))
        {
            this._pending.TryRemove(entry.Key, out _);
        }

        if (this._pending.Count >= 1024 || this._pending.Count(p => p.Value.ActorId == actor.Id && p.Value.Frontend == actor.Frontend) >= 64)
        {
            throw new AdminOperationException("Too many outstanding previews. Wait for them to expire.", AdminOperationErrorKind.Unavailable);
        }

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var expires = this._time.GetUtcNow().AddMinutes(5);
        this._pending[token] = new Confirmation(Binding(actor, operation, arguments, change), expires, actor.Id, actor.Frontend);
        return new JsonObject
        {
            ["summary"] = "Preview only. Review the diff, then repeat exactly the same arguments with apply=true and the confirmationToken.",
            ["change"] = change.DeepClone(),
            ["affectedObjects"] = 1,
            ["restartRequired"] = !operation.StartsWith("characters_", StringComparison.Ordinal),
            ["restartGuidance"] = "Changes use the existing configuration notification path. Restart affected game servers when their subsystem does not reload these settings.",
            ["confirmationToken"] = token,
            ["expiresAt"] = expires.ToString("O"),
        };
    }

    private sealed record Confirmation(string Digest, DateTimeOffset ExpiresAt, string ActorId, string Frontend);
}
