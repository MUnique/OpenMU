// <copyright file="GameServerSelectionFieldBuilderTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.GameLogic.Raklion;
using MUnique.OpenMU.Web.Shared.ComponentBuilders;

/// <summary>
/// Tests for the <see cref="GameServerSelectionFieldBuilder"/> routing.
/// </summary>
[TestFixture]
public class GameServerSelectionFieldBuilderTests
{
    private readonly GameServerSelectionFieldBuilder _builder = new();

    /// <summary>
    /// The server scope of periodic task configurations gets the checkbox list.
    /// </summary>
    [Test]
    public void MatchesAllowedServerIds()
    {
        var property = typeof(PeriodicTaskConfiguration).GetProperty(nameof(PeriodicTaskConfiguration.AllowedServerIds));

        Assert.That(this._builder.CanBuildComponent(property!), Is.True);
    }

    /// <summary>
    /// The server scope of derived configurations gets the checkbox list, through inheritance.
    /// </summary>
    [Test]
    public void MatchesInheritedAllowedServerIds()
    {
        var property = typeof(PeriodicInvasionConfiguration).GetProperty(nameof(PeriodicTaskConfiguration.AllowedServerIds));

        Assert.That(this._builder.CanBuildComponent(property!), Is.True);
    }

    /// <summary>
    /// Other byte lists keep their generic editor.
    /// </summary>
    [Test]
    public void IgnoresUnrelatedByteLists()
    {
        var property = typeof(RaklionEventDefinition).GetProperty(nameof(RaklionEventDefinition.FullyPushedCharacterClassNumbers));

        Assert.That(this._builder.CanBuildComponent(property!), Is.False);
    }

    /// <summary>
    /// Other list types keep their generic editor.
    /// </summary>
    [Test]
    public void IgnoresOtherListTypes()
    {
        var property = typeof(PeriodicTaskConfiguration).GetProperty(nameof(PeriodicTaskConfiguration.Timetable));

        Assert.That(this._builder.CanBuildComponent(property!), Is.False);
    }
}
