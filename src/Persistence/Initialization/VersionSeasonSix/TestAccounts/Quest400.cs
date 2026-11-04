// <copyright file="Quest400.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Initializer for an account to test the level 400 quests.
/// </summary>
internal class Quest400 : Quest220
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Quest400"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Quest400(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration, "quest3", 400)
    {
    }

    /// <inheritdoc/>
    protected override Character CreateCharacter(string name, CharacterClassNumber characterClass, int level, byte slot)
    {
        var character = base.CreateCharacter(name, characterClass, level, slot);
        character.Inventory!.Items.Add(this.CreateJewel(65, Items.Quest.FeatherOfDarkPhoenixNumber));
        character.Inventory.Items.Add(this.CreateJewel(66, Items.Quest.FlameOfDeathBeamKnightNumber));
        character.Inventory.Items.Add(this.CreateJewel(67, Items.Quest.HornOfHellMaineNumber));
        return character;
    }
}