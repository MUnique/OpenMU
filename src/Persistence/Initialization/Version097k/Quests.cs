// <copyright file="Quests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

/// <summary>
/// Initialization of the quests of version 0.97k.
/// </summary>
/// <remarks>
/// The quests of Sevina the Priestess for the evolution to the second character class are the only quests of this version.
/// </remarks>
internal class Quests : VersionSeasonSix.Quests
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Quests"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Quests(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.FindScrollOfEmperor(CharacterClassNumber.DarkKnight);
        this.FindScrollOfEmperor(CharacterClassNumber.FairyElf);
        this.FindScrollOfEmperor(CharacterClassNumber.DarkWizard);
        this.TreasuresOfMu(CharacterClassNumber.DarkKnight, Quest.BrokenSwordNumber);
        this.TreasuresOfMu(CharacterClassNumber.FairyElf, Quest.TearOfElfNumber);
        this.TreasuresOfMu(CharacterClassNumber.DarkWizard, Quest.SoulShardOfWizardNumber);
    }
}
