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
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Not enough level up points available.", MessageType.BlueNormal)).ConfigureAwait(false);
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
                    await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync($"Maximum of {attributeDef.Attribute?.MaximumValue} {attributeDef.Attribute?.Designation} has been reached.", MessageType.BlueNormal)).ConfigureAwait(false);
                    return;
                }
            }

            player.Attributes![attributeDef.Attribute] += amount;
            selectedCharacter.LevelUpPoints -= Math.Min(selectedCharacter.LevelUpPoints, amount);

            await player.InvokeViewPlugInAsync<IStatIncreaseResultPlugIn>(p => p.StatIncreaseResultAsync(targetAttribute, amount)).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Attribute not available.", MessageType.BlueNormal)).ConfigureAwait(false);
        }
    }
}