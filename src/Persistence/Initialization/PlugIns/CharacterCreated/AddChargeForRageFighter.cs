// <copyright file="AddChargeForRageFighter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Skills;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Adds the Charge skill to a created rage fighter character.
    /// </summary>
    [Guid("53F23DDD-3676-4D24-8DFF-2EF657255832")]
    [PlugIn(nameof(AddChargeForRageFighter), "Adds the Charge skill to a created rage fighter character.")]
    public class AddChargeForRageFighter : AddInitialSkillPlugInBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddChargeForRageFighter"/> class.
        /// </summary>
        public AddChargeForRageFighter()
            : base((byte)CharacterClassNumber.RageFighter, (ushort)SkillNumber.Charge)
        {
        }
    }
}