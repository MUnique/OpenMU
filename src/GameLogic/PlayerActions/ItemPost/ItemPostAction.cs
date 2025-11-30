// <copyright file="ItemPostAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemPost;

using System.Text;
using System.Linq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.ItemPost;
using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Handles the posting of an item into the chat.
/// </summary>
public class ItemPostAction
{
    /// <summary>
    /// Posts the specified item of the player to the global chat.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="storage">The storage which contains the item.</param>
    /// <param name="itemSlot">The item slot.</param>
    public async ValueTask PostItemAsync(Player player, Storages storage, byte itemSlot)
    {
        var item = this.GetItem(player, storage, itemSlot);
        if (item is null)
        {
            return;
        }

        var postState = player.GameContext.GetItemPostState();
        var postId = postState.AddItem(item);
        var itemName = this.BuildItemName(item);
        var messageText = $"#{postId} {itemName}";

        await player.GameContext.SendGlobalChatMessageAsync(player.SelectedCharacter!.Name, messageText, ChatMessageType.Normal).ConfigureAwait(false);
    }

    private string BuildItemName(Item item)
    {
        var stringBuilder = new StringBuilder();
        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent))
        {
            stringBuilder.Append("Excellent ");
        }

        var ancientSet = item.ItemSetGroups.FirstOrDefault(s => s.AncientSetDiscriminator != 0)?.ItemSetGroup;
        if (ancientSet is not null)
        {
            stringBuilder.Append(ancientSet.Name).Append(' ');
        }

        var itemName = item.Definition?.GetNameForLevel(item.Level);
        stringBuilder.Append(itemName);

        if (item.Level > 0)
        {
            stringBuilder.Append(' ').Append('+').Append(item.Level);
        }

        foreach (var option in item.ItemOptions
                     .Where(o => o.ItemOption?.OptionType is ItemOptionTypes.Option or ItemOptionTypes.Luck)
                     .OrderBy(o => o.ItemOption?.OptionType == ItemOptionTypes.Option))
        {
            var levelOption = option.ItemOption?.LevelDependentOptions.FirstOrDefault(o => o.Level == (option.ItemOption.LevelType == LevelType.ItemLevel ? item.Level : option.Level));
            var powerUpDefinition = levelOption?.PowerUpDefinition ?? option.ItemOption?.PowerUpDefinition;
            if (powerUpDefinition is not null)
            {
                stringBuilder.Append('+').Append(powerUpDefinition);
            }
            else if (option.ItemOption?.OptionType == ItemOptionTypes.Luck)
            {
                stringBuilder.Append("+Luck");
            }
        }

        if (item.HasSkill)
        {
            stringBuilder.Append("+Skill");
        }

        if (item.SocketCount > 0)
        {
            stringBuilder.Append('+').Append(item.SocketCount).Append('S');
        }

        return stringBuilder.ToString();
    }

    private Item? GetItem(Player player, Storages storage, byte itemSlot)
    {
        var itemStorage = storage switch
        {
            Storages.Inventory => player.Inventory,
            Storages.PersonalStore => player.ShopStorage,
            Storages.Vault => player.Vault,
            Storages.Trade or Storages.ChaosMachine or Storages.PetTrainer or Storages.Refinery or Storages.Smelting or Storages.ItemRestore or Storages.ChaosCardMaster or Storages.CherryBlossomSpirit or Storages.SeedCrafting or Storages.SeedSphereCrafting or Storages.SeedMountCrafting or Storages.SeedUnmountCrafting => player.TemporaryStorage,
            _ => null,
        };

        if (itemStorage is null)
        {
            return null;
        }

        return itemStorage.GetItem(itemSlot);
    }
}
