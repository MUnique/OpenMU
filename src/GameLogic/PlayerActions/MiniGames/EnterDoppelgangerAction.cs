// <copyright file="EnterDoppelgangerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

using MUnique.OpenMU.GameLogic.MiniGames;

/// <summary>
/// Action to enter the doppelganger event.
/// </summary>
/// <remarks>
/// The event takes place on one of several maps, each with its own <see cref="MiniGameDefinition"/>.
/// The first player of a party enters a randomly chosen one, and the other members of the party
/// follow into the same instance.
/// </remarks>
public class EnterDoppelgangerAction
{
    private readonly EnterMiniGameAction _enterAction = new();

    /// <summary>
    /// Tries to enter the doppelganger event.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="ticketInventoryIndex">The inventory index of the ticket item, or 0xFF to search it in the inventory.</param>
    public async ValueTask TryEnterAsync(Player player, byte ticketInventoryIndex)
    {
        var definitions = player.GameContext.Configuration.MiniGameDefinitions
            .Where(definition => definition.Type == MiniGameType.Doppelganger)
            .ToList();
        if (definitions.Count == 0)
        {
            await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(MiniGameType.Doppelganger, EnterResult.Failed)).ConfigureAwait(false);
            return;
        }

        var definition = definitions.FirstOrDefault(d => player.GameContext.MiniGames.TryGetRunningMiniGame(d, player) is not null)
                         ?? definitions[Rand.NextInt(0, definitions.Count)];

        await this._enterAction.TryEnterMiniGameAsync(player, MiniGameType.Doppelganger, definition.GameLevel, ticketInventoryIndex).ConfigureAwait(false);
    }
}
