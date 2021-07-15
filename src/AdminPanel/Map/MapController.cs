// <copyright file="MapController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.JSInterop;
    using MUnique.OpenMU.AdminPanel.Map.ViewPlugIns;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Controller which contains the logic for the shown game map.
    /// </summary>
    public sealed class MapController : IMapController, IWorldObserver, ILocateable, IBucketMapObserver, ISupportIdUpdate
    {
        private readonly string identifier;
        private readonly IGameServer gameServer;
        private readonly int mapNumber;
        private readonly IJSRuntime jsRuntime;
        private readonly ObserverToWorldViewAdapter adapterToWorldView;
        private readonly CancellationTokenSource disposeCts = new ();

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

            this.identifier = mapIdentifier;
            this.gameServer = gameServer;
            this.mapNumber = mapNumber;
            this.adapterToWorldView = new ObserverToWorldViewAdapter(this, byte.MaxValue);

            var viewPlugIns = new CustomPlugInContainer<IViewPlugIn>();
            var worldAccessor = $"{mapIdentifier}.world";
            viewPlugIns.RegisterPlugIn<IObjectGotKilledPlugIn>(new ObjectGotKilledPlugIn(this.jsRuntime, worldAccessor, this.disposeCts.Token));
            viewPlugIns.RegisterPlugIn<IObjectMovedPlugIn>(new ObjectMovedPlugIn(this.jsRuntime, worldAccessor, this.disposeCts.Token));
            viewPlugIns.RegisterPlugIn<IShowAnimationPlugIn>(new ShowAnimationPlugIn(this.jsRuntime, worldAccessor, this.disposeCts.Token));
            viewPlugIns.RegisterPlugIn<IObjectsOutOfScopePlugIn>(new ObjectsOutOfScopePlugIn(this.jsRuntime, worldAccessor, this.disposeCts.Token, this.Objects, () => this.ObjectsChanged?.Invoke(this, EventArgs.Empty)));
            viewPlugIns.RegisterPlugIn<INewPlayersInScopePlugIn>(new NewPlayersInScopePlugIn(this.jsRuntime, worldAccessor, this.disposeCts.Token, this.Objects, () => this.ObjectsChanged?.Invoke(this, EventArgs.Empty)));
            viewPlugIns.RegisterPlugIn<INewNpcsInScopePlugIn>(new NewNpcsInScopePlugIn(this.jsRuntime, worldAccessor, this.disposeCts.Token, this.Objects, () => this.ObjectsChanged?.Invoke(this, EventArgs.Empty)));
            viewPlugIns.RegisterPlugIn<IShowSkillAnimationPlugIn>(new ShowSkillAnimationPlugIn(this.jsRuntime, worldAccessor, this.disposeCts.Token));
            viewPlugIns.RegisterPlugIn<IShowAreaSkillAnimationPlugIn>(new ShowAreaSkillAnimationPlugIn(this.jsRuntime, worldAccessor, this.disposeCts.Token));

            this.ViewPlugIns = viewPlugIns;
        }

        /// <inheritdoc />
        public event EventHandler? ObjectsChanged;

        /// <inheritdoc/>
        public int InfoRange => byte.MaxValue;

        /// <inheritdoc/>
        public GameMap? CurrentMap => null;

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

        /// <inheritdoc/>
        public void LocateableAdded(object? sender, BucketItemEventArgs<ILocateable> eventArgs)
        {
            this.adapterToWorldView.LocateableAdded(sender, eventArgs);
        }

        /// <inheritdoc/>
        public void LocateableRemoved(object? sender, BucketItemEventArgs<ILocateable> eventArgs)
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
    }
}