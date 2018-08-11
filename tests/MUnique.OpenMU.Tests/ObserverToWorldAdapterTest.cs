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
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="ObserverToWorldViewAdapter"/>.
    /// </summary>
    [TestFixture]
    public class ObserverToWorldAdapterTest
    {
        /// <summary>
        /// Tests if a <see cref="ILocateable"/> is only reported once to the <see cref="IWorldView"/> when it's already known to it.
        /// </summary>
        [Test]
        public void LocateableAddedAlreadyExists()
        {
            var worldObserver = new Mock<IWorldObserver>();
            var view = new Mock<IWorldView>();
            worldObserver.Setup(o => o.WorldView).Returns(view.Object);
            var adapter = new ObserverToWorldViewAdapter(worldObserver.Object, 12);
            var map = new GameMap(new DataModel.Configuration.GameMapDefinition(), 10, 8, null);
            var nonPlayer = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), map)
            {
                X = 128,
                Y = 128
            };
            map.Add(nonPlayer);
            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer));
            adapter.ObservingBuckets.Add(nonPlayer.NewBucket);
            nonPlayer.OldBucket = nonPlayer.NewBucket; // oldbucket would be set, if it got moved on the map

            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer));
            view.Verify(v => v.NewNpcsInScope(It.Is<IEnumerable<NonPlayerCharacter>>(arg => arg.Contains(nonPlayer))), Times.Once);
        }

        /// <summary>
        /// Tests if a <see cref="ILocateable"/> is not reported as out of scope to the <see cref="IWorldView"/> when its new bucket is still observed.
        /// </summary>
        [Test]
        public void LocateableNotOutOfScopeWhenMovedToObservedBucket()
        {
            var worldObserver = new Mock<IWorldObserver>();
            var view = new Mock<IWorldView>();
            worldObserver.Setup(o => o.WorldView).Returns(view.Object);
            var adapter = new ObserverToWorldViewAdapter(worldObserver.Object, 12);
            var map = new GameMap(new DataModel.Configuration.GameMapDefinition(), 10, 8, null);
            var nonPlayer1 = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), map)
            {
                X = 128,
                Y = 128
            };
            map.Add(nonPlayer1);
            var nonPlayer2 = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), map)
            {
                X = 100,
                Y = 128
            };
            map.Add(nonPlayer2);
            adapter.ObservingBuckets.Add(nonPlayer1.NewBucket);
            adapter.ObservingBuckets.Add(nonPlayer2.NewBucket);

            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer1));
            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer2));

            map.Move(nonPlayer1, nonPlayer2.X, nonPlayer2.Y, nonPlayer1, MoveType.Instant);

            view.Verify(v => v.NewNpcsInScope(It.Is<IEnumerable<NonPlayerCharacter>>(arg => arg.Contains(nonPlayer1))), Times.Once);
            view.Verify(v => v.NewNpcsInScope(It.Is<IEnumerable<NonPlayerCharacter>>(arg => arg.Contains(nonPlayer2))), Times.Once);
            view.Verify(v => v.ObjectsOutOfScope(It.IsAny<IEnumerable<IIdentifiable>>()), Times.Never);
            view.Verify(v => v.ObjectMoved(It.Is<ILocateable>(arg => arg == nonPlayer1), MoveType.Instant), Times.Once);
        }
    }
}
