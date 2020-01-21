// <copyright file="AddSmallAxeForDarkKnight.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.Persistence.BasicModel;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.PlugIns;
    using Character = MUnique.OpenMU.DataModel.Entities.Character;
    using Item = MUnique.OpenMU.DataModel.Entities.Item;

    /// <summary>
    /// Adds a small axe to a created dark knight character.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ICharacterCreatedPlugIn" />
    [Guid("2377C222-4418-4F17-8388-1F8825E6243C")]
    [PlugIn(nameof(AddSmallAxeForDarkKnight), "Adds a small axe to a created dark knight character.")]
    public class AddSmallAxeForDarkKnight : ICharacterCreatedPlugIn
    {
        /// <inheritdoc />
        public void CharacterCreated(Player player, Character createdCharacter)
        {
            if (createdCharacter.CharacterClass.Number != (byte)CharacterClassNumber.DarkKnight
                && createdCharacter.CharacterClass.Number != (byte)CharacterClassNumber.MagicGladiator)
            {
                return;
            }

            if (player.GameContext.Configuration.Items
                    .FirstOrDefault(def => def.Group == 1 && def.Number == 0)
                is { } smallAxeDefinition)
            {
                var smallAxe = player.PersistenceContext.CreateNew<Item>();

                smallAxe.Definition = smallAxeDefinition;
                smallAxe.Durability = smallAxe.Definition.Durability;
                smallAxe.ItemSlot = 0;
                createdCharacter.Inventory.Items.Add(smallAxe);
            }
        }
    }
}
