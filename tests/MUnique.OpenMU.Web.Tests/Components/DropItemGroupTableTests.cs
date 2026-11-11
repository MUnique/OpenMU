// <copyright file="DropItemGroupTableTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using Bunit;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.AdminPanel.Components.ItemDrops;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="DropItemGroupTable"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DropItemGroupTableTests
{
    private BunitContext _context = null!;

    /// <summary>
    /// Creates the test context.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
    }

    /// <summary>
    /// Disposes the test context.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._context.Dispose();
    }

    /// <summary>
    /// A group which is not assigned to any map doesn't drop, so it's marked in the list.
    /// </summary>
    [Test]
    public void MarksGroupWhichIsNotAssignedToAnyMap()
    {
        var cut = this._context.Render<DropItemGroupTable>(parameters => parameters
            .Add(p => p.Groups, [CreateGroup()])
            .Add(p => p.Maps, [])
            .Add(p => p.WarnWhenUnassigned, true));

        Assert.That(cut.Markup, Does.Contain("Not assigned to any map"));
    }

    /// <summary>
    /// A group which is assigned to a map is not marked.
    /// </summary>
    [Test]
    public void DoesNotMarkGroupWhichIsAssignedToAMap()
    {
        var group = CreateGroup();
        var map = new BasicModel.GameMapDefinition { Id = Guid.NewGuid(), Name = "Test Map" };
        map.DropItemGroups.Add(group);

        var cut = this._context.Render<DropItemGroupTable>(parameters => parameters
            .Add(p => p.Groups, [group])
            .Add(p => p.Maps, [map])
            .Add(p => p.WarnWhenUnassigned, true));

        Assert.That(cut.Markup, Does.Not.Contain("Not assigned to any map"));
    }

    /// <summary>
    /// The marker is only shown where it's requested, e.g. not in the list of monster drops.
    /// </summary>
    [Test]
    public void DoesNotMarkWithoutTheParameter()
    {
        var cut = this._context.Render<DropItemGroupTable>(parameters => parameters
            .Add(p => p.Groups, [CreateGroup()]));

        Assert.That(cut.Markup, Does.Not.Contain("Not assigned to any map"));
    }

    private static BasicModel.DropItemGroup CreateGroup()
    {
        return new BasicModel.DropItemGroup
        {
            Id = Guid.NewGuid(),
            Description = "Test group",
            Chance = 0.5,
            ItemType = SpecialItemType.None,
        };
    }
}
