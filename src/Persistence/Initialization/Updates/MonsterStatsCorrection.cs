// <copyright file="MonsterStatsCorrection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The corrected stats of a monster, which an update plugin applies to existing databases.
/// Only the stats which are set are changed.
/// </summary>
/// <param name="Number">The number of the monster.</param>
internal sealed record MonsterStatsCorrection(short Number)
{
    /// <summary>
    /// Gets the level.
    /// </summary>
    public int? Level { get; init; }

    /// <summary>
    /// Gets the maximum health.
    /// </summary>
    public int? MaximumHealth { get; init; }

    /// <summary>
    /// Gets the minimum physical base damage.
    /// </summary>
    public int? MinimumPhysicalDamage { get; init; }

    /// <summary>
    /// Gets the maximum physical base damage.
    /// </summary>
    public int? MaximumPhysicalDamage { get; init; }

    /// <summary>
    /// Gets the base defense.
    /// </summary>
    public int? Defense { get; init; }

    /// <summary>
    /// Gets the attack rate against other monsters and players (PvM).
    /// </summary>
    public int? AttackRate { get; init; }

    /// <summary>
    /// Gets the defense rate (PvM).
    /// </summary>
    public int? DefenseRate { get; init; }

    /// <summary>
    /// Gets the move range.
    /// </summary>
    public byte? MoveRange { get; init; }

    /// <summary>
    /// Gets the attack range.
    /// </summary>
    public byte? AttackRange { get; init; }

    /// <summary>
    /// Gets the view range.
    /// </summary>
    public short? ViewRange { get; init; }

    /// <summary>
    /// Gets the move delay in milliseconds.
    /// </summary>
    public int? MoveDelayMilliseconds { get; init; }

    /// <summary>
    /// Gets the attack delay in milliseconds.
    /// </summary>
    public int? AttackDelayMilliseconds { get; init; }

    /// <summary>
    /// Gets the respawn delay in seconds.
    /// </summary>
    public int? RespawnDelaySeconds { get; init; }

    /// <summary>
    /// Applies the stats which are set to the monster of the game configuration, if the configuration has it.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public void ApplyTo(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Monsters.FirstOrDefault(m => m.Number == this.Number) is not { } monster)
        {
            return;
        }

        SetAttribute(context, gameConfiguration, monster, Stats.Level, this.Level);
        SetAttribute(context, gameConfiguration, monster, Stats.MaximumHealth, this.MaximumHealth);
        SetAttribute(context, gameConfiguration, monster, Stats.MinimumPhysBaseDmg, this.MinimumPhysicalDamage);
        SetAttribute(context, gameConfiguration, monster, Stats.MaximumPhysBaseDmg, this.MaximumPhysicalDamage);
        SetAttribute(context, gameConfiguration, monster, Stats.DefenseBase, this.Defense);
        SetAttribute(context, gameConfiguration, monster, Stats.AttackRatePvm, this.AttackRate);
        SetAttribute(context, gameConfiguration, monster, Stats.DefenseRatePvm, this.DefenseRate);
        monster.MoveRange = this.MoveRange ?? monster.MoveRange;
        monster.AttackRange = this.AttackRange ?? monster.AttackRange;
        monster.ViewRange = this.ViewRange ?? monster.ViewRange;
        if (this.MoveDelayMilliseconds is { } moveDelay)
        {
            monster.MoveDelay = TimeSpan.FromMilliseconds(moveDelay);
        }

        if (this.AttackDelayMilliseconds is { } attackDelay)
        {
            monster.AttackDelay = TimeSpan.FromMilliseconds(attackDelay);
        }

        if (this.RespawnDelaySeconds is { } respawnDelay)
        {
            monster.RespawnDelay = TimeSpan.FromSeconds(respawnDelay);
        }
    }

    private static void SetAttribute(IContext context, GameConfiguration gameConfiguration, MonsterDefinition monster, AttributeDefinition attributeDefinition, int? value)
    {
        if (value is not { } newValue)
        {
            return;
        }

        if (monster.Attributes.FirstOrDefault(a => a.AttributeDefinition?.Id == attributeDefinition.Id) is { } attribute)
        {
            attribute.Value = newValue;
        }
        else
        {
            monster.AddAttributes(new Dictionary<AttributeDefinition, float> { { attributeDefinition, newValue } }, context, gameConfiguration);
        }
    }
}
