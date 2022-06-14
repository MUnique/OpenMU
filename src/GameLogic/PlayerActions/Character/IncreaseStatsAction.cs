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
    public void IncreaseStats(Player player, AttributeDefinition statAttributeDefinition)
    {
        if (player.SelectedCharacter is null)
        {
            throw new InvalidOperationException("No character selected");
        }

        var selectedCharacter = player.SelectedCharacter;
        if (!selectedCharacter.CanIncreaseStats())
        {
            this.PublishIncreaseResult(player, statAttributeDefinition, false);
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

            this.PublishIncreaseResult(player, statAttributeDefinition, true);
            return;
        }

        this.PublishIncreaseResult(player, statAttributeDefinition, false);
    }

    private void PublishIncreaseResult(Player player, AttributeDefinition statAttributeDefinition, bool success)
    {
        player.ViewPlugIns.GetPlugIn<IStatIncreaseResultPlugIn>()?.StatIncreaseResult(statAttributeDefinition, success);
    }
}