// <copyright file="AddArrowsForFairyElf.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Items;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Adds arrows to a created fairy elf character.
    /// </summary>
    [Guid("71B6EB8D-E676-4B22-9E7E-15C7C3969852")]
    [PlugIn(nameof(AddArrowsForFairyElf), "Adds arrows to a created fairy elf character.")]
    public class AddArrowsForFairyElf : AddInitialItemPlugInBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddArrowsForFairyElf" /> class.
        /// </summary>
        public AddArrowsForFairyElf()
            : base((byte)CharacterClassNumber.FairyElf, (byte)ItemGroups.Bows, 15, 0)
        {
        }

        /// <inheritdoc />
        protected override Item CreateItem(Player player, Character createdCharacter)
        {
            if (base.CreateItem(player, createdCharacter) is { } item)
            {
                item.Durability = 255;
                return item;
            }

            return null;
        }
    }
}