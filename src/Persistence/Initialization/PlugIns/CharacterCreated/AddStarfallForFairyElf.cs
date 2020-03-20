// <copyright file="AddStarfallForFairyElf.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Skills;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Adds the Starfall skill (active in castle siege) to a created fairy elf character.
    /// </summary>
    [Guid("5AC4F8C8-8B4C-4DD6-B1CF-D4F6491DC17A")]
    [PlugIn(nameof(AddStarfallForFairyElf), "Adds the Starfall skill (active in castle siege) to a created fairy elf character.")]
    public class AddStarfallForFairyElf : AddInitialSkillPlugInBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddStarfallForFairyElf"/> class.
        /// </summary>
        public AddStarfallForFairyElf()
            : base((byte)CharacterClassNumber.FairyElf, (ushort)SkillNumber.Starfall)
        {
        }
    }
}