// <copyright file="WorldObserverHub.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNet.SignalR;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A hub with which someone can observe whole <see cref="GameMap"/>s.
    /// </summary>
    public class WorldObserverHub : Hub
    {
        private static readonly IDictionary<string, WorldObserverToHubAdapter> Observers = new Dictionary<string, WorldObserverToHubAdapter>();

        private static readonly ConcurrentQueue<ushort> FreeIds = new ConcurrentQueue<ushort>(Enumerable.Range(ushort.MaxValue - 0x100, 0xFF).Select(id => (ushort)id));

        private readonly IList<IManageableServer> servers;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldObserverHub"/> class.
        /// </summary>
        public WorldObserverHub()
        {
            this.servers = Servers;
        }

        /// <summary>
        /// Gets or sets the game servers which should be available to the hub.
        /// </summary>
        /// <remarks>Not nice, maybe think of another solution passing the game servers to the hub.</remarks>
        public static IList<IManageableServer> Servers { get; set; }

        /// <summary>
        /// Registers the connected client to listen on the events on the specified map. Is called from the client.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="mapId">The map identifier.</param>
        /// <remarks>
        /// This is a bit dirty... the AdminPanel should not have the reference to  GameLogic.
        /// Instead we need to extract some interfaces.
        /// </remarks>
        public void Listen(byte serverId, ushort mapId)
        {
            IGameServer gameServer = this.servers.OfType<IGameServer>().FirstOrDefault(g => g.Id == serverId);
            if (gameServer == null)
            {
                throw new ArgumentException($"unknown server id {serverId}", nameof(serverId));
            }

            if (!FreeIds.TryDequeue(out ushort observerKey))
            {
                throw new Exception("no free observer keys available");
            }

            WorldObserverToHubAdapter observer = new WorldObserverToHubAdapter(observerKey, serverId, mapId, this, this.Context.ConnectionId);
            Observers.Add(this.Context.ConnectionId, observer);

            try
            {
                gameServer.RegisterMapObserver(mapId, observer);
            }
            catch (ArgumentException)
            {
                Observers.Remove(this.Context.ConnectionId);
                observer.Dispose();
                throw;
            }
        }

        /// <inheritdoc/>
        public override Task OnDisconnected(bool stopCalled)
        {
            if (Observers.TryGetValue(this.Context.ConnectionId, out WorldObserverToHubAdapter observer))
            {
                Observers.Remove(this.Context.ConnectionId);
                var gameServer = this.servers.OfType<IGameServer>().First(g => g.Id == observer.ServerId);
                gameServer.UnregisterMapObserver(observer.MapId, observer.Id);
                FreeIds.Enqueue(observer.Id);
                observer.Dispose();
            }

            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// The item drops disappeared from the ground.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="disappearedItemIds">The ids of the disappeared items.</param>
        public void DroppedItemsDisappeared(WorldObserverToHubAdapter sender, IEnumerable<ushort> disappearedItemIds)
        {
            this.Clients.Client(sender.ConnectionId).DroppedItemsDisappeared(disappearedItemIds);
        }

        /// <summary>
        /// Shows the new npcs in scope.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newObjects">The new objects.</param>
        public void NewNpcsInScope(WorldObserverToHubAdapter sender, IEnumerable<NonPlayerCharacter> newObjects)
        {
            this.Clients.Client(sender.ConnectionId).NewNPCsInScope(newObjects.Select(o => new { Id = o.Id, Name = o.Definition.Designation, X = o.X, Y = o.Y, Rotation = o.Rotation, IsMonster = o is Monster }));
        }

        /// <summary>
        /// Shows the new players in scope.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newObjects">The new objects.</param>
        public void NewPlayersInScope(WorldObserverToHubAdapter sender, IEnumerable<Player> newObjects)
        {
            this.Clients.Client(sender.ConnectionId).NewPlayersInScope(newObjects.Select(o => new
            {
                Id = o.Id, Name = o.Name, X = o.X, Y = o.Y, Rotation = o.Rotation, ServerId = sender.ServerId
            }));
        }

        /// <summary>
        /// An object got killed by another object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="killedObject">The killed object.</param>
        /// <param name="killerObject">The object which killed the object.</param>
        public void ObjectGotKilled(WorldObserverToHubAdapter sender, IAttackable killedObject, IAttackable killerObject)
        {
            this.Clients.Client(sender.ConnectionId).ObjectGotKilled(killedObject.Id, killerObject.Id);
        }

        /// <summary>
        /// An object moved on the map.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="movedObject">The moved object.</param>
        /// <param name="moveType">Type of the move.</param>
        public void ObjectMoved(WorldObserverToHubAdapter sender, ILocateable movedObject, MoveType moveType)
        {
            byte x, y;
            object steps = null;
            int walkDelay = 0;
            if (movedObject is ISupportWalk walkable && moveType == MoveType.Walk)
            {
                x = walkable.WalkTarget.X;
                y = walkable.WalkTarget.Y;
                walkDelay = (int)walkable.StepDelay.TotalMilliseconds;
                var walkSteps = walkable.NextDirections.Select(step => new { step.To.X, step.To.Y, step.Direction }).ToList();

                // TODO: Can errors happen here when NextDirection changes in the meantime?
                var lastStep = walkable.NextDirections.LastOrDefault();
                if (!Equals(lastStep, default(WalkingStep)))
                {
                    var lastDirection = lastStep.To.GetDirectionTo(walkable.WalkTarget);
                    if (lastDirection != Direction.Undefined)
                    {
                        walkSteps.Add(new { X = x, Y = y, Direction = lastDirection });
                    }
                }

                steps = walkSteps;
            }
            else
            {
                x = movedObject.X;
                y = movedObject.Y;
            }

            this.Clients.Client(sender.ConnectionId).ObjectMoved(movedObject.Id, x, y, moveType, walkDelay, steps);
        }

        /// <summary>
        /// Objects are out of scope.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="objects">The objects.</param>
        public void ObjectsOutOfScope(WorldObserverToHubAdapter sender, IEnumerable<IIdentifiable> objects)
        {
            this.Clients.Client(sender.ConnectionId).ObjectsOutOfScope(objects.Select(o => o.Id));
        }

        /// <summary>
        /// Shows the animation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="animatingObj">The animating object.</param>
        /// <param name="animation">The animation.</param>
        /// <param name="targetObj">The target object.</param>
        /// <param name="direction">The direction.</param>
        public void ShowAnimation(WorldObserverToHubAdapter sender, IIdentifiable animatingObj, byte animation, IIdentifiable targetObj, byte direction)
        {
            this.Clients.Client(sender.ConnectionId).ShowAnimation(animatingObj.Id, animation, targetObj?.Id, direction);
        }

        /// <summary>
        /// Shows the area skill animation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="player">The player.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="rotation">The rotation.</param>
        public void ShowAreaSkillAnimation(WorldObserverToHubAdapter sender, Player player, Skill skill, byte x, byte y, byte rotation)
        {
            this.Clients.Client(sender.ConnectionId).ShowAreaSkillAnimation(player.Id, skill.SkillID, x, y, rotation);
        }

        /// <summary>
        /// Shows the dropped items.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="droppedItems">The dropped items.</param>
        /// <param name="freshDrops">if set to <c>true</c> this items are fresh drops; Otherwise they are already laying on the ground when reaching a newly discovered part of the map.</param>
        public void ShowDroppedItems(WorldObserverToHubAdapter sender, IEnumerable<DroppedItem> droppedItems, bool freshDrops)
        {
            this.Clients.Client(sender.ConnectionId).ShowDroppedItems(droppedItems.Select(drop => new { Id = drop.Id, X = drop.X, Y = drop.Y, ItemName = drop.Item.ToString() }), freshDrops);
        }

        /// <summary>
        /// Shows the skill animation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="attackingPlayer">The attacking player.</param>
        /// <param name="target">The target.</param>
        /// <param name="skill">The skill.</param>
        public void ShowSkillAnimation(WorldObserverToHubAdapter sender, Player attackingPlayer, IAttackable target, Skill skill)
        {
            this.Clients.Client(sender.ConnectionId).ShowSkillAnimation(attackingPlayer, target, skill);
        }
    }
}
