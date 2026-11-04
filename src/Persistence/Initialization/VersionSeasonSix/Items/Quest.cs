// <copyright file="Quest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Initialization of quest items.
/// </summary>
public class Quest : InitializerBase
{
    /// <summary>
    /// The scroll of emperor number.
    /// </summary>
    internal const byte ScrollOfEmperorNumber = 23;

    /// <summary>
    /// The broken sword number.
    /// </summary>
    internal const byte BrokenSwordNumber = 24;

    /// <summary>
    /// The tear of elf number.
    /// </summary>
    internal const byte TearOfElfNumber = 25;

    /// <summary>
    /// The soul shard of wizard number.
    /// </summary>
    internal const byte SoulShardOfWizardNumber = 26;

    /// <summary>
    /// The eye of abyssal number.
    /// </summary>
    internal const byte EyeOfAbyssalNumber = 68;

    /// <summary>
    /// The flame of death beam knight number.
    /// </summary>
    internal const byte FlameOfDeathBeamKnightNumber = 65;

    /// <summary>
    /// The horn of hell maine number.
    /// </summary>
    internal const byte HornOfHellMaineNumber = 66;

    /// <summary>
    /// The feather of dark phoenix number.
    /// </summary>
    internal const byte FeatherOfDarkPhoenixNumber = 67;

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
        this.CreateQuestItem(ScrollOfEmperorNumber, LocalizedString.FromResource(() => ItemNames.ScrollOfEmperorRingOfHonor), 0, 1, 1); // Ring of Honor is level 1
        this.CreateQuestItem(BrokenSwordNumber, LocalizedString.FromResource(() => ItemNames.BrokenSwordDarkStone), 0, 2, 1); // Dark Stone is level 1
        this.CreateQuestItem(TearOfElfNumber, LocalizedString.FromResource(() => ItemNames.TearOfElf), 0, 1);
        this.CreateQuestItem(SoulShardOfWizardNumber, LocalizedString.FromResource(() => ItemNames.SoulShardOfWizard), 0, 1);
        this.CreateQuestItem(EyeOfAbyssalNumber, LocalizedString.FromResource(() => ItemNames.EyeOfAbyssal), 0, 2);
        this.CreateQuestItem(FlameOfDeathBeamKnightNumber, LocalizedString.FromResource(() => ItemNames.FlameOfDeathBeamKnight), 0, 1);
        this.CreateQuestItem(HornOfHellMaineNumber, LocalizedString.FromResource(() => ItemNames.HornOfHellMaine), 0, 2);
        this.CreateQuestItem(FeatherOfDarkPhoenixNumber, LocalizedString.FromResource(() => ItemNames.FeatherOfDarkPhoenix), 0, 2);
    }

    private void CreateQuestItem(byte number, LocalizedString name, byte dropLevel, byte height, byte maximumLevel = 0)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(item);
        item.Group = 14;
        item.Number = number;
        item.Width = 1;
        item.Height = height;
        item.Name = name;
        item.DropLevel = dropLevel;
        item.IsBoundToCharacter = true;
        item.IsQuestItem = true;
        item.StorageLimitPerCharacter = 1;
        item.DropsFromMonsters = false; // it'll be added explicitly to a DropItemGroup
        item.Durability = 1;
        item.MaximumItemLevel = maximumLevel;
        item.SetGuid(item.Group, item.Number);
    }
}