// <copyright file="SetMoneyChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which sets a character's money.
/// </summary>
[Guid("B8E35F57-2ED4-4BAD-9F95-9C88E1B92B0C")]
[PlugIn("Set money command", "Sets money of a player.")]
[ChatCommandHelp(Command, "Sets money of a player. Usage: /setmoney <amount> <optional:character>", null)]
public class SetMoneyChatCommandPlugIn : ChatCommandPlugInBase<SetMoneyChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/setmoney";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string CharacterNotFoundMessage = "Character '{0}' not found.";
    private const string InvalidAmountMessage = "Invalid amount - must be between 0 and {0}.";
    private const string MoneySetMessage = "Money set to {0}.";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        var targetPlayer = player;
        if (arguments?.CharacterName != null)
        {
            targetPlayer = player.GameContext.GetPlayerByCharacterName(arguments.CharacterName);
            if (targetPlayer?.SelectedCharacter == null ||
                !targetPlayer.SelectedCharacter.Name.Equals(arguments.CharacterName, StringComparison.OrdinalIgnoreCase))
            {
                await this.ShowMessageToAsync(player, string.Format(CharacterNotFoundMessage, arguments.CharacterName)).ConfigureAwait(false);
                return;
            }
        }

        if (targetPlayer?.SelectedCharacter?.Inventory == null)
        {
            return;
        }

        if (targetPlayer.GameContext?.Configuration?.MaximumInventoryMoney is not int maxMoney)
        {
            return;
        }

        if (arguments == null || arguments.Amount < 0 || arguments.Amount > maxMoney)
        {
            await this.ShowMessageToAsync(player, string.Format(InvalidAmountMessage, maxMoney)).ConfigureAwait(false);
            return;
        }

        targetPlayer.Money = checked(arguments.Amount);
        await this.ShowMessageToAsync(player, string.Format(MoneySetMessage, arguments.Amount)).ConfigureAwait(false);
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