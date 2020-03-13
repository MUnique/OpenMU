// <copyright file="NewPlayersInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map.ViewPlugIns
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using log4net;
    using Microsoft.JSInterop;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.World;

    /// <summary>
    /// Implementation of <see cref="INewPlayersInScopePlugIn"/> which uses the javascript map app.
    /// </summary>
    public class NewPlayersInScopePlugIn : JsViewPlugInBase, INewPlayersInScopePlugIn
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDictionary<int, ILocateable> objects;
        private readonly Action objectsChangedCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewPlayersInScopePlugIn" /> class.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="worldAccessor">The world accessor.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="objects">The objects.</param>
        /// <param name="objectsChangedCallback">The objects changed callback.</param>
        public NewPlayersInScopePlugIn(IJSRuntime jsRuntime, string worldAccessor, CancellationToken cancellationToken, IDictionary<int, ILocateable> objects, Action objectsChangedCallback)
            : base(jsRuntime, $"{worldAccessor}.addOrUpdatePlayer", cancellationToken)
        {
            this.objects = objects;
            this.objectsChangedCallback = objectsChangedCallback;
        }

        /// <inheritdoc />
        public async void NewPlayersInScope(IEnumerable<Player> newObjects)
        {
            try
            {
                foreach (var player in newObjects)
                {
                    this.objects.TryAdd(player.Id, player);

                    if (this.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    await this.InvokeAsync(player.CreateMapObject());
                }

                this.objectsChangedCallback?.Invoke();
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(this.NewPlayersInScope)}", e);
            }
        }
    }
}