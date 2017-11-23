// <copyright file="WorldObserverToHubAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// An observer which can observe a whole map.
    /// </summary>
    public sealed class WorldObserverToHubAdapter : IWorldObserver, IWorldView, ILocateable, IBucketMapObserver, IDisposable
    {
        private readonly WorldObserverHub hub;

        private readonly ObserverToWorldViewAdapter adapterToWorldView;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldObserverToHubAdapter" /> class.
        /// </summary>
        /// <param name="observerId">The observer identifier.</param>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="mapId">The map identifier.</param>
        /// <param name="hub">The hub.</param>
        /// <param name="connectionId">The connection identifier.</param>
        public WorldObserverToHubAdapter(ushort observerId, byte serverId, ushort mapId, WorldObserverHub hub, string connectionId)
        {
            this.Id = observerId;
            this.ServerId = serverId;
            this.hub = hub;
            this.MapId = mapId;
            this.ConnectionId = connectionId;
            this.adapterToWorldView = new ObserverToWorldViewAdapter(this, byte.MaxValue);
        }

        /// <summary>
        /// Gets the connection id.
        /// </summary>
        public string ConnectionId
        {
            get;
        }

        /// <inheritdoc/>
        public int InfoRange => byte.MaxValue;

        /// <summary>
        /// Gets the server identifier on which the observer is observing
        /// </summary>
        public byte ServerId { get; }

        /// <inheritdoc/>
        public IWorldView WorldView => this;

        /// <summary>
        /// Gets the identifier of the map.
        /// </summary>
        public ushort MapId { get; }

        /// <inheritdoc/>
        public GameMap CurrentMap => null;

        /// <inheritdoc/>
        public byte X { get; set; }

        /// <inheritdoc/>
        public byte Y { get; set; }

        /// <inheritdoc/>
        public ushort Id { get; }

        /// <inheritdoc/>
        public IList<Bucket<ILocateable>> ObservingBuckets => this.adapterToWorldView.ObservingBuckets;

        /// <inheritdoc/>
        public void ObjectGotKilled(IAttackable killedObject, IAttackable killerObject)
        {
            this.hub.ObjectGotKilled(this, killedObject, killerObject);
        }

        /// <inheritdoc/>
        public void ObjectMoved(ILocateable movedObject, MoveType moveType)
        {
            this.hub.ObjectMoved(this, movedObject, moveType);
        }

        /// <inheritdoc/>
        public void ShowDroppedItems(IEnumerable<DroppedItem> droppedItems, bool freshDrops)
        {
            this.hub.ShowDroppedItems(this, droppedItems, freshDrops);
        }

        /// <inheritdoc/>
        public void DroppedItemsDisappeared(IEnumerable<ushort> disappearedItemIds)
        {
            this.hub.DroppedItemsDisappeared(this, disappearedItemIds);
        }

        /// <inheritdoc/>
        public void ShowAnimation(IIdentifiable animatingObj, byte animation, IIdentifiable targetObj, byte direction)
        {
            this.hub.ShowAnimation(this, animatingObj, animation, targetObj, direction);
        }

        /// <inheritdoc/>
        public void MapChange()
        {
            throw new NotImplementedException("WorldObserver can't change the map.");
        }

        /// <inheritdoc/>
        public void ObjectsOutOfScope(IEnumerable<IIdentifiable> objects)
        {
            this.hub.ObjectsOutOfScope(this, objects);
        }

        /// <inheritdoc/>
        public void NewPlayersInScope(IEnumerable<Player> newObjects)
        {
            this.hub.NewPlayersInScope(this, newObjects);
        }

        /// <inheritdoc/>
        public void NewNpcsInScope(IEnumerable<NonPlayerCharacter> newObjects)
        {
            this.hub.NewNpcsInScope(this, newObjects);
        }

        /// <inheritdoc/>
        public void UpdateRotation()
        {
            throw new NotImplementedException("WorldObserver does not have an own rotation.");
        }

        /// <inheritdoc/>
        public void ShowSkillAnimation(Player attackingPlayer, IAttackable target, Skill skill)
        {
            this.hub.ShowSkillAnimation(this, attackingPlayer, target, skill);
        }

        /// <inheritdoc/>
        public void ShowAreaSkillAnimation(Player player, Skill skill, byte x, byte y, byte rotation)
        {
            this.hub.ShowAreaSkillAnimation(this, player, skill, x, y, rotation);
        }

        /// <inheritdoc/>
        public void LocateableAdded(object sender, BucketItemEventArgs<ILocateable> eventArgs)
        {
            this.adapterToWorldView.LocateableAdded(sender, eventArgs);
        }

        /// <inheritdoc/>
        public void LocateableRemoved(object sender, BucketItemEventArgs<ILocateable> eventArgs)
        {
            this.adapterToWorldView.LocateableRemoved(sender, eventArgs);
        }

        /// <inheritdoc/>
        public void LocateablesOutOfScope(IEnumerable<ILocateable> oldObjects)
        {
            this.adapterToWorldView.LocateablesOutOfScope(oldObjects);
        }

        /// <inheritdoc/>
        public void NewLocateablesInScope(IEnumerable<ILocateable> newObjects)
        {
            this.adapterToWorldView.NewLocateablesInScope(newObjects);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.adapterToWorldView.Dispose();
        }
    }
}
