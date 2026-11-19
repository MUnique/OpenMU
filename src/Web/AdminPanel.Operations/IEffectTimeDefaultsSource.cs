// <copyright file="IEffectTimeDefaultsSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Threading;

/// <summary>
/// Knows which magic effects send their remaining time in the data which the installed configuration was created from,
/// e.g. the Season 6 data. The host provides it, because the data initializations aren't known to the operations.
/// </summary>
public interface IEffectTimeDefaultsSource
{
    /// <summary>
    /// Gets the defaults of the installed data. The first call may take a while, because the data is created in memory.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The defaults, or <c>null</c> if the host can't provide them.</returns>
    ValueTask<EffectTimeDefaultValues?> GetAsync(CancellationToken cancellationToken);
}
