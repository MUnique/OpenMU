﻿// <copyright file="WorldObserverToHubAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.SignalR;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// An observer which can observe a whole map.
    /// </summary>
    public sealed class WorldObserverToHubAdapter : IWorldObserver, IWorldView, ILocateable, IBucketMapObserver, IDisposable
    {
        private readonly IClientProxy clientProxy;
        private readonly ObserverToWorldViewAdapter adapterToWorldView;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldObserverToHubAdapter" /> class.
        /// </summary>
        /// <param name="observerId">The observer identifier.</param>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="mapId">The map identifier.</param>
        /// <param name="clientProxy">The client proxy.</param>
        public WorldObserverToHubAdapter(ushort observerId, byte serverId, ushort mapId, IClientProxy clientProxy)
        {
            this.clientProxy = clientProxy;
            this.Id = observerId;
            this.ServerId = serverId;
            this.MapId = mapId;
            this.adapterToWorldView = new ObserverToWorldViewAdapter(this, byte.MaxValue);
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
            this.clientProxy.SendAsync("ObjectGotKilled", killedObject, killerObject);
        }

        /// <inheritdoc/>
        public void ObjectMoved(ILocateable movedObject, MoveType moveType)
        {
            Point targetPoint = new Point(movedObject.X, movedObject.Y);
            object steps = null;
            int walkDelay = 0;
            if (movedObject is ISupportWalk walker && moveType == MoveType.Walk)
            {
                targetPoint = walker.WalkTarget;
                walkDelay = (int)walker.StepDelay.TotalMilliseconds;
                Span<WalkingStep> walkingSteps = new WalkingStep[16];
                var stepCount = walker.GetSteps(walkingSteps);
                var walkSteps = walkingSteps.Slice(0, stepCount).ToArray().Select(step => new { x = step.To.X, y = step.To.Y, direction = step.Direction }).ToList();

                var lastStep = walkSteps.LastOrDefault();
                if (lastStep != null)
                {
                    var lastPoint = new Point(lastStep.x, lastStep.y);
                    var lastDirection = lastPoint.GetDirectionTo(targetPoint);
                    if (lastDirection != Direction.Undefined)
                    {
                        walkSteps.Add(new { x = targetPoint.X, y = targetPoint.Y, direction = lastDirection });
                    }
                }

                steps = walkSteps;
            }

            this.clientProxy.SendAsync("ObjectMoved", movedObject.Id, targetPoint.X, targetPoint.Y, moveType, walkDelay, steps);
        }

        /// <inheritdoc/>
        public void ShowDroppedItems(IEnumerable<DroppedItem> droppedItems, bool freshDrops)
        {
            this.clientProxy.SendAsync("ShowDroppedItems", droppedItems.Select(drop => new { id = drop.Id, x = drop.X, y = drop.Y, itemName = drop.Item.ToString() }), freshDrops);
        }

        /// <inheritdoc/>
        public void DroppedItemsDisappeared(IEnumerable<ushort> disappearedItemIds)
        {
            this.clientProxy.SendAsync("DroppedItemsDisappeared", disappearedItemIds);
        }

        /// <inheritdoc/>
        public void ShowAnimation(IIdentifiable animatingObj, byte animation, IIdentifiable targetObj, byte direction)
        {
            this.clientProxy.SendAsync("ShowAnimation", animatingObj.Id, animation, targetObj?.Id, direction);
        }

        /// <inheritdoc/>
        public void MapChange()
        {
            throw new NotImplementedException("WorldObserver can't change the map.");
        }

        /// <inheritdoc/>
        public void ObjectsOutOfScope(IEnumerable<IIdentifiable> objects)
        {
            this.clientProxy.SendAsync("ObjectsOutOfScope", objects.Select(o => o.Id));
        }

        /// <inheritdoc/>
        public void NewPlayersInScope(IEnumerable<Player> newObjects)
        {
            this.clientProxy.SendAsync("NewPlayersInScope", newObjects.Select(o => new
            {
                id = o.Id,
                name = o.Name,
                x = o.X,
                y = o.Y,
                rotation = o.Rotation,
                serverId = this.ServerId,
                mapId = this.MapId
            }));
        }

        /// <inheritdoc/>
        public void NewNpcsInScope(IEnumerable<NonPlayerCharacter> newObjects)
        {
            this.clientProxy.SendAsync("NewNPCsInScope", newObjects.Select(o => new { id = o.Id, name = o.Definition.Designation, x = o.X, y = o.Y, rotation = o.Rotation, serverId = this.ServerId, mapId = this.MapId, isMonster = o is Monster }));
        }

        /// <inheritdoc/>
        public void UpdateRotation()
        {
            throw new NotImplementedException("WorldObserver does not have an own rotation.");
        }

        /// <inheritdoc/>
        public void ShowSkillAnimation(Player attackingPlayer, IAttackable target, Skill skill)
        {
            this.clientProxy.SendAsync("ShowSkillAnimation", attackingPlayer, target, skill);
        }

        /// <inheritdoc/>
        public void ShowAreaSkillAnimation(Player player, Skill skill, byte x, byte y, byte rotation)
        {
            this.clientProxy.SendAsync("ShowAreaSkillAnimation", player.Id, skill.SkillID, x, y, rotation);
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
