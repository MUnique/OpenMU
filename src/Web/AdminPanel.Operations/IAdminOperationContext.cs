// <copyright file="IAdminOperationContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Threading;

/// <summary>Resolves the authenticated caller from the hosting front end.</summary>
public interface IAdminOperationContext
{
    /// <summary>Gets the current caller; anonymous callers must be rejected.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authenticated caller.</returns>
    ValueTask<AdminActor> GetActorAsync(CancellationToken cancellationToken);
}
