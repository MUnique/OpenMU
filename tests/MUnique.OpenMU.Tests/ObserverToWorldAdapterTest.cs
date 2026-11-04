// <copyright file="ObserverToWorldAdapterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Nito.AsyncEx;

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the <see cref="ObserverToWorldViewAdapter"/>.
/// </summary>
[TestFixture]
public class ObserverToWorldAdapterTest
{
    /// <summary>
    /// Tests if a <see cref="ILocateable"/> is only reported once to the <see cref="INewNpcsInScopePlugIn"/> when it's already known to it.
    /// </summary>
    [Test]
    public async ValueTask LocateableAddedAlreadyExistsAsync()
    {
        var worldObserver = new Mock<IWorldObserver>();
        var view = new Mock<INewNpcsInScopePlugIn>();
        var viewPlugIns = new Mock<ICustomPlugInContainer<IViewPlugIn>>();
        viewPlugIns.Setup(v => v.GetPlugIn<INewNpcsInScopePlugIn>()).Returns(view.Object);
        worldObserver.Setup(o => o.ViewPlugIns).Returns(viewPlugIns.Object);
        var adapter = new ObserverToWorldViewAdapter(worldObserver.Object, 12);
        var map = new GameMap(new DataModel.Configuration.GameMapDefinition(), TimeSpan.FromSeconds(10), 8);
        var nonPlayer = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), map)
        {
            Position = new Point(128, 128),
        };
        await map.AddAsync(nonPlayer).ConfigureAwait(false);
        await adapter.LocateableAddedAsync(nonPlayer).ConfigureAwait(false);
        adapter.ObservingBuckets.Add(nonPlayer.NewBucket!);
        nonPlayer.OldBucket = nonPlayer.NewBucket; // oldbucket would be set, if it got moved on the map

        await adapter.LocateableAddedAsync(nonPlayer).ConfigureAwait(false);
        view.Verify(v => v.NewNpcsInScopeAsync(It.Is<IEnumerable<NonPlayerCharacter>>(arg => arg.Contains(nonPlayer)), true), Times.Once);
    }

    /// <summary>
    /// Tests if a <see cref="ILocateable"/> is not reported as out of scope to the view plugins when its new bucket is still observed.
    /// </summary>
    [Test]
    public async ValueTask LocateableNotOutOfScopeWhenMovedToObservedBucketAsync()
    {
        var worldObserver = new Mock<IWorldObserver>();
        var view1 = new Mock<INewNpcsInScopePlugIn>();
        var view2 = new Mock<IObjectsOutOfScopePlugIn>();
        var view3 = new Mock<IObjectMovedPlugIn>();
        var viewPlugIns = new Mock<ICustomPlugInContainer<IViewPlugIn>>();
        viewPlugIns.Setup(v => v.GetPlugIn<INewNpcsInScopePlugIn>()).Returns(view1.Object);
        viewPlugIns.Setup(v => v.GetPlugIn<IObjectsOutOfScopePlugIn>()).Returns(view2.Object);
        viewPlugIns.Setup(v => v.GetPlugIn<IObjectMovedPlugIn>()).Returns(view3.Object);
        worldObserver.Setup(o => o.ViewPlugIns).Returns(viewPlugIns.Object);
        var adapter = new ObserverToWorldViewAdapter(worldObserver.Object, 12);
        var map = new GameMap(new DataModel.Configuration.GameMapDefinition(), TimeSpan.FromSeconds(10), 8);
        var nonPlayer1 = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), map)
        {
            Position = new Point(128, 128),
        };
        await map.AddAsync(nonPlayer1).ConfigureAwait(false);
        var nonPlayer2 = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), map)
        {
            Position = new Point(100, 128),
        };
        await map.AddAsync(nonPlayer2).ConfigureAwait(false);
        adapter.ObservingBuckets.Add(nonPlayer1.NewBucket!);
        adapter.ObservingBuckets.Add(nonPlayer2.NewBucket!);

        await adapter.LocateableAddedAsync(nonPlayer1).ConfigureAwait(false);
        await adapter.LocateableAddedAsync(nonPlayer2).ConfigureAwait(false);

        await map.MoveAsync(nonPlayer1, nonPlayer2.Position, new AsyncLock(), MoveType.Instant).ConfigureAwait(false);

        view1.Verify(v => v.NewNpcsInScopeAsync(It.Is<IEnumerable<NonPlayerCharacter>>(arg => arg.Contains(nonPlayer1)), true), Times.Once);
        view1.Verify(v => v.NewNpcsInScopeAsync(It.Is<IEnumerable<NonPlayerCharacter>>(arg => arg.Contains(nonPlayer2)), true), Times.Once);
        view2.Verify(v => v.ObjectsOutOfScopeAsync(It.IsAny<IEnumerable<IIdentifiable>>()), Times.Never);
        view3.Verify(v => v.ObjectMovedAsync(It.Is<ILocateable>(arg => arg == nonPlayer1), MoveType.Instant), Times.Once);
    }
}