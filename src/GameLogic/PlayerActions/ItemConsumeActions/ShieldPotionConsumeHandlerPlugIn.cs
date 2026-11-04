// -----------------------------------------------------------------------
// <copyright file="ShieldPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Consume handler for shield potions, which recover the <see cref="Stats.CurrentShield"/>.
/// </summary>
public abstract class ShieldPotionConsumeHandlerPlugIn : RecoverConsumeHandlerPlugIn, IItemConsumeHandlerPlugIn
{
    /// <inheritdoc/>
    protected override AttributeDefinition MaximumAttribute => Stats.MaximumShield;

    /// <inheritdoc/>
    protected override AttributeDefinition CurrentAttribute => Stats.CurrentShield;

    /// <summary>
    /// Gets the recover percentage.
    /// </summary>
    protected abstract double RecoverPercentage { get; }

    /// <inheritdoc />
    public sealed override object CreateDefaultConfig()
    {
        return new RecoverConsumeHandlerConfiguration
        {
            TotalRecoverPercentage = this.RecoverPercentage,
            RecoverDelayReductionByPotionLevel = 1.0 / 16.0,
            RecoverPercentageIncreaseByPotionLevel = 1,
            CooldownTime = TimeSpan.FromSeconds(0.5),
            RecoverSteps =
            {
                new RecoverStep
                {
                    Delay = TimeSpan.FromMilliseconds(200),
                    RecoverPercentage = 20,
                },
                new RecoverStep
                {
                    Delay = TimeSpan.FromMilliseconds(600),
                    RecoverPercentage = 60,
                },
                new RecoverStep
                {
                    Delay = TimeSpan.FromMilliseconds(200),
                    RecoverPercentage = 20,
                },
            },
        };
    }
}