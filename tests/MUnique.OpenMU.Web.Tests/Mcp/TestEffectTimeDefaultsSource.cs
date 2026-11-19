// <copyright file="TestEffectTimeDefaultsSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Threading;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>
/// The made-up defaults of the tests: in them, effect 1 doesn't send its time and effect 6 does.
/// It doesn't execute the real Season 6 data initialization in memory, which is slow when many tests run at once;
/// the real defaults are tested with the data initialization.
/// </summary>
internal sealed class TestEffectTimeDefaultsSource : IEffectTimeDefaultsSource
{
    /// <inheritdoc />
    public ValueTask<EffectTimeDefaultValues?> GetAsync(CancellationToken cancellationToken)
        => ValueTask.FromResult<EffectTimeDefaultValues?>(new EffectTimeDefaultValues(
            Persistence.Initialization.VersionSeasonSix.DataInitialization.Id,
            "1.04d - Season 6 Episode 3",
            new Dictionary<short, bool> { [1] = false, [6] = true }));
}
