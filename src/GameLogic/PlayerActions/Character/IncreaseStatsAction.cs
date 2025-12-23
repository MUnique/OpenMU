// <copyright file="IncreaseStatsAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to increase stat attributes.
/// </summary>
public class IncreaseStatsAction
{
    /// <summary>
    /// Increases the specified stat attribute by one point, if enough points are available.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="targetAttribute">The stat attribute definition.</param>
    /// <param name="amount">The amount of points.</param>
    public async ValueTask IncreaseStatsAsync(Player player, AttributeDefinition targetAttribute, ushort amount = 1)
    {
        if (player.SelectedCharacter is not { } selectedCharacter)
        {
            throw new InvalidOperationException("No character selected");
        }

        if (amount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "The amount must be greater than 0.");
        }

        if (!selectedCharacter.CanIncreaseStats(amount))
        {
            var message = player.GetLocalizedMessage("Stats_Message_NotEnoughPoints", "You don't have enough level-up points available.");
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        var attributeDef = selectedCharacter.CharacterClass?.GetStatAttribute(targetAttribute);
        if (attributeDef is { IncreasableByPlayer: true })
        {
            if (attributeDef.Attribute?.MaximumValue is { } maximumValue
                && player.Attributes![attributeDef.Attribute] is { } current
                && current + amount > maximumValue)
            {
                amount = (ushort)(maximumValue - current);
                if (amount == 0)
                {
                    var message = player.GetLocalizedMessage("Stats_Message_MaximumReached", "Maximum of {0} {1} reached.", attributeDef.Attribute?.MaximumValue ?? 0, attributeDef.Attribute?.Designation ?? string.Empty);
                    await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
                    return;
                }
            }

            player.Attributes![attributeDef.Attribute] += amount;
            selectedCharacter.LevelUpPoints -= Math.Min(selectedCharacter.LevelUpPoints, amount);

            await player.InvokeViewPlugInAsync<IStatIncreaseResultPlugIn>(p => p.StatIncreaseResultAsync(targetAttribute, amount)).ConfigureAwait(false);
        }
        else
        {
            var message = player.GetLocalizedMessage("Stats_Message_AttributeUnavailable", "Attribute not available.");
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
        }
    }
}
