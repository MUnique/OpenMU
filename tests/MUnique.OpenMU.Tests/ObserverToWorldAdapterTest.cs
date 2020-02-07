// <copyright file="ObserverToWorldAdapterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;
    using NUnit.Framework;

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
        public void LocateableAddedAlreadyExists()
        {
            var worldObserver = new Mock<IWorldObserver>();
            var view = new Mock<INewNpcsInScopePlugIn>();
            var viewPlugIns = new Mock<ICustomPlugInContainer<IViewPlugIn>>();
            viewPlugIns.Setup(v => v.GetPlugIn<INewNpcsInScopePlugIn>()).Returns(view.Object);
            worldObserver.Setup(o => o.ViewPlugIns).Returns(viewPlugIns.Object);
            var adapter = new ObserverToWorldViewAdapter(worldObserver.Object, 12);
            var map = new GameMap(new DataModel.Configuration.GameMapDefinition(), 10, 8);
            var nonPlayer = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), map)
            {
                Position = new Point(128, 128),
            };
            map.Add(nonPlayer);
            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer));
            adapter.ObservingBuckets.Add(nonPlayer.NewBucket);
            nonPlayer.OldBucket = nonPlayer.NewBucket; // oldbucket would be set, if it got moved on the map

            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer));
            view.Verify(v => v.NewNpcsInScope(It.Is<IEnumerable<NonPlayerCharacter>>(arg => arg.Contains(nonPlayer))), Times.Once);
        }

        /// <summary>
        /// Tests if a <see cref="ILocateable"/> is not reported as out of scope to the view plugins when its new bucket is still observed.
        /// </summary>
        [Test]
        public void LocateableNotOutOfScopeWhenMovedToObservedBucket()
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
            var map = new GameMap(new DataModel.Configuration.GameMapDefinition(), 10, 8);
            var nonPlayer1 = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), map)
            {
                Position = new Point(128, 128),
            };
            map.Add(nonPlayer1);
            var nonPlayer2 = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), map)
            {
                Position = new Point(100, 128),
            };
            map.Add(nonPlayer2);
            adapter.ObservingBuckets.Add(nonPlayer1.NewBucket);
            adapter.ObservingBuckets.Add(nonPlayer2.NewBucket);

            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer1));
            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer2));

            map.Move(nonPlayer1, nonPlayer2.Position, nonPlayer1, MoveType.Instant);

            view1.Verify(v => v.NewNpcsInScope(It.Is<IEnumerable<NonPlayerCharacter>>(arg => arg.Contains(nonPlayer1))), Times.Once);
            view1.Verify(v => v.NewNpcsInScope(It.Is<IEnumerable<NonPlayerCharacter>>(arg => arg.Contains(nonPlayer2))), Times.Once);
            view2.Verify(v => v.ObjectsOutOfScope(It.IsAny<IEnumerable<IIdentifiable>>()), Times.Never);
            view3.Verify(v => v.ObjectMoved(It.Is<ILocateable>(arg => arg == nonPlayer1), MoveType.Instant), Times.Once);
        }
    }
}
