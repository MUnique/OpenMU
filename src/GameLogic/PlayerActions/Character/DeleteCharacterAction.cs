// <copyright file="DeleteCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to delete a character in the character selection screen.
    /// </summary>
    public class DeleteCharacterAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(DeleteCharacterAction));

        /// <summary>
        /// Tries to delete the character.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="characterName">Name of the character.</param>
        /// <param name="securityCode">The security code.</param>
        public void DeleteCharacter(Player player, string characterName, string securityCode)
        {
            var result = this.DeleteCharacterRequest(player, characterName, securityCode);
            player.ViewPlugIns.GetPlugIn<IShowCharacterDeleteResponsePlugIn>()?.ShowCharacterDeleteResponse(result);
        }

        private CharacterDeleteResult DeleteCharacterRequest(Player player, string characterName, string securityCode)
        {
            if (player.PlayerState.CurrentState != PlayerState.CharacterSelection)
            {
                Log.Error($"Account {player.Account.LoginName} not in the right state, but {player.PlayerState.CurrentState}.");
                return CharacterDeleteResult.Unsuccessful;
            }

            var character = player.Account.Characters.FirstOrDefault(c => c.Name == characterName);

            if (character == null)
            {
                Log.Error("Character not found. Hacker maybe tried to delete other players character!" +
                    Environment.NewLine + "\tAccName: " + player.Account.LoginName +
                    Environment.NewLine + "\tTried to delete Character: " + characterName);

                return CharacterDeleteResult.Unsuccessful;
            }

            if (player.Account.SecurityCode != null && player.Account.SecurityCode != securityCode)
            {
                return CharacterDeleteResult.WrongSecurityCode;
            }

            if (player.GameContext is IGameServerContext gameServerContext && gameServerContext.GuildServer.GetGuildPosition(character.Id) != null)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Can't delete a guild member. Remove the character from guild first.", MessageType.BlueNormal);
                return CharacterDeleteResult.Unsuccessful;
            }

            player.Account.Characters.Remove(character);
            player.GameContext.PlugInManager.GetPlugInPoint<ICharacterDeletedPlugIn>()?.CharacterDeleted(player, character);
            player.PersistenceContext.Delete(character);
            return CharacterDeleteResult.Successful;
        }
    }
}
