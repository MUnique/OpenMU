// Copyright (c) MUnique. Licensed under the MIT license.

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Creates a stack (or multiple pieces) of an item.
/// Usage: /itemstack &lt;group&gt; &lt;number&gt; &lt;count&gt; &lt;lvl?&gt;.
/// If the item is stackable, it creates one item and sets Durability to min(count, Definition.Durability).
/// Otherwise, it creates 'count' separate items (subject to inventory space).
/// </summary>
[Guid("0D0B0F6E-0C55-4A8B-A2C0-8C9B67A8D3F2")]
[PlugIn("Item stack chat command", "Handles the chat command '/itemstack <group> <number> <count> <lvl?>'. Creates a stack or multiple pieces.")]
[ChatCommandHelp(Command, "Creates a stack or multiple pieces of an item.", typeof(ItemStackChatCommandArgs), CharacterStatus.GameMaster)]
public sealed class ItemStackChatCommandPlugIn : ChatCommandPlugInBase<ItemStackChatCommandArgs>
{
    private const string Command = "/itemstack";

    public override string Key => Command;

    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, ItemStackChatCommandArgs args)
    {
        if (args.Count <= 0)
        {
            var message = gameMaster.GetLocalizedMessage("Chat_ItemStack_CountPositive", "Count must be greater than zero.");
            await this.ShowMessageToAsync(gameMaster, message).ConfigureAwait(false);
            return;
        }

        var def = gameMaster.GameContext.Configuration.Items
            .FirstOrDefault(i => i.Group == args.Group && i.Number == args.Number);

        if (def is null)
        {
            var message = gameMaster.GetLocalizedMessage("Chat_ItemStack_ItemNotFound", "Item {0} {1} not found.", args.Group, args.Number);
            await this.ShowMessageToAsync(gameMaster, message).ConfigureAwait(false);
            return;
        }

        var isStackable = def.ItemSlot is null && def.Durability > 1;
        var created = 0;

        if (isStackable)
        {
            var item = gameMaster.PersistenceContext.CreateNew<Item>();
            item.Definition = def;
            item.Level = args.Level;
            item.Durability = Math.Min(args.Count, (int)def.Durability);

            var slot = gameMaster.Inventory!.CheckInvSpace(item);
            if (slot is not null && await gameMaster.Inventory.AddItemAsync(slot.Value, item).ConfigureAwait(false))
            {
                created = (int)item.Durability;
                var finalItem = gameMaster.Inventory.GetItem(slot.Value) ?? item;
                await gameMaster.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(finalItem)).ConfigureAwait(false);
            }
        }
        else
        {
            for (int i = 0; i < args.Count; i++)
            {
                var item = gameMaster.PersistenceContext.CreateNew<Item>();
                item.Definition = def;
                item.Level = args.Level;
                item.Durability = def.Durability;
                var slot = gameMaster.Inventory!.CheckInvSpace(item);
                if (slot is null || !await gameMaster.Inventory.AddItemAsync(slot.Value, item).ConfigureAwait(false))
                {
                    break;
                }
                created++;
                var finalItem = gameMaster.Inventory.GetItem(slot.Value) ?? item;
                await gameMaster.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(finalItem)).ConfigureAwait(false);
            }
        }

        if (created > 0)
        {
            var success = gameMaster.GetLocalizedMessage(
                "Chat_ItemStack_Success",
                "[{0}] Created {1}x {2}",
                this.Key,
                created,
                def.Name ?? string.Empty);
            await this.ShowMessageToAsync(gameMaster, success).ConfigureAwait(false);
        }
        else
        {
            var failure = gameMaster.GetLocalizedMessage("Chat_ItemStack_NoSpace", "[{0}] No space to create items", this.Key);
            await this.ShowMessageToAsync(gameMaster, failure).ConfigureAwait(false);
        }
    }
}
