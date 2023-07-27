// <copyright file="ObjectGotKilledPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.ViewPlugIns;

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
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="worldAccessor">The world accessor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public ObjectGotKilledPlugIn(IJSRuntime jsRuntime, ILoggerFactory loggerFactory, string worldAccessor, CancellationToken cancellationToken)
        : base(jsRuntime, loggerFactory, $"{worldAccessor}.killObject", cancellationToken)
    {
    }

    /// <inheritdoc />
    public async ValueTask ObjectGotKilledAsync(IAttackable killedObject, IAttacker? killerObject)
    {
        // todo: maybe add the skill which led to the death, for special effects
        await this.InvokeAsync(killedObject.Id, killerObject?.Id ?? 0).ConfigureAwait(false);
    }
}