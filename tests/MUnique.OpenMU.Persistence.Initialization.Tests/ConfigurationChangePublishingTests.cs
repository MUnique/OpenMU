// <copyright file="ConfigurationChangePublishingTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Tests filtering of Entity Framework configuration change notifications.
/// </summary>
[TestFixture]
internal class ConfigurationChangePublishingTests
{
    /// <summary>
    /// Verifies that only configuration entities are published to the configuration change listener.
    /// </summary>
    [Test]
    public void OnlyConfigurationEntitiesArePublished()
    {
        using var context = new EntityDataContext();
        var configurationType = context.Model.FindEntityType(typeof(CastleSiegeConfiguration));
        var dataType = context.Model.FindEntityType(typeof(CastleSiegeNpcState));

        Assert.Multiple(() =>
        {
            Assert.That(configurationType, Is.Not.Null);
            Assert.That(dataType, Is.Not.Null);
            Assert.That(EntityFrameworkContextBase.PublishesConfigurationChanges(configurationType!), Is.True);
            Assert.That(EntityFrameworkContextBase.PublishesConfigurationChanges(dataType!), Is.False);
        });
    }
}
