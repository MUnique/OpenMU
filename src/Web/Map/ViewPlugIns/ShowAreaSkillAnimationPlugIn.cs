// <copyright file="ShowAreaSkillAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.ViewPlugIns;

using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Implementation of <see cref="IShowAreaSkillAnimationPlugIn"/> which uses the javascript map app.
/// </summary>
public class ShowAreaSkillAnimationPlugIn : JsViewPlugInBase, IShowAreaSkillAnimationPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAreaSkillAnimationPlugIn"/> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="worldAccessor">The world accessor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public ShowAreaSkillAnimationPlugIn(IJSRuntime jsRuntime, ILoggerFactory loggerFactory, string worldAccessor, CancellationToken cancellationToken)
        : base(jsRuntime, loggerFactory, $"{worldAccessor}.addAreaSkillAnimation", cancellationToken)
    {
    }

    /// <inheritdoc />
    public async ValueTask ShowAreaSkillAnimationAsync(Player player, Skill skill, Point point, byte rotation)
    {
        await this.InvokeAsync(player.Id, skill.Number, point.X, point.Y, rotation).ConfigureAwait(false);
    }
}