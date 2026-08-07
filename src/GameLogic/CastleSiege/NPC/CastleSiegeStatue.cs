// <copyright file="CastleSiegeStatue.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An attackable Castle Siege Guardian Statue.
/// </summary>
public sealed class CastleSiegeStatue : CastleSiegeAttackableNpc
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeStatue"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The NPC definition.</param>
    /// <param name="map">The map on which the statue is spawned.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="runtime">The Castle Siege runtime entry.</param>
    /// <param name="intelligence">The statue intelligence.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="plugInManager">The plugin manager.</param>
    public CastleSiegeStatue(
        MonsterSpawnArea spawnInfo,
        MonsterDefinition stats,
        GameMap map,
        CastleSiegeContext context,
        CastleSiegeNpcRuntime runtime,
        CastleSiegeStatueIntelligence intelligence,
        IDropGenerator dropGenerator,
        PlugInManager plugInManager)
        : base(spawnInfo, stats, map, context, runtime, intelligence, dropGenerator, plugInManager)
    {
    }

    /// <summary>
    /// Gets the monster number of Castle Siege Guardian Statues.
    /// </summary>
    public static short MonsterNumber { get; } = 283;

    /// <summary>
    /// Gets the defense upgrade level.
    /// </summary>
    public byte DefenseLevel => this.State.DefenseLevel;

    /// <summary>
    /// Gets the life upgrade level.
    /// </summary>
    public byte LifeLevel => this.State.LifeLevel;

    /// <summary>
    /// Gets the regeneration upgrade level.
    /// </summary>
    public byte RegenLevel => this.State.RegenLevel;

    /// <inheritdoc />
    public override void ApplyPersistedUpgrades(bool preserveMissingHealth)
    {
        this.SetDefense(GetValue(this.Context.Configuration.StatueDefenseUpgrades, this.DefenseLevel));
        this.SetMaximumHealth(
            GetValue(this.Context.Configuration.StatueLifeUpgrades, this.LifeLevel),
            preserveMissingHealth);
    }

    /// <summary>
    /// Executes one regeneration tick.
    /// </summary>
    /// <returns>The restored hit points.</returns>
    public int Regenerate()
    {
        if (!this.IsAlive || this.RegenLevel == 0 || this.Health >= this.MaximumHealth)
        {
            return 0;
        }

        var regenerationPercentage = GetValue(
            this.Context.Configuration.StatueRegenUpgrades,
            this.RegenLevel);
        var restored = Math.Max(1, this.MaximumHealth * regenerationPercentage / 100);
        var previousHealth = this.Health;
        this.Health = Math.Min(this.MaximumHealth, this.Health + restored);
        this.State.CurrentHp = this.Health;
        return this.Health - previousHealth;
    }

    private static int GetValue(IEnumerable<CastleSiegeUpgradeDefinition> definitions, byte level)
    {
        return definitions.FirstOrDefault(definition => definition.Level == level)?.Value
               ?? throw new InvalidOperationException($"Castle Siege statue upgrade level {level} is not configured.");
    }
}
