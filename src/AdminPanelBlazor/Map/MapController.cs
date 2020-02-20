// <copyright file="MapController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanelBlazor.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.JSInterop;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Controller which contains the logic for the shown game map.
    /// TODO: Split class, it does too much.
    /// </summary>
    public sealed class MapController : IMapController, IWorldObserver, IObjectGotKilledPlugIn, IObjectMovedPlugIn, IShowAnimationPlugIn, IObjectsOutOfScopePlugIn, INewPlayersInScopePlugIn, INewNpcsInScopePlugIn, IShowSkillAnimationPlugIn, IShowAreaSkillAnimationPlugIn, ILocateable, IBucketMapObserver, ISupportIdUpdate
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string identifier;
        private readonly IGameServer gameServer;
        private readonly int mapNumber;
        private readonly IJSRuntime jsRuntime;
        private readonly string worldAccessor;
        private readonly ObserverToWorldViewAdapter adapterToWorldView;
        private readonly CancellationTokenSource disposeCts = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="MapController" /> class.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="mapIdentifier">The map identifier.</param>
        /// <param name="gameServer">The game server.</param>
        /// <param name="mapNumber">The map number.</param>
        public MapController(IJSRuntime jsRuntime, string mapIdentifier, IGameServer gameServer, int mapNumber)
        {
            this.jsRuntime = jsRuntime;
            this.worldAccessor = $"{mapIdentifier}.world";
            this.identifier = mapIdentifier;
            this.gameServer = gameServer;
            this.mapNumber = mapNumber;
            this.adapterToWorldView = new ObserverToWorldViewAdapter(this, byte.MaxValue);
            this.ViewPlugIns = new ViewContainer(this);
        }

        /// <inheritdoc />
        public event EventHandler ObjectsChanged;

        /// <inheritdoc/>
        public int InfoRange => byte.MaxValue;

        /// <inheritdoc/>
        public GameMap CurrentMap => null;

        /// <inheritdoc/>
        public Point Position { get; set; }

        /// <inheritdoc cref="ISupportIdUpdate" />
        public ushort Id { get; set; }

        /// <inheritdoc/>
        public IList<Bucket<ILocateable>> ObservingBuckets => this.adapterToWorldView.ObservingBuckets;

        /// <inheritdoc/>
        public ICustomPlugInContainer<IViewPlugIn> ViewPlugIns { get; }

        /// <inheritdoc />
        public IDictionary<int, ILocateable> Objects { get; } = new Dictionary<int, ILocateable>();

        /// <inheritdoc />
        public async void NewNpcsInScope(IEnumerable<NonPlayerCharacter> newObjects)
        {
            try
            {
                foreach (var npc in newObjects)
                {
                    this.Objects.TryAdd(npc.Id, npc);

                    if (this.disposeCts.IsCancellationRequested)
                    {
                        return;
                    }

                    await this.jsRuntime.InvokeVoidAsync($"{this.worldAccessor}.addOrUpdateNpc", this.disposeCts.Token, CreateMapObject(npc));
                }

                this.ObjectsChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (TaskCanceledException)
            {
                // don't need to handle that.
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(this.NewNpcsInScope)}", e);
            }
        }

        /// <inheritdoc />
        public async void NewPlayersInScope(IEnumerable<Player> newObjects)
        {
            try
            {
                foreach (var player in newObjects)
                {
                    this.Objects.TryAdd(player.Id, player);

                    if (this.disposeCts.IsCancellationRequested)
                    {
                        return;
                    }

                    await this.jsRuntime.InvokeVoidAsync($"{this.worldAccessor}.addOrUpdatePlayer", this.disposeCts.Token, CreateMapObject(player));
                }

                this.ObjectsChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (TaskCanceledException)
            {
                // don't need to handle that.
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(this.NewPlayersInScope)}", e);
            }
        }

        /// <inheritdoc />
        public async void ObjectsOutOfScope(IEnumerable<IIdentifiable> objects)
        {
            try
            {
                foreach (var obj in objects)
                {
                    this.Objects.Remove(obj.Id);

                    if (this.disposeCts.IsCancellationRequested)
                    {
                        return;
                    }

                    await this.jsRuntime.InvokeVoidAsync($"{this.worldAccessor}.removeObject", this.disposeCts.Token, obj.Id);
                }

                this.ObjectsChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (TaskCanceledException)
            {
                // don't need to handle that.
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(this.ObjectsOutOfScope)}; objects: {string.Join(';', objects)}", e);
            }
        }

        /// <inheritdoc />
        public async void ObjectGotKilled(IAttackable killedObject, IAttackable killerObject)
        {
            try
            {
                await this.jsRuntime.InvokeVoidAsync($"{this.worldAccessor}.killObject", this.disposeCts.Token, killedObject.Id, killerObject.Id);
            }
            catch (TaskCanceledException)
            {
                // don't need to handle that.
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(this.ObjectGotKilled)}; killedObject: {killedObject}, killerObject: {killerObject}", e);
            }
        }

        /// <inheritdoc />
        public async void ObjectMoved(ILocateable movedObject, MoveType moveType)
        {
            try
            {
                await this.ObjectMovedAsync(movedObject, moveType).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // don't need to handle that.
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(this.ObjectMoved)}; movedObject: {movedObject}, moveType: {moveType}", e);
            }
        }

        /// <inheritdoc />
        public async void ShowSkillAnimation(Player attackingPlayer, IAttackable target, Skill skill)
        {
            try
            {
                await this.jsRuntime.InvokeVoidAsync($"{this.worldAccessor}.addSkillAnimation", this.disposeCts.Token, attackingPlayer.Id, target?.Id, skill.Number);
            }
            catch (TaskCanceledException)
            {
                // don't need to handle that.
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(this.ShowSkillAnimation)}", e);
            }
        }

        /// <inheritdoc />
        public async void ShowAreaSkillAnimation(Player player, Skill skill, Point point, byte rotation)
        {
            try
            {
                await this.jsRuntime.InvokeVoidAsync($"{this.worldAccessor}.addAreaSkillAnimation", this.disposeCts.Token, player.Id, skill.Number, point.X, point.Y, rotation);
            }
            catch (TaskCanceledException)
            {
                // don't need to handle that.
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(this.ShowAreaSkillAnimation)}", e);
            }
        }

        /// <inheritdoc />
        public async void ShowAnimation(IIdentifiable animatingObj, byte animation, IIdentifiable targetObj, Direction direction)
        {
            try
            {
                await this.jsRuntime.InvokeVoidAsync($"{this.worldAccessor}.addAnimation", this.disposeCts.Token, animatingObj.Id, animation, targetObj?.Id, direction);
            }
            catch (TaskCanceledException)
            {
                // don't need to handle that.
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(this.ShowAnimation)}", e);
            }
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

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            this.disposeCts.Cancel();
            this.gameServer.UnregisterMapObserver((ushort)this.mapNumber, this.Id);
            this.adapterToWorldView.Dispose();
            try
            {
                await this.jsRuntime.InvokeVoidAsync("DisposeMap", this.identifier);
            }
            finally
            {
                this.disposeCts.Dispose();
            }
        }

        private static MapObject CreateMapObject(ILocateable locateable)
        {
            return new MapObject
            {
                Direction = (locateable as IRotatable)?.Rotation ?? default,
                Id = locateable.Id,
                MapId = locateable.CurrentMap.MapId,
                Name = locateable.ToString(),
                X = locateable.Position.X,
                Y = locateable.Position.Y,
            };
        }

        private async Task ObjectMovedAsync(ILocateable movedObject, MoveType moveType)
        {
            Point targetPoint = movedObject.Position;
            object steps = null;
            int walkDelay = 0;
            if (movedObject is ISupportWalk walker && moveType == MoveType.Walk)
            {
                targetPoint = walker.WalkTarget;
                walkDelay = (int)walker.StepDelay.TotalMilliseconds;
                var walkingSteps = new WalkingStep[16];
                var stepCount = walker.GetSteps(walkingSteps);
                var walkSteps = walkingSteps.AsSpan().Slice(0, stepCount).ToArray()
                    .Select(step => new { x = step.To.X, y = step.To.Y, direction = step.Direction }).ToList();

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

            await this.jsRuntime.InvokeVoidAsync($"{this.worldAccessor}.objectMoved", this.disposeCts.Token, movedObject.Id, targetPoint.X, targetPoint.Y, moveType, walkDelay, steps);
        }

        private class ViewContainer : ICustomPlugInContainer<IViewPlugIn>
        {
            private readonly MapController adapter;

            public ViewContainer(MapController adapter)
            {
                this.adapter = adapter;
            }

            public T GetPlugIn<T>()
                where T : class, IViewPlugIn
            {
                if (this.adapter is T t)
                {
                    return t;
                }

                return default;
            }
        }
    }
}