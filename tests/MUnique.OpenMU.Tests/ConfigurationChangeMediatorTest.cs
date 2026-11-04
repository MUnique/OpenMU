// <copyright file="ConfigurationChangeMediatorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Tests for the <see cref="ConfigurationChangeMediator"/>.
/// </summary>
[TestFixture]
public class ConfigurationChangeMediatorTest
{
    /// <summary>
    /// Tests that disposing the registration for new configurations of one object only removes the handler of
    /// this object. The handlers of the other objects are still called for new configurations of the same type.
    /// </summary>
    [Test]
    public async ValueTask DisposingRegistrationForNewKeepsTheOtherRegistrationsAsync()
    {
        var mediator = new ConfigurationChangeMediator();
        var releasedObjectNotified = false;
        var remainingObjectNotified = false;
        var releasedRegistration = mediator.RegisterForNew<MonsterSpawnArea, object>(new object(), (_, _) =>
        {
            releasedObjectNotified = true;
            return ValueTask.CompletedTask;
        });
        using var remainingRegistration = mediator.RegisterForNew<MonsterSpawnArea, object>(new object(), (_, _) =>
        {
            remainingObjectNotified = true;
            return ValueTask.CompletedTask;
        });

        releasedRegistration.Dispose();
        var spawnArea = new MonsterSpawnArea();
        await mediator.HandleConfigurationAddedAsync(typeof(MonsterSpawnArea), Guid.NewGuid(), spawnArea).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(releasedObjectNotified, Is.False);
            Assert.That(remainingObjectNotified, Is.True);
        });
    }
}
