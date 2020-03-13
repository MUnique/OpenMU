// <copyright file="ObjectGotKilledPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map.ViewPlugIns
{
    using System.Threading;
    using Microsoft.JSInterop;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.World;

    /// <summary>
    /// Implementation of <see cref="IObjectGotKilledPlugIn"/> which uses the javascript map app.
    /// </summary>
    public class ObjectGotKilledPlugIn : JsViewPlugInBase, IObjectGotKilledPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectGotKilledPlugIn"/> class.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="worldAccessor">The world accessor.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ObjectGotKilledPlugIn(IJSRuntime jsRuntime, string worldAccessor, CancellationToken cancellationToken)
            : base(jsRuntime, $"{worldAccessor}.killObject", cancellationToken)
        {
        }

        /// <inheritdoc />
        public async void ObjectGotKilled(IAttackable killedObject, IAttackable killerObject)
        {
            await this.InvokeAsync(killedObject.Id, killerObject.Id);
        }
    }
}