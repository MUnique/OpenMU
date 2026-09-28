// <copyright file="CrywolfPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The plugin of the crywolf event, in which the players defend the crywolf fortress against the army of Balgass.
/// </summary>
/// <remarks>
/// The event starts at the configured times. The result of the event, the occupation state of the fortress,
/// is kept until the next event and saved in the database. It results in benefits or penalties on all game servers:
/// <list type="bullet">
///   <item>While the fortress is in peace after it has been defended, the maximum health of the monsters is reduced,
///     and the chaos machine mixes of the event tickets get an additional success rate.</item>
///   <item>While the fortress is occupied, the jewels drop less often, and the experience of killed monsters is reduced.</item>
/// </list>
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.CrywolfPlugIn_Name), Description = nameof(PlugInResources.CrywolfPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3E8B5C27-9D41-4A6F-B2C8-7F1D6E0A9B34")]
public sealed class CrywolfPlugIn : IFeaturePlugIn, IPeriodicTaskPlugIn, ISupportCustomConfiguration<CrywolfEventDefinition>, ISupportDefaultCustomConfiguration,
    IObjectAddedToMapPlugIn, IObjectRemovedFromMapPlugIn, IPlayerStateChangedPlugIn, IMonsterItemDropPlugIn, IDisposable
{
    /// <summary>
    /// The jewels which drop less often while the penalties apply, like in the original game.
    /// </summary>
    private static readonly HashSet<(byte Group, short Number)> PenaltyJewels = new()
    {
        (14, 13), // Jewel of Bless
        (14, 14), // Jewel of Soul
        (14, 16), // Jewel of Life
        (14, 22), // Jewel of Creation
        (12, 15), // Jewel of Chaos
        (14, 31), // Jewel of Guardian
    };

    private readonly ConcurrentDictionary<IGameContext, CrywolfContext> _contexts = new();
    private readonly ConcurrentDictionary<IGameContext, int> _runningTicks = new();

    /// <summary>
    /// The multiplier of the maximum health of the monsters. Like the occupation state, it's the same for all game servers.
    /// </summary>
    private readonly SimpleElement _monsterHealthMultiplier = new(1.0f, AggregateType.Multiplicate);

    /// <summary>
    /// The multiplier of the <see cref="Stats.ExperienceRate"/> and <see cref="Stats.MasterExperienceRate"/> of the players.
    /// </summary>
    private readonly SimpleElement _experienceMultiplier = new(1.0f, AggregateType.Multiplicate);

    private int _isUpdatingMultipliers;

    /// <inheritdoc />
    public CrywolfEventDefinition? Configuration { get; set; }

    /// <summary>
    /// Gets the context of the crywolf event of the game context, if it's running.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The context of the crywolf event.</returns>
    public static CrywolfContext? GetContext(IGameContext gameContext)
    {
        var plugIn = gameContext.FeaturePlugIns.GetPlugIn<CrywolfPlugIn>();
        return plugIn is not null && plugIn._contexts.TryGetValue(gameContext, out var context) ? context : null;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (this._runningTicks.GetOrAdd(gameContext, 0) != 0
            || !this._runningTicks.TryUpdate(gameContext, 1, 0))
        {
            return;
        }

        try
        {
            var definition = this.Configuration ??= new CrywolfEventDefinition();
            if (!this._contexts.TryGetValue(gameContext, out var context))
            {
                context = new CrywolfContext(gameContext, definition);
                await context.InitializeAsync().ConfigureAwait(false);
                this._contexts[gameContext] = context;
            }
            else
            {
                context.UpdateDefinition(definition);
            }

            await context.TickAsync().ConfigureAwait(false);
            await this.UpdateMultipliersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            gameContext.LoggerFactory.CreateLogger<CrywolfPlugIn>().LogError(ex, "Unexpected error in the crywolf event.");
        }
        finally
        {
            this._runningTicks[gameContext] = 0;
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        foreach (var context in this._contexts.Values)
        {
            context.SkipWaitingTime();
        }
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new CrywolfEventDefinition();
    }

    /// <inheritdoc />
    public ValueTask ObjectAddedToMapAsync(GameMap map, ILocateable addedObject)
    {
        if (addedObject is Monster monster && IsAffectedByHealthBenefit(monster))
        {
            monster.Attributes.AddElement(this._monsterHealthMultiplier, Stats.MaximumHealth);

            // The monster got its health before it was added to the map. When it respawns, it stays on the map and keeps the multiplier.
            monster.Health = (int)monster.Attributes[Stats.MaximumHealth];
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ObjectRemovedFromMapAsync(GameMap map, ILocateable removedObject)
    {
        if (removedObject is Monster monster && IsAffectedByHealthBenefit(monster))
        {
            monster.Attributes.RemoveElement(this._monsterHealthMultiplier, Stats.MaximumHealth);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (currentState.IsDisconnectedOrFinished())
        {
            player.Attributes?.RemoveElement(this._experienceMultiplier, Stats.ExperienceRate);
            player.Attributes?.RemoveElement(this._experienceMultiplier, Stats.MasterExperienceRate);
        }
        else if (previousState == PlayerState.CharacterSelection && currentState == PlayerState.EnteredWorld)
        {
            player.Attributes?.AddElement(this._experienceMultiplier, Stats.ExperienceRate);
            player.Attributes?.AddElement(this._experienceMultiplier, Stats.MasterExperienceRate);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void ItemDropping(AttackableNpcBase monster, Player killer, Item item, CancelEventArgs eventArgs)
    {
        if (item.Definition is { } definition
            && PenaltyJewels.Contains((definition.Group, definition.Number))
            && this._contexts.TryGetValue(killer.GameContext, out var context)
            && context.ArePenaltiesApplied
            && Rand.NextInt(0, 100) >= context.Definition.JewelDropPenaltyPercentage)
        {
            eventArgs.Cancel = true;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var context in this._contexts.Values)
        {
            context.Dispose();
        }

        this._contexts.Clear();
    }

    private static bool IsAffectedByHealthBenefit(Monster monster)
    {
        // Like in the original game, it applies to all monsters, but not to the summons of players.
        // Monsters with a fixed maximum health (e.g. the gates of blood castle) keep it.
        return monster.Definition.ObjectKind == NpcObjectKind.Monster
               && monster.SummonedBy is null
               && monster.SpawnArea.MaximumHealthOverride is null;
    }

    private async ValueTask UpdateMultipliersAsync()
    {
        if (Interlocked.Exchange(ref this._isUpdatingMultipliers, 1) != 0)
        {
            return;
        }

        try
        {
            // The occupation state is the same on all game servers, but the one which runs the event knows its changes first.
            var context = this._contexts.Values.FirstOrDefault(c => c.IsEventServer) ?? this._contexts.Values.FirstOrDefault();
            if (context is null)
            {
                return;
            }

            var experienceMultiplier = context.ExperienceMultiplier;
            if (Math.Abs(this._experienceMultiplier.Value - experienceMultiplier) > float.Epsilon)
            {
                this._experienceMultiplier.Value = experienceMultiplier;
            }

            var healthMultiplier = context.MonsterHealthMultiplier;
            var previousHealthMultiplier = this._monsterHealthMultiplier.Value;
            if (Math.Abs(previousHealthMultiplier - healthMultiplier) > float.Epsilon)
            {
                this._monsterHealthMultiplier.Value = healthMultiplier;
                if (healthMultiplier < previousHealthMultiplier)
                {
                    await this.LimitMonsterHealthAsync().ConfigureAwait(false);
                }
            }
        }
        finally
        {
            Volatile.Write(ref this._isUpdatingMultipliers, 0);
        }
    }

    /// <summary>
    /// Limits the health of the living monsters to their reduced maximum health.
    /// </summary>
    private async ValueTask LimitMonsterHealthAsync()
    {
        foreach (var gameContext in this._contexts.Keys)
        {
            foreach (var map in await gameContext.GetMapsAsync().ConfigureAwait(false))
            {
                foreach (var monster in map.GetNpcsInRange(new Point(128, 128), byte.MaxValue).OfType<Monster>())
                {
                    var maximumHealth = (int)monster.Attributes[Stats.MaximumHealth];
                    if (monster.Health > maximumHealth)
                    {
                        monster.Health = maximumHealth;
                    }
                }
            }
        }
    }
}
