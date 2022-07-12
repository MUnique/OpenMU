// <copyright file="IncreaseStatsAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// Action to increase stat attributes.
/// </summary>
public class IncreaseStatsAction
{
    /// <summary>
    /// Increases the specified stat attribute by one point, if enough points are available.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="statAttributeDefinition">The stat attribute definition.</param>
    public async ValueTask IncreaseStatsAsync(Player player, AttributeDefinition statAttributeDefinition)
    {
        if (player.SelectedCharacter is null)
        {
            throw new InvalidOperationException("No character selected");
        }

        var selectedCharacter = player.SelectedCharacter;
        if (!selectedCharacter.CanIncreaseStats())
        {
            await this.PublishIncreaseResult(player, statAttributeDefinition, false);
            return;
        }

        var attributeDef = selectedCharacter.CharacterClass?.GetStatAttribute(statAttributeDefinition);
        if (attributeDef is { IncreasableByPlayer: true })
        {
            player.Attributes![attributeDef.Attribute]++;
            if (selectedCharacter.LevelUpPoints > 0)
            {
                selectedCharacter.LevelUpPoints--;
            }

            await this.PublishIncreaseResult(player, statAttributeDefinition, true);
            return;
        }

        await this.PublishIncreaseResult(player, statAttributeDefinition, false);
    }

    private ValueTask PublishIncreaseResult(Player player, AttributeDefinition statAttributeDefinition, bool success)
    {
        return player.InvokeViewPlugInAsync<IStatIncreaseResultPlugIn>(p => p.StatIncreaseResultAsync(statAttributeDefinition, success));
    }
}