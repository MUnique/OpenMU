// <copyright file="AddInitialItemPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Linq;
    using System.Reflection;
    using log4net;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlugIns;

    /// <summary>
    /// Base class for a <see cref="ICharacterCreatedPlugIn"/> which adds an item to the inventory
    /// of the created character.
    /// </summary>
    public class AddInitialItemPlugInBase : ICharacterCreatedPlugIn
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly byte characterClassNumber;
        private readonly byte itemGroup;
        private readonly byte itemNumber;
        private readonly byte itemSlot;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddInitialItemPlugInBase"/> class.
        /// </summary>
        /// <param name="characterClassNumber">The character class number.</param>
        /// <param name="itemGroup">The item group.</param>
        /// <param name="itemNumber">The item number.</param>
        /// <param name="itemSlot">The item slot.</param>
        protected AddInitialItemPlugInBase(byte characterClassNumber, byte itemGroup, byte itemNumber, byte itemSlot)
        {
            this.characterClassNumber = characterClassNumber;
            this.itemGroup = itemGroup;
            this.itemNumber = itemNumber;
            this.itemSlot = itemSlot;
        }

        /// <inheritdoc/>
        public void CharacterCreated(Player player, Character createdCharacter)
        {
            if (this.characterClassNumber != createdCharacter.CharacterClass.Number)
            {
                Log.DebugFormat("Wrong character class {0}, expected {1}", createdCharacter.CharacterClass.Number, this.characterClassNumber);
                return;
            }

            if (this.CreateItem(player, createdCharacter) is { } item)
            {
                createdCharacter.Inventory.Items.Add(item);
            }
        }

        /// <summary>
        /// Creates the item.
        /// Can be overwritten to modify the default.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="createdCharacter">The created character.</param>
        /// <returns>The created item.</returns>
        protected virtual Item CreateItem(Player player, Character createdCharacter)
        {
            if (player.GameContext.Configuration.Items
                    .FirstOrDefault(def => def.Group == this.itemGroup && def.Number == this.itemNumber)
                is { } itemDefinition)
            {
                var item = player.PersistenceContext.CreateNew<Item>();

                item.Definition = itemDefinition;
                item.Durability = item.Definition.Durability;
                item.ItemSlot = this.itemSlot;
                return item;
            }

            Log.Error($"Unknown item, group {this.itemGroup}, number {this.itemNumber}.");
            return null;
        }
    }
}