// <copyright file="AddSmallAxeForDarkKnight.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Items;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Adds a small axe to a created dark knight character.
    /// </summary>
    [Guid("2377C222-4418-4F17-8388-1F8825E6243C")]
    [PlugIn(nameof(AddSmallAxeForDarkKnight), "Adds a small axe to a created dark knight character.")]
    public class AddSmallAxeForDarkKnight : AddInitialItemPlugInBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddSmallAxeForDarkKnight" /> class.
        /// </summary>
        public AddSmallAxeForDarkKnight() 
            : base((byte)CharacterClassNumber.DarkKnight, (byte)ItemGroups.Axes, 0, 0)
        {
        }
    }
}
