// <copyright file="CircuitConnectionState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map;

using System.Threading;
using Microsoft.AspNetCore.Components.Server.Circuits;

/// <summary>
/// Tracks whether the browser behind the current blazor circuit is reachable.
/// </summary>
/// <remarks>
/// A <see cref="Microsoft.JSInterop.JSDisconnectedException"/> can't tell a short connection loss,
/// which the reconnect modal recovers from, apart from a closed tab. The <see cref="CircuitHandler"/>
/// reports them separately, so the live map can drop its calls while the browser is away and
/// resynchronize when it comes back. A circuit which is closed for good disposes its components,
/// which takes the map down with it.
/// </remarks>
public sealed class CircuitConnectionState : CircuitHandler
{
    private volatile bool _isConnected = true;

    /// <summary>
    /// Occurs when the browser reconnected after a temporary connection loss.
    /// </summary>
    /// <remarks>
    /// It's raised before the circuit carries js interop calls again, so handlers must not call
    /// the browser right away, and must not block.
    /// </remarks>
    public event EventHandler? ConnectionRestored;

    /// <summary>
    /// Gets a value indicating whether the browser is currently reachable.
    /// </summary>
    /// <remarks>
    /// It starts out as <c>true</c>, because components are only rendered on a live circuit, and
    /// <see cref="OnConnectionUpAsync"/> isn't raised for the connection they started on.
    /// </remarks>
    public bool IsConnected => this._isConnected;

    /// <inheritdoc />
    public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        this._isConnected = false;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var wasDown = !this._isConnected;
        this._isConnected = true;

        if (wasDown)
        {
            this.ConnectionRestored?.Invoke(this, EventArgs.Empty);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        this._isConnected = false;
        return Task.CompletedTask;
    }
}
