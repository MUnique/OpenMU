// <copyright file="Quest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Initializer for the quest items of version 0.97k, which are required for the evolution to the second character class.
/// </summary>
/// <remarks>
/// The items have no item levels in this version, so they are just called e.g. 'Scroll of Emperor' instead of 'Ring of Honor'.
/// </remarks>
internal class Quest : VersionSeasonSix.Items.Quest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Quest"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Quest(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateQuestItem(ScrollOfEmperorNumber, LocalizedString.FromResource(() => ItemNames.ScrollOfEmperorRingOfHonor), 0, 1);
        this.CreateQuestItem(BrokenSwordNumber, LocalizedString.FromResource(() => ItemNames.BrokenSwordDarkStone), 0, 2);
        this.CreateQuestItem(TearOfElfNumber, LocalizedString.FromResource(() => ItemNames.TearOfElf), 0, 1);
        this.CreateQuestItem(SoulShardOfWizardNumber, LocalizedString.FromResource(() => ItemNames.SoulShardOfWizard), 0, 1);
    }
}
