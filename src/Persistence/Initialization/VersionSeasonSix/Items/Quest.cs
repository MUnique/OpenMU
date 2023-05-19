// <copyright file="Quest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

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
        this.CreateQuestItem(ScrollOfEmperorNumber, "Scroll of Emperor;Ring of Honor", 0, 1, 1); // Ring of Honor is level 1
        this.CreateQuestItem(BrokenSwordNumber, "Broken Sword;Dark Stone", 0, 2, 1); // Dark Stone is level 1
        this.CreateQuestItem(TearOfElfNumber, "Tear of Elf", 0, 1);
        this.CreateQuestItem(SoulShardOfWizardNumber, "Soul Shard of Wizard", 0, 1);
        this.CreateQuestItem(EyeOfAbyssalNumber, "Eye of Abyssal", 0, 2);
        this.CreateQuestItem(FlameOfDeathBeamKnightNumber, "Flame of Death Beam Knight", 0, 1);
        this.CreateQuestItem(HornOfHellMaineNumber, "Horn of Hell Maine", 0, 2);
        this.CreateQuestItem(FeatherOfDarkPhoenixNumber, "Feather of Dark Phoenix", 0, 2);
    }

    private void CreateQuestItem(byte number, string name, byte dropLevel, byte height, byte maximumLevel = 0)
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
        item.StorageLimitPerCharacter = 1;
        item.DropsFromMonsters = false; // it'll be added explicitly to a DropItemGroup
        item.Durability = 1;
        item.MaximumItemLevel = maximumLevel;
        item.SetGuid(item.Group, item.Number);
    }
}