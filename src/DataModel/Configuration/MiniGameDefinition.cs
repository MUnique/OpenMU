// <copyright file="MiniGameDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Definition for a mini game.
/// </summary>
/// <remarks>
/// Each game level of a mini game has its own <see cref="MiniGameDefinition"/>.
/// </remarks>
[Cloneable]
public partial class MiniGameDefinition
{
    /// <summary>
    /// Gets or sets the type of the mini game.
    /// </summary>
    public MiniGameType Type { get; set; }

    /// <summary>
    /// Gets or sets the name of the mini game.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the mini game.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the level of the mini game.
    /// </summary>
    public byte GameLevel { get; set; }

    /// <summary>
    /// Gets or sets the creation policy of the mini game.
    /// </summary>
    public MiniGameMapCreationPolicy MapCreationPolicy { get; set; }

    /// <summary>
    /// Gets or sets the duration between opening the mini game map and actually starting the game.
    /// </summary>
    public TimeSpan EnterDuration { get; set; }

    /// <summary>
    /// Gets or sets the duration of the game.
    /// </summary>
    public TimeSpan GameDuration { get; set; }

    /// <summary>
    /// Gets or sets the duration after which the game map exists when the game finished.
    /// </summary>
    public TimeSpan ExitDuration { get; set; }

    /// <summary>
    /// Gets or sets the maximum player count which can enter the game.
    /// </summary>
    public int MaximumPlayerCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to save the score as <see cref="MUnique.OpenMU.DataModel.Statistics.MiniGameRankingEntry"/>.
    /// </summary>
    public bool SaveRankingStatistics { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether only characters with <see cref="CharacterClass.IsMasterClass"/> = <see langword="true"/> can enter.
    /// </summary>
    public bool RequiresMasterClass { get; set; }

    /// <summary>
    /// Gets or sets the minimum character level to which this mini game is available.
    /// </summary>
    public int MinimumCharacterLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum character level to which this mini game is available.
    /// </summary>
    public int MaximumCharacterLevel { get; set; }

    /// <summary>
    /// Gets or sets the minimum character level for special characters (MG, DL) to which this mini game is available.
    /// </summary>
    public int MinimumSpecialCharacterLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum character level for special characters (MG, DL) to which this mini game is available.
    /// </summary>
    public int MaximumSpecialCharacterLevel { get; set; }

    /// <summary>
    /// Gets or sets the ticket item level which is required to enter the event.
    /// </summary>
    public int TicketItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the entrance fee which is deducted from the players inventory
    /// when entering the mini game event.
    /// </summary>
    public int EntranceFee { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether player killers are allowed to
    /// enter the mini game.
    /// </summary>
    /// <value>
    ///   <c>true</c> if player killers are allowed to enter; otherwise, <c>false</c>.
    /// </value>
    public bool ArePlayerKillersAllowedToEnter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to allow being in a party during the event.
    /// </summary>
    public bool AllowParty { get; set; }

    /// <summary>
    /// Gets or sets the entrance gate to the mini game map.
    /// </summary>
    [Required]
    public virtual ExitGate? Entrance { get; set; }

    /// <summary>
    /// Gets or sets the ticket item which is required to enter the mini game.
    /// </summary>
    public virtual ItemDefinition? TicketItem { get; set; }

    /// <summary>
    /// Gets or sets the rewards which are given to the player when the game has been finished successfully.
    /// Multiple awards per players are possible.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<MiniGameReward> Rewards { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the spawn waves of the mini game.
    /// Overlapping waves are possible.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<MiniGameSpawnWave> SpawnWaves { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the mini game change events.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<MiniGameChangeEvent> ChangeEvents { get; protected set; } = null!;
}
