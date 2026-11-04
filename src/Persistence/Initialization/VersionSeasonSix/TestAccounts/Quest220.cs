// <copyright file="Quest220.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Initializer for an account to test the level 220 quests.
/// </summary>
internal class Quest220 : Quest150
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Quest220"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Quest220(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration, "quest2", 220)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Quest220"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="accountName">Name of the account.</param>
    /// <param name="level">The level.</param>
    public Quest220(IContext context, GameConfiguration gameConfiguration, string accountName, int level)
        : base(context, gameConfiguration, accountName, level)
    {
    }

    /// <inheritdoc/>
    protected override Character CreateKnight()
    {
        var character = base.CreateKnight();
        var ringOfHonor = this.CreateJewel(49, Items.Quest.ScrollOfEmperorNumber);
        ringOfHonor.Level = 1;
        character.Inventory!.Items.Add(ringOfHonor);
        var darkStone = this.CreateJewel(50, Items.Quest.BrokenSwordNumber);
        darkStone.Level = 1;
        character.Inventory.Items.Add(darkStone);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateWizard()
    {
        var character = base.CreateWizard();
        var ringOfHonor = this.CreateJewel(49, Items.Quest.ScrollOfEmperorNumber);
        ringOfHonor.Level = 1;
        character.Inventory!.Items.Add(ringOfHonor);
        return character;
    }

    /// <inheritdoc/>
    protected override Character CreateElf()
    {
        var character = base.CreateElf();
        var ringOfHonor = this.CreateJewel(49, Items.Quest.ScrollOfEmperorNumber);
        ringOfHonor.Level = 1;
        character.Inventory!.Items.Add(ringOfHonor);
        return character;
    }
}