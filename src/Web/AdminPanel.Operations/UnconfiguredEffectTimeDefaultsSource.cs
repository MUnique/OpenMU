// <copyright file="UnconfiguredEffectTimeDefaultsSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Threading;

/// <summary>Provides no defaults when a host has not installed its source of the data initializations.</summary>
internal sealed class UnconfiguredEffectTimeDefaultsSource : IEffectTimeDefaultsSource
{
    /// <inheritdoc />
    public ValueTask<EffectTimeDefaultValues?> GetAsync(CancellationToken cancellationToken)
        => ValueTask.FromResult<EffectTimeDefaultValues?>(null);
}
