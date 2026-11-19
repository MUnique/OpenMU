// <copyright file="NoEffectTimeDefaultsSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Threading;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>
/// A source which has no defaults, like a host which can't determine them.
/// </summary>
internal sealed class NoEffectTimeDefaultsSource : IEffectTimeDefaultsSource
{
    /// <inheritdoc />
    public ValueTask<EffectTimeDefaultValues?> GetAsync(CancellationToken cancellationToken)
        => ValueTask.FromResult<EffectTimeDefaultValues?>(null);
}
