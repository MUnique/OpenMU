// <copyright file="UpdatePlugInConventionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests that all configuration update plugins follow the versioning conventions:
/// a positive version, an immutable creation date without future dates,
/// and an update date which is never before the creation date.
/// </summary>
[TestFixture]
internal class UpdatePlugInConventionsTest
{
    /// <summary>
    /// Tests that all update plugins shipped in the initialization assembly
    /// follow the versioning conventions.
    /// </summary>
    [Test]
    public void AllUpdatePluginsFollowVersioningConvention()
    {
        // Reference a type of the initialization assembly, so that it's loaded for the discovery.
        _ = typeof(UpdatePlugInBase);
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.DiscoverAndRegisterPlugInsOf<IConfigurationUpdatePlugIn>();
        var provider = manager.GetStrategyProvider<Guid, IConfigurationUpdatePlugIn>();
        Assert.That(provider, Is.Not.Null);

        var plugins = provider!.AvailableStrategies
            .Where(plugin => plugin.GetType().Assembly == typeof(UpdatePlugInBase).Assembly)
            .ToList();
        Assert.That(plugins, Is.Not.Empty);

        foreach (var plugin in plugins)
        {
            Assert.Multiple(() =>
            {
                Assert.That(plugin.Version, Is.GreaterThanOrEqualTo(1), $"Update {plugin.Name} has an invalid version.");
                Assert.That(plugin.CreatedAt, Is.LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)), $"Update {plugin.Name} has a creation date in the future.");
                Assert.That(plugin.UpdatedAt, Is.GreaterThanOrEqualTo(plugin.CreatedAt), $"Update {plugin.Name} was updated before it was created.");
            });
        }
    }
}
