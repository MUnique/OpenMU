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
            await this.ShowMessageToAsync(gameMaster, "Count must be > 0").ConfigureAwait(false);
            return;
        }

        var def = gameMaster.GameContext.Configuration.Items
            .FirstOrDefault(i => i.Group == args.Group && i.Number == args.Number);

        if (def is null)
        {
            await this.ShowMessageToAsync(gameMaster, $"Item {args.Group} {args.Number} not found.").ConfigureAwait(false);
            return;
        }

        var isStackable = def.ItemSlot is null && def.Durability > 1;
        var created = 0;

        if (isStackable)
        {
            var item = new TemporaryItem
            {
                Definition = def,
                Level = args.Level,
                Durability = Math.Min(args.Count, (int)def.Durability),
            };

            if (await gameMaster.Inventory!.AddItemAsync(item).ConfigureAwait(false))
            {
                created = (int)item.Durability;
                await gameMaster.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
            }
        }
        else
        {
            for (int i = 0; i < args.Count; i++)
            {
                var item = new TemporaryItem
                {
                    Definition = def,
                    Level = args.Level,
                    Durability = def.Durability,
                };
                if (!await gameMaster.Inventory!.AddItemAsync(item).ConfigureAwait(false))
                {
                    break;
                }
                created++;
                await gameMaster.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
            }
        }

        await this.ShowMessageToAsync(gameMaster, created > 0
            ? $"[{this.Key}] Created {created}x {def.Name}"
            : $"[{this.Key}] No space to create items").ConfigureAwait(false);
    }
}
