// <copyright file="Character.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The hero state of a player. Given enough time, the state converges to <see cref="Normal"/>.
/// </summary>
public enum HeroState
{
    /// <summary>
    /// The character is new.
    /// </summary>
    New,

    /// <summary>
    /// The character is a hero.
    /// </summary>
    Hero,

    /// <summary>
    /// The character is a hero, but the hero state is almost gone.
    /// </summary>
    LightHero,

    /// <summary>
    /// The normal state.
    /// </summary>
    Normal,

    /// <summary>
    /// The character killed another character, and has a kill warning.
    /// </summary>
    PlayerKillWarning,

    /// <summary>
    /// The character killed two characters, and has some restrictions.
    /// </summary>
    PlayerKiller1stStage,

    /// <summary>
    /// The character killed more than two characters, and has hard restrictions.
    /// </summary>
    PlayerKiller2ndStage,
}

/// <summary>
/// The Character Status of a player.
/// </summary>
public enum CharacterStatus
{
    /// <summary>
    /// The character is normal.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// The character is banned.
    /// </summary>
    Banned = 1,

    /// <summary>
    /// The character is a GameMaster (have mu logo on the head).
    /// </summary>
    GameMaster = 32,
}

/// <summary>
/// The character pose.
/// </summary>
public enum CharacterPose : byte
{
    /// <summary>
    /// The character is standing (normal).
    /// </summary>
    Standing = 0,

    /// <summary>
    /// The character is sitting on an object.
    /// </summary>
    Sitting = 2,

    /// <summary>
    /// The character is leaning towards something (wall etc).
    /// </summary>
    Leaning = 3,

    /// <summary>
    /// The character is hanging on something.
    /// </summary>
    Hanging = 4,
}

/// <summary>
/// The character of a player.
/// </summary>
public class Character
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the character class.
    /// </summary>
    [Required]
    public virtual CharacterClass? CharacterClass { get; set; }

    /// <summary>
    /// Gets or sets the character slot in the account.
    /// </summary>
    public byte CharacterSlot { get; set; }

    /// <summary>
    /// Gets or sets the create date.
    /// </summary>
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the experience.
    /// </summary>
    public long Experience { get; set; }

    /// <summary>
    /// Gets or sets the master experience.
    /// </summary>
    public long MasterExperience { get; set; }

    /// <summary>
    /// Gets or sets the remaining level up points which can be spent on increasable stat attributes.
    /// </summary>
    public int LevelUpPoints { get; set; }

    /// <summary>
    /// Gets or sets the master level up points which can be spent on master skills.
    /// </summary>
    public int MasterLevelUpPoints { get; set; }

    /// <summary>
    /// Gets or sets the current game map.
    /// </summary>
    [Required]
    public virtual GameMapDefinition? CurrentMap { get; set; }

    /// <summary>
    /// Gets or sets the x-coordinate of its map position.
    /// </summary>
    public byte PositionX { get; set; }

    /// <summary>
    /// Gets or sets the y-coordinate of its map position.
    /// </summary>
    public byte PositionY { get; set; }

    /// <summary>
    /// Gets or sets the player kill count.
    /// </summary>
    public int PlayerKillCount { get; set; }

    /// <summary>
    /// Gets or sets the remaining seconds for the current hero state, when the player state is not normal.
    /// </summary>
    public int StateRemainingSeconds { get; set; }

    /// <summary>
    /// Gets or sets the hero state.
    /// </summary>
    public HeroState State { get; set; }

    /// <summary>
    /// Gets or sets the character status.
    /// </summary>
    public CharacterStatus CharacterStatus { get; set; }

    /// <summary>
    /// Gets or sets the pose.
    /// </summary>
    public CharacterPose Pose { get; set; }

    /// <summary>
    /// Gets or sets the used fruit points.
    /// </summary>
    public int UsedFruitPoints { get; set; }

    /// <summary>
    /// Gets or sets the used negative fruit points.
    /// </summary>
    public int UsedNegFruitPoints { get; set; }

    /// <summary>
    /// Gets or sets the number of inventory extensions.
    /// </summary>
    public int InventoryExtensions { get; set; }

    /// <summary>
    /// Gets or sets the key configuration, which is set by the client and just saved as is.
    /// </summary>
    public byte[]? KeyConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the configuration of the mu helper, which is set by the client and just saved as is.
    /// </summary>
    public byte[]? MuHelperConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the stat attributes.
    /// </summary>
    /// <remarks>
    /// Please note, that it's not possible to add stat attribute with the same
    /// attribute definition to the <see cref="Account.Attributes"/> and the <see cref="Character.Attributes"/>.
    /// </remarks>
    [MemberOfAggregate]
    public virtual ICollection<StatAttribute> Attributes { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the letters.
    /// </summary>
    [MemberOfAggregate]
    public virtual IList<LetterHeader> Letters { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the learned skills.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<SkillEntry> LearnedSkills { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the inventory.
    /// </summary>
    [MemberOfAggregate]
    public virtual ItemStorage? Inventory { get; set; }

    /// <summary>
    /// Gets or sets the drop item groups.
    /// </summary>
    public virtual ICollection<DropItemGroup> DropItemGroups { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the quest states.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CharacterQuestState> QuestStates { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Name;
    }
}