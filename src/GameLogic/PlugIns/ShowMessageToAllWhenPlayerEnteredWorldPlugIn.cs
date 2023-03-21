// <copyright file="ShowMessageToAllWhenPlayerEnteredWorldPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which shows a message to all players when a player enters the game.
/// </summary>
[PlugIn(nameof(ShowMessageToAllWhenPlayerEnteredWorldPlugIn), "Shows a message to all players when a player enters the game.")]
[Guid("12784A17-1085-408E-99CE-5233FDA2B177")]
public class ShowMessageToAllWhenPlayerEnteredWorldPlugIn : IPlayerStateChangedPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (previousState != PlayerState.CharacterSelection || currentState != PlayerState.EnteredWorld || player.SelectedCharacter is not { } selectedCharacter)
        {
            return;
        }

        await player.GameContext.SendGlobalMessageAsync($"{selectedCharacter.Name} entered the game.", MessageType.BlueNormal).ConfigureAwait(false);
    }
}