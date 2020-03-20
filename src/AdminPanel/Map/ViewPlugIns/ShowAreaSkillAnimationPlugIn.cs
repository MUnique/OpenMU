// <copyright file="ShowAreaSkillAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map.ViewPlugIns
{
    using System.Reflection;
    using System.Threading;
    using log4net;
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
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowAreaSkillAnimationPlugIn"/> class.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="worldAccessor">The world accessor.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ShowAreaSkillAnimationPlugIn(IJSRuntime jsRuntime, string worldAccessor, CancellationToken cancellationToken)
            : base(jsRuntime, $"{worldAccessor}.addAreaSkillAnimation", cancellationToken)
        {
        }

        /// <inheritdoc />
        public async void ShowAreaSkillAnimation(Player player, Skill skill, Point point, byte rotation)
        {
            await this.InvokeAsync(player.Id, skill.Number, point.X, point.Y, rotation);
        }
    }
}