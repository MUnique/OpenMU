// <copyright file="DeleteCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character;

using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to delete a character in the character selection screen.
/// </summary>
public class DeleteCharacterAction
{
    /// <summary>
    /// Tries to delete the character.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="characterName">Name of the character.</param>
    /// <param name="securityCode">The security code.</param>
    public async ValueTask DeleteCharacterAsync(Player player, string characterName, string securityCode)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        var result = await this.DeleteCharacterRequestAsync(player, characterName, securityCode).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IShowCharacterDeleteResponsePlugIn>(p => p.ShowCharacterDeleteResponseAsync(result)).ConfigureAwait(false);
    }

    private async ValueTask<CharacterDeleteResult> DeleteCharacterRequestAsync(Player player, string characterName, string securityCode)
    {
        if (player.PlayerState.CurrentState != PlayerState.CharacterSelection)
        {
            player.Logger.LogError($"Account {player.Account?.LoginName} not in the right state, but {player.PlayerState.CurrentState}.");
            return CharacterDeleteResult.Unsuccessful;
        }

        var character = player.Account!.Characters.FirstOrDefault(c => c.Name == characterName);

        if (character is null)
        {
            player.Logger.LogError("Character not found. Hacker maybe tried to delete other players character!" +
                                   Environment.NewLine + "\tAccName: " + player.Account.LoginName +
                                   Environment.NewLine + "\tTried to delete Character: " + characterName);

            return CharacterDeleteResult.Unsuccessful;
        }

        var checkAsPassword = string.IsNullOrEmpty(player.Account.SecurityCode);
        if (checkAsPassword && !BCrypt.Net.BCrypt.Verify(securityCode, player.Account.PasswordHash))
        {
            return CharacterDeleteResult.WrongSecurityCode;
        }

        if (!checkAsPassword && player.Account.SecurityCode != securityCode)
        {
            return CharacterDeleteResult.WrongSecurityCode;
        }

        if (player.GameContext is IGameServerContext gameServerContext && await gameServerContext.GuildServer.GetGuildPositionAsync(character.Id).ConfigureAwait(false) != GuildPosition.Undefined)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Can't delete a guild member. Remove the character from guild first.", MessageType.BlueNormal)).ConfigureAwait(false);
            return CharacterDeleteResult.Unsuccessful;
        }

        player.Account.Characters.Remove(character);
        player.GameContext.PlugInManager.GetPlugInPoint<ICharacterDeletedPlugIn>()?.CharacterDeleted(player, character);
        await player.PersistenceContext.DeleteAsync(character).ConfigureAwait(false);
        return CharacterDeleteResult.Successful;
    }
}