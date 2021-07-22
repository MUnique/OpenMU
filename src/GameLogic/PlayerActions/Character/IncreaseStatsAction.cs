// <copyright file="IncreaseStatsAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character
{
    using System;
    using System.Linq;
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

            if (player.SelectedCharacter.LevelUpPoints > 0)
            {
                var attributeDef = player.SelectedCharacter.CharacterClass?.StatAttributes.FirstOrDefault(a => a.Attribute == statAttributeDefinition);
                if (attributeDef is { IncreasableByPlayer: true })
                {
                    player.Attributes![attributeDef.Attribute]++;
                    player.SelectedCharacter.LevelUpPoints--;
                    player.ViewPlugIns.GetPlugIn<IStatIncreaseResultPlugIn>()?.StatIncreaseResult(statAttributeDefinition, true);
                    return;
                }
            }

            player.ViewPlugIns.GetPlugIn<IStatIncreaseResultPlugIn>()?.StatIncreaseResult(statAttributeDefinition, false);
        }
    }
}
