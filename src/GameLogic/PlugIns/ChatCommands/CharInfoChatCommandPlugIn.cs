// <copyright file="CharInfoChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.IO;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles charinfo commands.
/// </summary>
[Guid("0C7162BC-C74E-4A65-82E3-12811E4BE170")]
[PlugIn]
[Display(Name = nameof(PlugInResources.CharInfoChatCommandPlugIn_Name), Description = nameof(PlugInResources.CharInfoChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(CharInfoChatCommandArgs), CharacterStatus.GameMaster)]
public class CharInfoChatCommandPlugIn : ChatCommandPlugInBase<CharInfoChatCommandArgs>
{
    private const string Command = "/charinfo";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, CharInfoChatCommandArgs arguments)
    {
        var player = await this.GetPlayerByCharacterNameAsync(gameMaster, arguments.CharacterName ?? string.Empty).ConfigureAwait(false);

        if (player?.Account is not { } account
            || player.SelectedCharacter is not { } character)
        {
            return;
        }

        await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterInfoAccountNameFormat), account.LoginName).ConfigureAwait(false);

        await this.ShowAllLinesMessageToAsync(gameMaster, GetCharacterInfo(gameMaster, character)).ConfigureAwait(false);

        await this.ShowAllLinesMessageToAsync(gameMaster, player.Attributes?.ToString()).ConfigureAwait(false);
    }

    private static string GetCharacterInfo(Player gameMaster, Character character)
    {
        return gameMaster.GetLocalizedMessage(
            nameof(PlayerMessage.CharacterInfoFormat),
            character.Id,
            character.Name,
            character.CharacterClass?.Name.GetTranslation(gameMaster.Culture),
            character.CharacterSlot,
            character.CreateDate,
            character.Experience,
            character.LevelUpPoints,
            character.MasterExperience,
            character.MasterLevelUpPoints,
            character.CurrentMap?.Name,
            character.PositionX,
            character.PositionY,
            character.PlayerKillCount,
            character.StateRemainingSeconds,
            Enum.GetName(character.State),
            Enum.GetName(character.CharacterStatus),
            character.UsedFruitPoints,
            character.UsedNegFruitPoints,
            character.InventoryExtensions);
    }

    private async ValueTask ShowAllLinesMessageToAsync(Player gameMaster, string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        using var reader = new StringReader(message);

        while (true)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            if (line == null)
            {
                break;
            }

            await gameMaster.ShowBlueMessageAsync(line).ConfigureAwait(false);
        }
    }
}