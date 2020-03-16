// <copyright file="NewNpcsInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map.ViewPlugIns
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.JSInterop;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views.World;

    /// <summary>
    /// Implementation of <see cref="INewNpcsInScopePlugIn"/> which uses the javascript map app.
    /// </summary>
    public class NewNpcsInScopePlugIn : JsViewPlugInBase, INewNpcsInScopePlugIn
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDictionary<int, ILocateable> objects;
        private readonly Action objectsChangedCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewNpcsInScopePlugIn" /> class.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="worldAccessor">The world accessor.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="objects">The objects.</param>
        /// <param name="objectsChangedCallback">The objects changed callback.</param>
        public NewNpcsInScopePlugIn(IJSRuntime jsRuntime, string worldAccessor, CancellationToken cancellationToken, IDictionary<int, ILocateable> objects, Action objectsChangedCallback)
            : base(jsRuntime, $"{worldAccessor}.addOrUpdateNpc", cancellationToken)
        {
            this.objects = objects;
            this.objectsChangedCallback = objectsChangedCallback;
        }

        /// <inheritdoc />
        public async void NewNpcsInScope(IEnumerable<NonPlayerCharacter> newObjects)
        {
            try
            {
                foreach (var npc in newObjects)
                {
                    this.objects.TryAdd(npc.Id, npc);

                    if (this.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    await this.InvokeAsync(npc.CreateMapObject());
                }

                this.objectsChangedCallback?.Invoke();
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
    }
}