// <copyright file="AddForceForDarkLord.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Skills;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Adds the Force skill to a created dark lord character.
    /// </summary>
    [Guid("D197321F-1BAC-4A82-8548-13674AF6D82C")]
    [PlugIn(nameof(AddForceForDarkLord), "Adds the Force skill to a created dark lord character.")]
    public class AddForceForDarkLord : AddInitialSkillPlugInBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddForceForDarkLord"/> class.
        /// </summary>
        public AddForceForDarkLord()
            : base((byte)CharacterClassNumber.DarkLord, (ushort)SkillNumber.Force)
        {
        }
    }
}