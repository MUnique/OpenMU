// <copyright file="AddCrescentMoonSlashForDarkKnight.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Skills;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Adds the crescent moon slash skill (active in castle siege) to a created dark knight character.
    /// </summary>
    [Guid("2BB94D35-0DEF-4458-84AC-ECAAE6E896BE")]
    [PlugIn(nameof(AddCrescentMoonSlashForDarkKnight), "Adds the crescent moon slash skill (active in castle siege) to a created dark knight character.")]
    public class AddCrescentMoonSlashForDarkKnight : AddInitialSkillPlugInBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCrescentMoonSlashForDarkKnight"/> class.
        /// </summary>
        public AddCrescentMoonSlashForDarkKnight()
            : base((byte)CharacterClassNumber.DarkKnight, (ushort)SkillNumber.CrescentMoonSlash)
        {
        }
    }
}