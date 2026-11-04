// <copyright file="IConfigurationEditContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Threading;

/// <summary>Provides an isolated configuration read/validate/write scope for providers with database transactions.</summary>
/// <remarks>Call before reading. Save commits before publishing configuration notifications. Disposing without save rolls back.</remarks>
public interface IConfigurationEditContext
{
    /// <summary>Begins a serializable configuration edit transaction on this owned context.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask BeginConfigurationEditAsync(CancellationToken cancellationToken);
}
