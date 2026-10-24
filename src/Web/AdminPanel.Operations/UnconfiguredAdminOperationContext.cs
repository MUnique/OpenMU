// <copyright file="UnconfiguredAdminOperationContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Threading;

/// <summary>Fails closed when a host has not installed its identity adapter.</summary>
internal sealed class UnconfiguredAdminOperationContext : IAdminOperationContext
{
    /// <inheritdoc />
    public ValueTask<AdminActor> GetActorAsync(CancellationToken cancellationToken)
        => throw new AdminOperationException("This host has no authenticated admin operation context.");
}
