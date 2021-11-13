// <copyright file="Orbs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initializes orb items which are used to learn skills.
/// </summary>
internal class Orbs : Version075.Items.Orbs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Orbs"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Orbs(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Initializes all orbs.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        this.CreateOrb(7, SkillNumber.TwistingSlash, 1, "Orb of Twisting Slash", 47, 80, 0, 0, 0, 29000, CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator);
    }
}