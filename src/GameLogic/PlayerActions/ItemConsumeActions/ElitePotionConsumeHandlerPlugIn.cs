// <copyright file="ElitePotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// The base class of the consume handlers for the elite potions of the in-game shop.
/// </summary>
/// <remarks>
/// Unlike the usual potions, the elite potions recover a percentage of the maximum value
/// immediately, without steps, and independent of the character level, as the client describes them
/// (e.g. "Restores HP by 100% immediately.").
/// </remarks>
public abstract class ElitePotionConsumeHandlerPlugIn : RecoverConsumeHandlerPlugIn
{
    /// <summary>
    /// Gets the default percentage of the maximum value which is recovered.
    /// </summary>
    protected abstract double RecoverPercentage { get; }

    /// <inheritdoc />
    public sealed override object CreateDefaultConfig()
    {
        return new RecoverConsumeHandlerConfiguration
        {
            TotalRecoverPercentage = this.RecoverPercentage,
            CooldownTime = TimeSpan.FromSeconds(0.5),
        };
    }
}
