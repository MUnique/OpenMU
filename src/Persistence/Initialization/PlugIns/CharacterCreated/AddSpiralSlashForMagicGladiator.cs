// <copyright file="AddSpiralSlashForMagicGladiator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the Spiral Slash skill (active in castle siege) to a created magic gladiator character.
/// </summary>
[Guid("83B0D163-7E97-40FC-851A-D5500B4BB33E")]
[PlugIn(nameof(AddSpiralSlashForMagicGladiator), "Adds the Spiral Slash skill (active in castle siege) to a created magic gladiator character.")]
public class AddSpiralSlashForMagicGladiator : AddInitialSkillPlugInBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddSpiralSlashForMagicGladiator"/> class.
    /// </summary>
    public AddSpiralSlashForMagicGladiator()
        : base((byte)CharacterClassNumber.MagicGladiator, (ushort)SkillNumber.SpiralSlash)
    {
    }
}