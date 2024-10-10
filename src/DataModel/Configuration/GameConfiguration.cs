// <copyright file="GameConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Defines the game configuration.
/// A game configuration contains the whole configuration of a game, directly or indirectly.
/// </summary>
[AggregateRoot]
[Cloneable]
public partial class GameConfiguration
{
    /// <summary>
    /// Gets or sets the maximum reachable level.
    /// </summary>
    public short MaximumLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum reachable master level.
    /// </summary>
    public short MaximumMasterLevel { get; set; }

    /// <summary>
    /// Gets or sets the experience rate of the game.
    /// </summary>
    public float ExperienceRate { get; set; }

    /// <summary>
    /// Gets or sets the minimum monster level which are required to be killed
    /// in order to gain master experience for master character classes.
    /// </summary>
    public byte MinimumMonsterLevelForMasterExperience { get; set; }

    /// <summary>
    /// Gets or sets the information range. This defines how far players can see other game objects.
    /// </summary>
    public byte InfoRange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether area skills hit players.
    /// </summary>
    /// <remarks>
    /// Usually false, during castle siege this might be true.
    /// </remarks>
    public bool AreaSkillHitsPlayer { get; set; }

    /// <summary>
    /// Gets or sets the maximum inventory money value.
    /// </summary>
    public int MaximumInventoryMoney { get; set; }

    /// <summary>
    /// Gets or sets the maximum vault money value.
    /// </summary>
    public int MaximumVaultMoney { get; set; }

    /// <summary>
    /// Gets or sets the experience formula per level. The variable name for the level is "level".
    /// </summary>
    public string? ExperienceFormula { get; set; }

    /// <summary>
    /// Gets or sets the experience formula per master level. The variable name for the level is "level".
    /// </summary>
    public string? MasterExperienceFormula { get; set; }

    /// <summary>
    /// Gets or sets the interval for attribute recoveries. See also MUnique.OpenMU.GameLogic.Attributes.Stats.Regeneration.
    /// </summary>
    public int RecoveryInterval { get; set; }

    /// <summary>
    /// Gets or sets the maximum numbers of letters a player can have in his inbox.
    /// </summary>
    public int MaximumLetters { get; set; }

    /// <summary>
    /// Gets or sets the price of sending a letter.
    /// </summary>
    public int LetterSendPrice { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of characters per account.
    /// </summary>
    public byte MaximumCharactersPerAccount { get; set; }

    /// <summary>
    /// Gets or sets the character name regex.
    /// </summary>
    /// <remarks>
    /// "^[a-zA-Z0-9]{3,10}$";.
    /// </remarks>
    public string? CharacterNameRegex { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the password.
    /// </summary>
    public int MaximumPasswordLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of parties.
    /// </summary>
    public byte MaximumPartySize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether if a monster should drop or adds money to the character directly.
    /// </summary>
    public bool ShouldDropMoney { get; set; }

    /// <summary>
    /// Gets or sets the duration of item drops on the ground.
    /// </summary>
    public TimeSpan ItemDropDuration { get; set; }

    /// <summary>
    /// Gets or sets the maximum droppable item option level.
    /// </summary>
    public byte MaximumItemOptionLevelDrop { get; set; }

    /// <summary>
    /// Gets or sets the accumulated damage which needs to be done to decrease <see cref="Item.Durability"/> of a defending item by 1.
    /// </summary>
    public double DamagePerOneItemDurability { get; set; }

    /// <summary>
    /// Gets or sets the accumulated damage which needs to be done to decrease <see cref="Item.Durability"/> of a pet item by 1.
    /// </summary>
    public double DamagePerOnePetDurability { get; set; }

    /// <summary>
    /// Gets or sets the number of hits which needs to be done to decrease the <see cref="Item.Durability"/> of an offensive item by 1.
    /// </summary>
    public double HitsPerOneItemDurability { get; set; }

    /// <summary>
    /// Gets or sets the duel configuration.
    /// </summary>
    [MemberOfAggregate]
    public virtual DuelConfiguration? DuelConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the possible jewel mixes.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<JewelMix> JewelMixes { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the warp list.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<WarpInfo> WarpList { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the drop item groups which can be assigned to maps and characters.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<DropItemGroup> DropItemGroups { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the skills of this game configuration.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<Skill> Skills { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the character classes.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CharacterClass> CharacterClasses { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the item definitions.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemDefinition> Items { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the item level bonus tables.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemLevelBonusTable> ItemLevelBonusTables { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the item slot types.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemSlotType> ItemSlotTypes { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the item option definitions.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemOptionDefinition> ItemOptions { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the item option types.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemOptionType> ItemOptionTypes { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the item set groups.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemSetGroup> ItemSetGroups { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the item option combination bonuses.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemOptionCombinationBonus> ItemOptionCombinationBonuses { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the map definitions.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<GameMapDefinition> Maps { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the monster definitions.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<MonsterDefinition> Monsters { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the attributes.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<AttributeDefinition> Attributes { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the magic effects.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<MagicEffectDefinition> MagicEffects { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the master skill roots.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<MasterSkillRoot> MasterSkillRoots { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the plug in configurations.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<PlugInConfiguration> PlugInConfigurations { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the event definitions.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<MiniGameDefinition> MiniGameDefinitions { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return "Default Game Configuration";
    }
}