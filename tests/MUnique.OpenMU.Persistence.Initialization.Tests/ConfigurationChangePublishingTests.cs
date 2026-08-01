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
    /// <param name="entityType">The entity type.</param>
    /// <param name="shouldPublish">Whether changes of this entity type should be published.</param>
    [TestCase(typeof(CastleSiegeConfiguration), true)]
    [TestCase(typeof(CastleSiegeNpcState), false)]
    [TestCase(typeof(Account), false)]
    [TestCase(typeof(Guild), false)]
    [TestCase(typeof(Friend), false)]
    public void OnlyConfigurationEntitiesArePublished(Type entityType, bool shouldPublish)
    {
        using var context = new EntityDataContext();
        var modelType = context.Model.FindEntityType(entityType);

        Assert.That(modelType, Is.Not.Null);
        Assert.That(EntityFrameworkContextBase.PublishesConfigurationChanges(modelType!), Is.EqualTo(shouldPublish));
    }
}
