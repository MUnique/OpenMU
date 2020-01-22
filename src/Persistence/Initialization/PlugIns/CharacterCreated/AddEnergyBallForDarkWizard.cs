// <copyright file="AddEnergyBallForDarkWizard.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Skills;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Adds the energy ball skill to a created dark wizard character.
    /// </summary>
    [Guid("6A721FED-51CD-41AD-BA7C-EC1642FFF00A")]
    [PlugIn(nameof(AddEnergyBallForDarkWizard), "Adds the energy ball skill to a created dark wizard character.")]
    public class AddEnergyBallForDarkWizard : AddInitialSkillPlugInBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddEnergyBallForDarkWizard" /> class.
        /// </summary>
        public AddEnergyBallForDarkWizard()
            : base((byte)CharacterClassNumber.DarkWizard, (ushort)SkillNumber.EnergyBall)
        {
        }
    }
}