// <copyright file="ObserverToWorldAdapterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;
    using NUnit.Framework;
    using Rhino.Mocks;

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
            var worldObserver = MockRepository.GenerateStub<IWorldObserver>();
            var view = MockRepository.GenerateStrictMock<IWorldView>();
            worldObserver.Stub(o => o.WorldView).Return(view);
            var adapter = new ObserverToWorldViewAdapter(worldObserver, 12);
            var map = new GameMap(new DataModel.Configuration.GameMapDefinition(), 10, 8, 1000);
            var nonPlayer = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), 0, map)
            {
                X = 128,
                Y = 128
            };
            map.Add(nonPlayer);
            view.Expect(v => v.NewNpcsInScope(null)).IgnoreArguments().Repeat.Once();
            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer));
            adapter.ObservingBuckets.Add(nonPlayer.CurrentBucket);
            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer));
            view.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests if a <see cref="ILocateable"/> is not reported as out of scope to the <see cref="IWorldView"/> when its new bucket is still observed.
        /// </summary>
        [Test]
        public void LocateableNotOutOfScopeWhenMovedToObservedBucket()
        {
            var worldObserver = MockRepository.GenerateStub<IWorldObserver>();
            var view = MockRepository.GenerateStrictMock<IWorldView>();
            worldObserver.Stub(o => o.WorldView).Return(view);
            var adapter = new ObserverToWorldViewAdapter(worldObserver, 12);
            var map = new GameMap(new DataModel.Configuration.GameMapDefinition(), 10, 8, 1000);
            var nonPlayer1 = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), 0, map)
            {
                X = 128,
                Y = 128
            };
            map.Add(nonPlayer1);
            var nonPlayer2 = new NonPlayerCharacter(new DataModel.Configuration.MonsterSpawnArea(), new DataModel.Configuration.MonsterDefinition(), 1, map)
            {
                X = 100,
                Y = 128
            };
            map.Add(nonPlayer2);
            adapter.ObservingBuckets.Add(nonPlayer1.CurrentBucket);
            adapter.ObservingBuckets.Add(nonPlayer2.CurrentBucket);
            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer1));
            adapter.LocateableAdded(map, new BucketItemEventArgs<ILocateable>(nonPlayer2));

            view.Expect(v => v.ObjectsOutOfScope(null)).IgnoreArguments().Repeat.Never();
            map.Move(nonPlayer1, nonPlayer2.X, nonPlayer2.Y, nonPlayer1, MoveType.Instant);
            view.VerifyAllExpectations();
        }
    }
}
