// <copyright file="AddInitialItemPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Base class for a <see cref="ICharacterCreatedPlugIn"/> which adds an item to the inventory
/// of the created character.
/// </summary>
public class AddInitialItemPlugInBase : ICharacterCreatedPlugIn
{
    private readonly byte? _characterClassNumber;
    private readonly byte _itemGroup;
    private readonly byte _itemNumber;
    private readonly byte _itemSlot;
    private readonly byte _itemLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddInitialItemPlugInBase" /> class.
    /// </summary>
    /// <param name="characterClassNumber">The character class number.</param>
    /// <param name="itemGroup">The item group.</param>
    /// <param name="itemNumber">The item number.</param>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="itemLevel">The item level.</param>
    protected AddInitialItemPlugInBase(byte? characterClassNumber, byte itemGroup, byte itemNumber, byte itemSlot, byte itemLevel = 0)
    {
        this._characterClassNumber = characterClassNumber;
        this._itemGroup = itemGroup;
        this._itemNumber = itemNumber;
        this._itemSlot = itemSlot;
        this._itemLevel = itemLevel;
    }

    /// <inheritdoc/>
    public void CharacterCreated(Player player, Character createdCharacter)
    {
        using var logScope = player.Logger.BeginScope(this.GetType());
        if (this._characterClassNumber.HasValue && this._characterClassNumber != createdCharacter.CharacterClass?.Number)
        {
            player.Logger.LogDebug("Wrong character class {0}, expected {1}", createdCharacter.CharacterClass?.Number, this._characterClassNumber);
            return;
        }

        if (createdCharacter.Inventory!.Items.FirstOrDefault(i => i.ItemSlot == this._itemSlot) is { } existingItem)
        {
            player.Logger.LogError("Item slot {0} already contains an item ({1}).", this._itemSlot, existingItem);
            return;
        }

        if (this.CreateItem(player, createdCharacter) is { } item)
        {
            createdCharacter.Inventory!.Items.Add(item);
        }
    }

    /// <summary>
    /// Creates the item.
    /// Can be overwritten to modify the default.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="createdCharacter">The created character.</param>
    /// <returns>The created item.</returns>
    protected virtual Item? CreateItem(Player player, Character createdCharacter)
    {
        if (player.GameContext.Configuration.Items
                .FirstOrDefault(def => def.Group == this._itemGroup && def.Number == this._itemNumber)
            is { } itemDefinition)
        {
            var item = player.PersistenceContext.CreateNew<Item>();

            item.Definition = itemDefinition;
            item.Durability = item.Definition.Durability;
            item.ItemSlot = this._itemSlot;
            item.Level = this._itemLevel;
            return item;
        }

        player.Logger.LogWarning($"Unknown item, group {this._itemGroup}, number {this._itemNumber}.");
        return null;
    }
}