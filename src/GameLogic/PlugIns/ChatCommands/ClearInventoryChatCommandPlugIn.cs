// <copyright file="ClearInventoryChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which clears a character's inventory.
/// </summary>
[Guid("1E895A6F-3056-4A78-BA64-96E24363B8BC")]
[PlugIn("Clear Inventory chat command", "Clears inventory. Usage: /clearinv (optional:character)")]
[ChatCommandHelp(Command, "Clears inventory.", null, MinimumStatus)]
public class ClearInventoryChatCommandPlugIn : ChatCommandPlugInBase<ClearInventoryChatCommandPlugIn.Arguments>, ISupportCustomConfiguration<ClearInventoryChatCommandPlugIn.ClearInventoryConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    private const string Command = "/clearinv";
    private const CharacterStatus MinimumStatus = CharacterStatus.Normal;
    private const string CharacterNotFoundMessage = "Character '{0}' not found.";
    private const int ConfirmationTimeoutSeconds = 10;
    private readonly Dictionary<Guid, DateTime> pendingConfirmations = new();

    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public ClearInventoryConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    public object CreateDefaultConfig() => new ClearInventoryConfiguration();

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        if (player.SelectedCharacter is not { } selectedCharacter)
        {
            return;
        }

        var configuration = this.Configuration ??= (ClearInventoryConfiguration)this.CreateDefaultConfig();
        bool removeMoney = configuration.MoneyCost > 0;
        var targetPlayer = player;
        bool isGameMaster = selectedCharacter?.CharacterStatus >= CharacterStatus.GameMaster;
        if (isGameMaster)
        {
            removeMoney = false;
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
        }

        if (targetPlayer.Inventory is null)
        {
            return;
        }

        if (!isGameMaster && configuration.RequireConfirmation)
        {
            var playerId = selectedCharacter!.Id;
            if (!this.pendingConfirmations.TryGetValue(playerId, out var confirmationTime) || (DateTime.UtcNow - confirmationTime).TotalSeconds > ConfirmationTimeoutSeconds)
            {
                this.pendingConfirmations[playerId] = DateTime.UtcNow;
                await this.ShowMessageToAsync(player, configuration.ConfirmationMessage).ConfigureAwait(false);
                return;
            }

            this.pendingConfirmations.Remove(playerId);
        }

        var itemsToRemove = targetPlayer.Inventory.Items
            .Where(item => item is not null &&
                (item.ItemSlot < InventoryConstants.FirstEquippableItemSlotIndex ||
                item.ItemSlot > InventoryConstants.LastEquippableItemSlotIndex))
            .ToList();
        if (itemsToRemove.Count == 0)
        {
            return;
        }

        if (removeMoney && !player.TryRemoveMoney(configuration.MoneyCost))
        {
            await this.ShowMessageToAsync(player, configuration.NotEnoughMoneyMessage).ConfigureAwait(false);
            return;
        }

        foreach (var item in itemsToRemove)
        {
            await targetPlayer.DestroyInventoryItemAsync(item).ConfigureAwait(false);
        }

        await this.ShowMessageToAsync(player, configuration.InventoryClearedMessage).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Clear Inventory chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the character name to clear inventory for (GM only).
        /// </summary>
        public string? CharacterName { get; set; }
    }

    /// <summary>
    /// The configuration of a <see cref="ClearInventoryChatCommandPlugIn"/>.
    /// </summary>
    public class ClearInventoryConfiguration
    {
        /// <summary>
        /// Gets or sets the character name to clear inventory for (GM only).
        /// </summary>
        [Display(Name = "Money Cost", Description = @"The money cost to clear inventory (except GMs). Default: 0.")]
        public int MoneyCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player needs to run the command again within 10 seconds to confirm the inventory clearing (excluding GM).
        /// </summary>
        [Display(Name = "Require Confirmation", Description = @"If true, the player will need to run the command again within 10 seconds to confirm the inventory clearing (excluding GM).")]
        public bool RequireConfirmation { get; set; } = true;

        /// <summary>
        /// Gets or sets the message to show the confirmation message.
        /// </summary>
        [Display(Name = "Confirmation Message", Description = @"The message to show when the player needs to run the command again within 10 seconds to confirm the inventory clearing.")]
        public string ConfirmationMessage { get; set; } = "Confirmation: run again within 10 seconds to confirm inventory clearing";

        /// <summary>
        /// Gets or sets the message to show when the player does not have enough money to run the command.
        /// </summary>
        [Display(Name = "Not Enough Money Message", Description = @"The message to show when the player does not have enough money to run the command.")]
        public string NotEnoughMoneyMessage { get; set; } = "Not enough money to run command";

        /// <summary>
        /// Gets or sets the message to show when the inventory is cleared.
        /// </summary>
        [Display(Name = "Inventory Cleared Message", Description = @"The message to show when the inventory is cleared.")]
        public string InventoryClearedMessage { get; set; } = "Inventory cleared";
    }
}