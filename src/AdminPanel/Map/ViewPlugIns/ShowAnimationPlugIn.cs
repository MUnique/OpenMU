// <copyright file="ShowAnimationPlugIn.cs" company="MUnique">
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

    /// <summary>
    /// Implementation of <see cref="IShowAnimationPlugIn"/> which uses the javascript map app.
    /// </summary>
    public class ShowAnimationPlugIn : JsViewPlugInBase, IShowAnimationPlugIn
    {
        private const byte MonsterAttackAnimation = 0x78;

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowAnimationPlugIn"/> class.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="worldAccessor">The world accessor.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ShowAnimationPlugIn(IJSRuntime jsRuntime, string worldAccessor, CancellationToken cancellationToken)
            : base(jsRuntime, $"{worldAccessor}.addAnimation", cancellationToken)
        {
        }

        /// <inheritdoc />
        public async void ShowAnimation(IIdentifiable animatingObj, byte animation, IIdentifiable? targetObj, Direction direction)
        {
            await this.InvokeAsync(animatingObj.Id, animation, targetObj?.Id ?? 0, direction);
        }

        /// <inheritdoc />
        public async void ShowMonsterAttackAnimation(IIdentifiable animatingObj, IIdentifiable? targetObj, Direction direction)
        {
            await this.InvokeAsync(animatingObj.Id, MonsterAttackAnimation, targetObj?.Id ?? 0, direction);
        }
    }
}