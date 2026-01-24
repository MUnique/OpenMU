// <copyright file="PKChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles pk commands.
/// </summary>
[Guid("30B7EFF0-33EE-4136-BEB0-BE503B748DC6")]
[PlugIn]
[Display(Name = nameof(PlugInResources.PkChatCommandPlugIn_Name), Description = nameof(PlugInResources.PkChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, "Sets player kill level and count of a character.", typeof(PkChatCommandArgs), CharacterStatus.GameMaster)]
public class PkChatCommandPlugIn : ChatCommandPlugInBase<PkChatCommandArgs>
{
    private const string Command = "/pk";
    private const int MinPkLevel = HeroState.PlayerKillWarning - HeroState.Normal;
    private const int MaxPkLevel = HeroState.PlayerKiller2ndStage - HeroState.Normal;

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, PkChatCommandArgs arguments)
    {
        if (arguments.Level < MinPkLevel || arguments.Level > MaxPkLevel)
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerKillLevelRangeError), MinPkLevel, MaxPkLevel).ConfigureAwait(false);
            return;
        }

        if (arguments.Count <= 0)
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerKillCountMustBePositive)).ConfigureAwait(false);
            return;
        }

        var targetPlayer = await this.GetPlayerByCharacterNameAsync(gameMaster, arguments.CharacterName ?? string.Empty).ConfigureAwait(false);
        var character = targetPlayer?.SelectedCharacter;
        if (character is null)
        {
            // logged out in the mean time ...
            return;
        }

        character.State = HeroState.Normal + arguments.Level;
        character.StateRemainingSeconds = (int)TimeSpan.FromHours(arguments.Count).TotalSeconds;
        character.PlayerKillCount = arguments.Count;
        await targetPlayer!.ForEachWorldObserverAsync<IUpdateCharacterHeroStatePlugIn>(p => p.UpdateCharacterHeroStateAsync(targetPlayer!), true).ConfigureAwait(false);

        await gameMaster.ShowLocalizedBlueMessageAsync(
            nameof(PlayerMessage.PlayerKillStateChangeResult),
            this.Key,
            character.Name,
            character.State,
            character.PlayerKillCount,
            Math.Round(TimeSpan.FromSeconds(character!.StateRemainingSeconds).TotalMinutes)).ConfigureAwait(false);
    }
}