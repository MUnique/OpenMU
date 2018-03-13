// <copyright file="GameMapTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using NUnit.Framework;
    using Rhino.Mocks;

    /// <summary>
    /// Tests for the game map.
    /// </summary>
    [TestFixture]
    public class GameMapTest
    {
        private const byte ChunkSize = 8;

        /// <summary>
        /// An interface which combines several other interfaces which are needed in combination for this test.
        /// </summary>
        public interface ITestPlayer : ILocateable, IBucketMapObserver, IObservable
        {
        }

        /// <summary>
        /// Tests if the discovery of players works when a new player is entering the map in the view range.
        /// </summary>
        [Test]
        public void TestPlayerEntersMap()
        {
            var map = new GameMap(new GameMapDefinition(), 60, ChunkSize);
            var player1 = this.GetPlayer();
            player1.Stub(p => p.Id).Return(1);
            player1.X = 100;
            player1.Y = 100;
            map.Add(player1);
            var player2 = this.GetPlayer();
            player2.Stub(p => p.Id).Return(2);
            player2.X = 101;
            player2.Y = 100;
            map.Add(player2);
            player1.AssertWasCalled(p => p.NewLocateablesInScope(null), o => o.IgnoreArguments());
            player2.AssertWasCalled(p => p.NewLocateablesInScope(null), o => o.IgnoreArguments());
            player1.AssertWasCalled(p => p.LocateableAdded(null, null), o => o.IgnoreArguments());
        }

        /// <summary>
        /// Tests if movements of a player into the view range of another player causes
        /// that the players get notified about each other as soon as they are in view range.
        /// </summary>
        [Test]
        public void TestPlayerMovesInMap()
        {
            var map = new GameMap(new GameMapDefinition(), 60, ChunkSize);
            var player1 = this.GetPlayer();
            player1.Stub(p => p.Id).Return(1);

            map.Add(player1);
            var player2 = this.GetPlayer();
            player2.Stub(p => p.Id).Return(2);
            player2.X = 101;
            player2.Y = 100;
            map.Add(player2);

            map.Move(player1, 100, 100, new object(), 0);
            player1.AssertWasCalled(p => p.NewLocateablesInScope(null), o => o.IgnoreArguments().Repeat.Times(2));
            player2.AssertWasCalled(p => p.NewLocateablesInScope(null), o => o.IgnoreArguments().Repeat.Times(1));
            player2.AssertWasCalled(p => p.LocateableAdded(null, null), o => o.IgnoreArguments().Repeat.Once());
        }

        /// <summary>
        /// Tests if movements of a player out of the view range of another player causes
        /// that the players get notified about it.
        /// </summary>
        [Test]
        public void PlayerMovesOutOfRange()
        {
            var map = new GameMap(new GameMapDefinition(), 60, ChunkSize);
            var player1 = this.GetPlayer();
            player1.Stub(p => p.Id).Return(1);
            player1.X = 101;
            player1.Y = 100;
            map.Add(player1);
            var player2 = this.GetPlayer();
            player2.Stub(p => p.Id).Return(2);
            player2.X = 101;
            player2.Y = 100;
            map.Add(player2);

            map.Move(player1, 100, 130, new object(), 0);
            player1.AssertWasCalled(p => p.LocateablesOutOfScope(null), o => o.IgnoreArguments());
            player2.AssertWasCalled(p => p.LocateableRemoved(null, null), o => o.IgnoreArguments());
        }

        /// <summary>
        /// Tests if movements of a player into and out of the view range of another player causes
        /// that the players get notified about it.
        /// </summary>
        [Test]
        public void PlayerMovesOutAndIntoTheRange()
        {
            var map = new GameMap(new GameMapDefinition(), 60, ChunkSize);
            var player1 = this.GetPlayer();
            player1.Stub(p => p.Id).Return(1);
            player1.X = 101;
            player1.Y = 100;
            map.Add(player1);
            var player2 = this.GetPlayer();
            player2.Stub(p => p.Id).Return(2);
            player2.X = 101;
            player2.Y = 100;
            map.Add(player2);

            map.Move(player1, 100, 130, new object(), 0);
            player1.AssertWasCalled(p => p.LocateablesOutOfScope(null), o => o.IgnoreArguments());
            player2.AssertWasCalled(p => p.LocateableRemoved(null, null), o => o.IgnoreArguments());
            player2.AssertWasCalled(p => p.NewLocateablesInScope(null), o => o.IgnoreArguments().Repeat.Times(1));
            player1.AssertWasCalled(p => p.LocateableAdded(null, null), o => o.IgnoreArguments().Repeat.Times(1));

            map.Move(player2, 101, 130, new object(), 0);
            player2.AssertWasCalled(p => p.NewLocateablesInScope(null), o => o.IgnoreArguments().Repeat.Times(2));
            player1.AssertWasCalled(p => p.LocateableAdded(null, null), o => o.IgnoreArguments().Repeat.Times(2));
        }

        /// <summary>
        /// Tests the performance of the movements. Not a standard test.
        /// </summary>
        /// [Test]
        public void TestPerformanceMove()
        {
            var map = new GameMap(new GameMapDefinition(), 60, ChunkSize);
            var player1 = this.GetPlayer();
            player1.Stub(p => p.Id).Return(1);

            map.Add(player1);
            var player2 = this.GetPlayer();
            player2.Stub(p => p.Id).Return(2);
            player2.X = 101;
            player2.Y = 100;
            map.Add(player2);

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var moveLock = new object();
            for (int i = 0; i < 1000; i++)
            {
                map.Move(player1, (byte)(100 + (i % 30)), (byte)(100 + (i % 30)), moveLock, 0);
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Tests if the players in view range get notified when another player leaves the map.
        /// </summary>
        [Test]
        public void TestPlayerLeavesMap()
        {
            var map = new GameMap(new GameMapDefinition(), 60, ChunkSize);
            var player1 = this.GetPlayer();
            player1.Stub(p => p.Id).Return(1);
            player1.X = 100;
            player1.Y = 100;
            map.Add(player1);
            var player2 = this.GetPlayer();
            player2.Stub(p => p.Id).Return(2);
            player2.X = 101;
            player2.Y = 100;
            map.Add(player2);
            map.Remove(player2);
            Assert.AreEqual(player2.ObservingBuckets.Count, 0);
            player1.AssertWasCalled(p => p.LocateableRemoved(null, null), o => o.IgnoreArguments());
            player2.AssertWasCalled(p => p.LocateableRemoved(null, null), o => o.IgnoreArguments());
            Assert.That(player1.Observers.Count, Is.EqualTo(0));
            Assert.That(player2.Observers.Count, Is.EqualTo(0));
        }

        private ITestPlayer GetPlayer()
        {
            var player = MockRepository.GenerateStub<ITestPlayer>();
            player.Stub(p => p.ObservingBuckets).Return(new List<Bucket<ILocateable>>());
            player.Stub(p => p.Observers).Return(new HashSet<IWorldObserver>());
            player.Stub(p => p.ObserverLock).Return(new System.Threading.ReaderWriterLockSlim());
            player.Stub(p => p.InfoRange).Return(20);
            player.Stub(p => p.AddObserver(null)).IgnoreArguments().WhenCalled(i => player.Observers.Add(i.Arguments[0] as IWorldObserver));
            player.Stub(p => p.RemoveObserver(null)).IgnoreArguments().WhenCalled(i => player.Observers.Remove(i.Arguments[0] as IWorldObserver));
            return player;
        }
    }
}
