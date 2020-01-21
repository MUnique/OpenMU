// <copyright file="AddShortBowForFairyElf.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Items;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Adds a short bow to a created fairy elf character.
    /// </summary>
    [Guid("9EF17296-0436-4059-BC4E-0A71967F36EC")]
    [PlugIn(nameof(AddShortBowForFairyElf), "Adds a short bow to a created fairy elf character.")]
    public class AddShortBowForFairyElf : AddInitialItemPlugInBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddShortBowForFairyElf" /> class.
        /// </summary>
        public AddShortBowForFairyElf()
            : base((byte)CharacterClassNumber.FairyElf, (byte)ItemGroups.Bows, 0, 1)
        {
        }
    }
}