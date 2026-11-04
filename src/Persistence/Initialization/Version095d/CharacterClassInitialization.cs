// <copyright file="CharacterClassInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Initialization of character classes data for Version 0.95d.
/// </summary>
internal class CharacterClassInitialization : Initialization.CharacterClasses.CharacterClassInitialization
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterClassInitialization"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CharacterClassInitialization(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override bool UseClassicPvp => true;

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateDarkKnight(CharacterClassNumber.DarkKnight, LocalizedString.FromResource(() => CharacterClassNames.DarkKnight), false, null, true);
        this.CreateDarkWizard(CharacterClassNumber.DarkWizard, LocalizedString.FromResource(() => CharacterClassNames.DarkWizard), false, null, true);
        this.CreateFairyElf(CharacterClassNumber.FairyElf, LocalizedString.FromResource(() => CharacterClassNames.FairyElf), false, null, true);
        this.CreateMagicGladiator(CharacterClassNumber.MagicGladiator, LocalizedString.FromResource(() => CharacterClassNames.MagicGladiator), false, null, true);
    }
}