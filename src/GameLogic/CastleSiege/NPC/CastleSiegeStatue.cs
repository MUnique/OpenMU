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
        this.SetDefense(
            this.Context.Configuration.GetUpgrades(MonsterNumber, CastleSiegeUpgradeType.Defense)?.GetValue(this.DefenseLevel)
            ?? throw new InvalidOperationException($"Castle Siege statue defense level {this.DefenseLevel} is not configured."));
        this.SetMaximumHealth(
            this.Context.Configuration.GetUpgrades(MonsterNumber, CastleSiegeUpgradeType.Life)?.GetValue(this.LifeLevel)
            ?? throw new InvalidOperationException($"Castle Siege statue life level {this.LifeLevel} is not configured."),
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

        var regenerationPercentage = this.Context.Configuration
            .GetUpgrades(MonsterNumber, CastleSiegeUpgradeType.Regen)
            ?.GetValue(this.RegenLevel)
            ?? throw new InvalidOperationException($"Castle Siege statue regeneration level {this.RegenLevel} is not configured.");
        var restored = Math.Max(1, this.MaximumHealth * regenerationPercentage / 100);
        restored = this.RestoreHealth(restored, this.MaximumHealth);

        // There is no standalone structure-heal packet. Combat updates the client on the next hit, while the
        // management interface receives the current value with its next structure-list response. The periodic
        // NPC snapshot copies the atomic health value into the persisted state.
        return restored;
    }
}
