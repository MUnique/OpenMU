// <copyright file="IncreaseStatsAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.World;
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
        if (player.SelectedCharacter is null)
        {
            throw new InvalidOperationException("No character selected");
        }

        if (amount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "The amount must be greater than 0.");
        }

        if (amount == 1)
        {
            await this.IncreaseStatsBySinglePointAsync(player, player.SelectedCharacter, targetAttribute).ConfigureAwait(false);
        }
        else
        {
            await this.IncreaseStatsByMultiplePointsAsync(player, amount, player.SelectedCharacter, targetAttribute).ConfigureAwait(false);
        }
    }

    private async ValueTask IncreaseStatsByMultiplePointsAsync(Player player, ushort amount, DataModel.Entities.Character selectedCharacter, AttributeDefinition targetAttribute)
    {
        if (!selectedCharacter.CanIncreaseStats(amount))
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Not enough level up points available.", MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        var attributeDef = selectedCharacter.CharacterClass?.GetStatAttribute(targetAttribute);
        if (attributeDef is { IncreasableByPlayer: true })
        {
            player.Attributes![attributeDef.Attribute] += amount;
            selectedCharacter.LevelUpPoints -= Math.Min(selectedCharacter.LevelUpPoints, amount);

            var map = player.CurrentMap!;

            await player.InvokeViewPlugInAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(player.GetAsEnumerable())).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IUpdateCharacterStatsPlugIn>(p => p.UpdateCharacterStatsAsync()).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IUpdateInventoryListPlugIn>(p => p.UpdateInventoryListAsync()).ConfigureAwait(false);
            var currentGate = new Persistence.BasicModel.ExitGate
            {
                Map = map.Definition,
                X1 = player.Position.X,
                X2 = player.Position.X,
                Y1 = player.Position.Y,
                Y2 = player.Position.Y,
            };

            await player.WarpToAsync(currentGate).ConfigureAwait(false);

            return;
        }

        await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Attribute not available.", MessageType.BlueNormal)).ConfigureAwait(false);
    }

    private async ValueTask IncreaseStatsBySinglePointAsync(Player player, DataModel.Entities.Character selectedCharacter,  AttributeDefinition targetAttribute)
    {
        if (!selectedCharacter.CanIncreaseStats())
        {
            await this.PublishIncreaseResultAsync(player, targetAttribute, false).ConfigureAwait(false);
            return;
        }

        var attributeDef = selectedCharacter.CharacterClass?.GetStatAttribute(targetAttribute);
        if (attributeDef is { IncreasableByPlayer: true })
        {
            player.Attributes![attributeDef.Attribute]++;
            if (selectedCharacter.LevelUpPoints > 0)
            {
                selectedCharacter.LevelUpPoints--;
            }

            await this.PublishIncreaseResultAsync(player, targetAttribute, true).ConfigureAwait(false);
            return;
        }

        await this.PublishIncreaseResultAsync(player, targetAttribute, false).ConfigureAwait(false);
    }

    private ValueTask PublishIncreaseResultAsync(Player player, AttributeDefinition statAttributeDefinition, bool success)
    {
        return player.InvokeViewPlugInAsync<IStatIncreaseResultPlugIn>(p => p.StatIncreaseResultAsync(statAttributeDefinition, success));
    }
}