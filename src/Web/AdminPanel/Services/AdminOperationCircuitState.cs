// <copyright file="AdminOperationCircuitState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Threading;
using Microsoft.AspNetCore.Components.Server.Circuits;

/// <summary>Positively identifies the scoped service provider of a running panel circuit.</summary>
public sealed class AdminOperationCircuitState : CircuitHandler
{
    /// <summary>Gets a value indicating whether this scope belongs to an open panel circuit.</summary>
    public bool IsOpen { get; private set; }

    /// <inheritdoc />
    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        this.IsOpen = true;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        this.IsOpen = false;
        return Task.CompletedTask;
    }
}
