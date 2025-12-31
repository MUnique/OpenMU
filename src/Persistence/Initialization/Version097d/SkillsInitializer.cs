// <copyright file="SkillsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization logic for skills of version 0.97d.
/// </summary>
internal class SkillsInitializer : MUnique.OpenMU.Persistence.Initialization.Version095d.SkillsInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkillsInitializer"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SkillsInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        if (!this.GameConfiguration.Skills.Any(s => s.Number == (short)SkillNumber.CrescentMoonSlash))
        {
            this.CreateSkill(
                SkillNumber.CrescentMoonSlash,
                "Crescent Moon Slash",
                CharacterClasses.AllKnights,
                DamageType.Physical,
                90,
                4,
                abilityConsumption: 15,
                manaConsumption: 22,
                movesToTarget: true,
                movesTarget: true);
        }
    }
}
