// <copyright file="AddManaRaysForMagicGladiator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the Mana Rays skill (active in castle siege) to a created magic gladiator character.
/// </summary>
[Guid("D83347DB-4D33-47E9-B898-5EDB2777B8A1")]
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddManaRaysForMagicGladiator_Display1_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddManaRaysForMagicGladiator_Display1_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
public class AddManaRaysForMagicGladiator : AddInitialSkillPlugInBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddManaRaysForMagicGladiator"/> class.
    /// </summary>AddManaRaysForMagicGladiator.cs
    public AddManaRaysForMagicGladiator()
        : base((byte)CharacterClassNumber.MagicGladiator, (ushort)SkillNumber.ManaRays)
    {
    }
}