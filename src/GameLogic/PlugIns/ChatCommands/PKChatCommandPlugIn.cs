// <copyright file="PKChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles pk commands.
    /// </summary>
    [Guid("30B7EFF0-33EE-4136-BEB0-BE503B748DC6")]
    [PlugIn("PK chat command", "Handles the chat command '/pk <char> <pk_lvl> <pk_count>'. Sets PK Level and Count for a character.")]
    [ChatCommandHelp(Command, typeof(PKChatCommandArgs), CharacterStatus.GameMaster)]
    public class PKChatCommandPlugIn : ChatCommandPlugInBase<PKChatCommandArgs>
    {
        private const string Command = "/pk";
        private const int MinPkLevel = HeroState.PlayerKillWarning - HeroState.Normal;
        private const int MaxPkLevel = HeroState.PlayerKiller2ndStage - HeroState.Normal;

        /// <inheritdoc />
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc />
        protected override void DoHandleCommand(Player gameMaster, PKChatCommandArgs arguments)
        {
            if (arguments.Level < MinPkLevel || arguments.Level > MaxPkLevel)
            {
                throw new ArgumentException($"PK level must be between {MinPkLevel} and {MaxPkLevel}.");
            }

            if (arguments.Count <= default(int))
            {
                throw new ArgumentException($"PK count must be greater than zero.");
            }

            var targetPlayer = this.GetPlayerByCharacterName(gameMaster, arguments.CharacterName ?? string.Empty);
            targetPlayer.SelectedCharacter!.State = HeroState.Normal + arguments.Level;
            targetPlayer.SelectedCharacter!.StateRemainingSeconds = (int)TimeSpan.FromHours(arguments.Count).TotalSeconds;
            targetPlayer.SelectedCharacter!.PlayerKillCount = arguments.Count;
            targetPlayer.ForEachWorldObserver(o => o.ViewPlugIns.GetPlugIn<IUpdateCharacterHeroStatePlugIn>()?.UpdateCharacterHeroState(targetPlayer), true);

            var message = string.Format(
                "The state of {0} has been changed to {1} with {2} murders for {3} minutes",
                targetPlayer.Name,
                targetPlayer.SelectedCharacter!.State,
                targetPlayer.SelectedCharacter!.PlayerKillCount,
                Math.Round(TimeSpan.FromSeconds(targetPlayer.SelectedCharacter!.StateRemainingSeconds).TotalMinutes));

            this.ShowMessageTo(gameMaster, $"[{this.Key}] {message}");
        }
    }
}
