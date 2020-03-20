// <copyright file="AddLanceForSummoner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Skills;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Adds the Lance skill to a created summoner character.
    /// </summary>
    [Guid("AC326833-D60E-4705-A7DF-740FA37ACBA8")]
    [PlugIn(nameof(AddLanceForSummoner), "Adds the Lance skill to a created summoner character.")]
    public class AddLanceForSummoner : AddInitialSkillPlugInBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddLanceForSummoner"/> class.
        /// </summary>
        public AddLanceForSummoner()
            : base((byte)CharacterClassNumber.Summoner, (ushort)SkillNumber.Lance)
        {
        }
    }
}