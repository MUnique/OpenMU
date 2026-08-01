// <copyright file="CastleSiegeConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Main configuration for the castle siege event.
/// </summary>
[Cloneable]
public partial class CastleSiegeConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether the castle siege feature is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the number of seconds a guild must hold the crown to capture the castle.
    /// </summary>
    public int CrownHoldTimeSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the minimum combined level of a guild master required to register for the siege.
    /// </summary>
    public int RegisterMinLevel { get; set; } = 200;

    /// <summary>
    /// Gets or sets the minimum number of guild members required to register for the siege.
    /// </summary>
    public int RegisterMinMembers { get; set; } = 20;

    /// <summary>
    /// Gets or sets the minimum number of seconds a participant must be present in the battle to be eligible for a reward.
    /// </summary>
    public int ParticipantRewardMinSeconds { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of attacking alliance slots.
    /// </summary>
    public int MaxAttackingGuilds { get; set; } = 3;

    /// <summary>
    /// Gets or sets the guild score awarded to the guild that wins the siege.
    /// </summary>
    public int GuildScoreCastleSiege { get; set; }

    /// <summary>
    /// Gets or sets the guild score awarded to alliance member guilds of the winning side.
    /// </summary>
    public int GuildScoreCastleSiegeMembers { get; set; }

    /// <summary>
    /// Gets or sets the Zen cost for the castle owner to re-purchase a destroyed gate.
    /// </summary>
    public int GateBuyPrice { get; set; }

    /// <summary>
    /// Gets or sets the Zen cost for the castle owner to re-purchase a destroyed statue.
    /// </summary>
    public int StatueBuyPrice { get; set; }

    /// <summary>
    /// Gets or sets the map definition for the Valley of Loren (map 30), where the siege takes place.
    /// </summary>
    public virtual GameMapDefinition? CastleSiegeMapDefinition { get; set; }

    /// <summary>
    /// Gets or sets the map definition for the Land of Trials (map 31), the castle-owner's exclusive zone.
    /// </summary>
    public virtual GameMapDefinition? LandOfTrialsMapDefinition { get; set; }

    /// <summary>
    /// Gets or sets the item definition for the participation reward item.
    /// </summary>
    public virtual ItemDefinition? RewardItemDefinition { get; set; }

    /// <summary>
    /// Gets or sets the schedule entries that define when each siege state begins.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CastleSiegeStateScheduleEntry> StateSchedule { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the definitions for all castle siege NPCs (gates, statues, etc.).
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CastleSiegeNpcDefinition> NpcDefinitions { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the upgrade levels for gate defense.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CastleSiegeUpgradeDefinition> GateDefenseUpgrades { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the upgrade levels for gate maximum HP.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CastleSiegeUpgradeDefinition> GateLifeUpgrades { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the upgrade levels for statue defense.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CastleSiegeUpgradeDefinition> StatueDefenseUpgrades { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the upgrade levels for statue maximum HP.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CastleSiegeUpgradeDefinition> StatueLifeUpgrades { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the upgrade levels for statue HP regeneration.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CastleSiegeUpgradeDefinition> StatueRegenUpgrades { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the zones on the siege map where attacking siege machines may be placed.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CastleSiegeZoneDefinition> AttackMachineZones { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the zones on the siege map where defensive siege machines may be placed.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CastleSiegeZoneDefinition> DefenseMachineZones { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the zone where defending players respawn during the siege.
    /// </summary>
    [MemberOfAggregate]
    public virtual CastleSiegeZoneDefinition? DefenseRespawnArea { get; set; }

    /// <summary>
    /// Gets or sets the zone where attacking players respawn during the siege.
    /// </summary>
    [MemberOfAggregate]
    public virtual CastleSiegeZoneDefinition? AttackRespawnArea { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return "Castle Siege Configuration";
    }
}
