// <copyright file="AdministrationDeliveries.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using System.Text.Json;
using MUnique.OpenMU.Dapr.Common;
using Nito.AsyncEx;

/// <summary>Prevents service-invocation retries from repeating a live write in this process.</summary>
/// <remarks>Results, including failures, survive client cancellation. An uncertain response must never be retried as a new operation.</remarks>
public sealed class AdministrationDeliveries
{
    private readonly object _sync = new();
    private readonly Dictionary<Guid, Delivery> _requests = new();

    /// <summary>Executes one delivery or returns its original result.</summary>
    /// <typeparam name="T">The safe result type.</typeparam>
    /// <param name="operation">The internal method name.</param>
    /// <param name="request">The request with its stable delivery identifier.</param>
    /// <param name="action">The live write.</param>
    /// <returns>The original confirmed result.</returns>
    public async ValueTask<T> RunAsync<T>(string operation, GameServerAdministrationRequest request, Func<ValueTask<T>> action)
    {
        if (request.RequestId == Guid.Empty)
        {
            throw new ArgumentException("A delivery identifier is required.", nameof(request));
        }

        var fingerprint = operation + JsonSerializer.Serialize(request);
        Delivery delivery;
        lock (this._sync)
        {
            // Retain pending deliveries; evict completed results only after the transport retry window.
            foreach (var key in this._requests.Where(p => p.Value.CreatedAt < DateTime.UtcNow.AddMinutes(-10) && p.Value.Result.IsStarted && p.Value.Result.Task.IsCompleted).Select(p => p.Key).ToArray())
            {
                this._requests.Remove(key);
            }

            if (this._requests.TryGetValue(request.RequestId, out var existing))
            {
                if (existing.Fingerprint != fingerprint)
                {
                    throw new ArgumentException("A delivery identifier cannot be reused with different arguments.", nameof(request));
                }

                delivery = existing;
            }
            else
            {
                if (this._requests.Count >= 1000)
                {
                    throw new InvalidOperationException("The live delivery cache is full. Wait before submitting new operations.");
                }

                delivery = new(fingerprint, DateTime.UtcNow, new AsyncLazy<object?>(async () => await action().ConfigureAwait(false)));
                this._requests.Add(request.RequestId, delivery);
            }
        }

        return (T)(await delivery.Result.ConfigureAwait(false))!;
    }

    private sealed record Delivery(string Fingerprint, DateTime CreatedAt, AsyncLazy<object?> Result);
}
