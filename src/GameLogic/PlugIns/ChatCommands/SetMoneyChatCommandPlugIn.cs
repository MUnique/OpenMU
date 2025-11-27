// <copyright file="SetMoneyChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which sets a character's money.
/// </summary>
[Guid("00AA4F0E-911D-49FE-8D88-114C7496D383")]
[PlugIn("Set money command", "Sets money of a player. Usage: /setmoney (amount) (optional:character)")]
[ChatCommandHelp(Command, "Sets money of a player. Usage: /setmoney (amount) (optional:character)", null)]
public class SetMoneyChatCommandPlugIn : ChatCommandPlugInBase<SetMoneyChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/setmoney";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        var targetPlayer = player;
        if (arguments?.CharacterName is { } characterName)
        {
            targetPlayer = player.GameContext.GetPlayerByCharacterName(characterName);
            if (targetPlayer?.SelectedCharacter is null ||
                !targetPlayer.SelectedCharacter.Name.Equals(characterName, StringComparison.OrdinalIgnoreCase))
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNotFound), characterName).ConfigureAwait(false);
                return;
            }
        }

        if (targetPlayer.SelectedCharacter?.Inventory is null)
        {
            return;
        }

        if (targetPlayer.GameContext?.Configuration?.MaximumInventoryMoney is not int maxMoney)
        {
            return;
        }

        if (arguments is null || arguments.Amount < 0 || arguments.Amount > maxMoney)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.InvalidMoneyAmount), maxMoney).ConfigureAwait(false);
            return;
        }

        targetPlayer.Money = checked(arguments.Amount);
        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SetMoneyResult), arguments.Amount).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Set Money chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the amount of money to set.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the character name to set money for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}