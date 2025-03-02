// <copyright file="AddFireBlastForDarkLord.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the Fire Blast skill (active in castle siege) to a created dark lord character.
/// </summary>
[Guid("49D2D6F2-4E70-4F4B-B7CE-0645EC2F094A")]
[PlugIn(nameof(AddFireBlastForDarkLord), "Adds the Fire Blast skill (active in castle siege) to a created dark lord character.")]
public class AddFireBlastForDarkLord : AddInitialSkillPlugInBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddFireBlastForDarkLord"/> class.
    /// </summary>
    public AddFireBlastForDarkLord()
        : base((byte)CharacterClassNumber.DarkLord, (ushort)SkillNumber.FireBlast)
    {
    }
}