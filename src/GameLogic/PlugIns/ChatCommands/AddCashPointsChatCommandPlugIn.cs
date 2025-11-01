// <copyright file="AddCashPointsChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which adds cash points to a player's account.
/// </summary>
[Guid("F7A2C5D8-9E1B-4A3C-8F6D-2E9B7A4C1D3E")]
[PlugIn("Add cash points command", "Adds cash points to a player's account. Usage: /addcash (type) (amount) (optional:character)")]
[ChatCommandHelp(Command, "Adds cash points to a player. Types: wcoinc, wcoinp, goblin. Usage: /addcash (type) (amount) (optional:character)", null)]
public class AddCashPointsChatCommandPlugIn : ChatCommandPlugInBase<AddCashPointsChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/addcash";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string CharacterNotFoundMessage = "Character '{0}' not found.";
    private const string InvalidTypeMessage = "Invalid type. Use: wcoinc, wcoinp, or goblin";
    private const string InvalidAmountMessage = "Invalid amount - must be between 0 and 1000000.";
    private const string CashPointsAddedMessage = "{0} {1} points added successfully.";

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
                await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, CharacterNotFoundMessage, characterName)).ConfigureAwait(false);
                return;
            }
        }

        if (targetPlayer.Account is null)
        {
            return;
        }

        if (arguments is null || arguments.Amount < 0 || arguments.Amount > 1000000)
        {
            await this.ShowMessageToAsync(player, InvalidAmountMessage).ConfigureAwait(false);
            return;
        }

        var type = arguments.Type?.ToLowerInvariant();
        switch (type)
        {
            case "wcoinc":
                targetPlayer.Account.WCoinC += arguments.Amount;
                await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, CashPointsAddedMessage, arguments.Amount, "WCoinC")).ConfigureAwait(false);
                break;
            case "wcoinp":
                targetPlayer.Account.WCoinP += arguments.Amount;
                await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, CashPointsAddedMessage, arguments.Amount, "WCoinP")).ConfigureAwait(false);
                break;
            case "goblin":
                targetPlayer.Account.GoblinPoints += arguments.Amount;
                await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, CashPointsAddedMessage, arguments.Amount, "Goblin Points")).ConfigureAwait(false);
                break;
            default:
                await this.ShowMessageToAsync(player, InvalidTypeMessage).ConfigureAwait(false);
                break;
        }
    }

    /// <summary>
    /// Arguments for the Add Cash Points chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the type of cash points (wcoinc, wcoinp, goblin).
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the amount to add.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the character name to add points for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}
