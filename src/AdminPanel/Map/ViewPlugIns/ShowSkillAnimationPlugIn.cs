// <copyright file="ShowSkillAnimationPlugIn.cs" company="MUnique">
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
    /// Implementation of <see cref="IShowSkillAnimationPlugIn"/> which uses the javascript map app.
    /// </summary>
    public class ShowSkillAnimationPlugIn : JsViewPlugInBase, IShowSkillAnimationPlugIn
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowSkillAnimationPlugIn"/> class.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="worldAccessor">The world accessor.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ShowSkillAnimationPlugIn(IJSRuntime jsRuntime, string worldAccessor, CancellationToken cancellationToken)
            : base(jsRuntime, $"{worldAccessor}.addSkillAnimation", cancellationToken)
        {
        }

        /// <inheritdoc />
        public void ShowSkillAnimation(IAttacker attacker, IAttackable? target, Skill skill, bool effectApplied)
        {
            this.ShowSkillAnimation(attacker, target, skill.Number, effectApplied);
        }

        /// <inheritdoc />
        public async void ShowSkillAnimation(IAttacker attacker, IAttackable? target, short skillNumber, bool effectApplied)
        {
            await this.InvokeAsync(attacker.Id, target?.Id ?? 0, skillNumber);
        }
    }
}