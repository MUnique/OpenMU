// <copyright file="PKClearCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The pkclear command which clears players pk status.
    /// </summary>
    [Guid("8025961F-96B3-46B3-9EED-0CB05CDAC3E0")]
    [PlugIn("PKClear", "Clears players pk status")]
    [ChatCommandHelp(Command, null, MinimumStatus)]
    public class PKClearCommand : IChatCommandPlugIn
    {
        private const string Command = "/pkclear";

        private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            try
            {
                int cost = player.GameContext.Configuration.PKClearMoneyCost;

                if (player.SelectedCharacter.State >= HeroState.PlayerKillWarning && player.TryRemoveMoney(cost))
                {
                    player.SelectedCharacter.State = HeroState.Normal;

                    if (cost <= 0)
                    {
                        player.ViewPlugIns.GetPlugIn<IUpdateCharacterHeroStatePlugIn>()?.UpdateCharacterHeroState();
                        player.ShowMessage($"[PK System] Status cleared.");
                        return;
                    }

                    player.ViewPlugIns.GetPlugIn<IUpdateCharacterHeroStatePlugIn>()?.UpdateCharacterHeroState();
                    player.ShowMessage($"[PK System] Status cleared, -{cost} zen.");
                }
                else
                {
                    if (player.SelectedCharacter.State <= HeroState.Normal)
                    {
                        player.ShowMessage($"[PK System] Status cannot be cleared, your status is {player.SelectedCharacter.State.ToString().ToLower()}.");
                    }
                    else if (player.SelectedCharacter.State >= HeroState.PlayerKillWarning && !player.TryRemoveMoney(cost))
                    {
                        player.ShowMessage($"[PK System] Status cannot be cleared, zen required: {cost}.");
                    }
                }
            }
            catch (ArgumentException e)
            {
                player.ShowMessage(e.Message);
            }
        }
    }
}