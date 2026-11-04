// <copyright file="ShowSkillAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.ViewPlugIns;

using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Implementation of <see cref="IShowSkillAnimationPlugIn"/> which uses the javascript map app.
/// </summary>
public class ShowSkillAnimationPlugIn : JsViewPlugInBase, IShowSkillAnimationPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShowSkillAnimationPlugIn"/> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="worldAccessor">The world accessor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public ShowSkillAnimationPlugIn(IJSRuntime jsRuntime, ILoggerFactory loggerFactory, string worldAccessor, CancellationToken cancellationToken)
        : base(jsRuntime, loggerFactory, $"{worldAccessor}.addSkillAnimation", cancellationToken)
    {
    }

    /// <inheritdoc />
    public ValueTask ShowSkillAnimationAsync(IAttacker attacker, IAttackable? target, Skill skill, bool effectApplied)
    {
        return this.ShowSkillAnimationAsync(attacker, target, skill.Number, effectApplied);
    }

    /// <inheritdoc />
    public async ValueTask ShowSkillAnimationAsync(IAttacker attacker, IAttackable? target, short skillNumber, bool effectApplied)
    {
        await this.InvokeAsync(attacker.Id, target?.Id ?? 0, skillNumber).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowComboAnimationAsync(IAttacker attacker, IAttackable? target)
    {
        await this.InvokeAsync(attacker.Id, target?.Id ?? 0, 59).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowNovaStartAsync(IAttacker attacker)
    {
        await this.InvokeAsync(attacker.Id, 0, 58).ConfigureAwait(false);
    }
}