// <copyright file="MonsterAttributeScaler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A feature plugin which scales all monster base stats by a configurable percentage.
/// Uses multiplicative IElement instances applied per-monster. A server restart is
/// required for configuration changes to take effect.
/// </summary>
[PlugIn]
[Display(Name = "Monster Attribute Scaler", Description = "Increases all monster base stats by a configurable percentage.")]
[Guid("59E5B45F-0A3D-4BAF-8B6C-9E1D2F3A4B5C")]
public class MonsterAttributeScaler : IFeaturePlugIn, IObjectAddedToMapPlugIn, IObjectRemovedFromMapPlugIn,
    ISupportCustomConfiguration<MonsterAttributeScalerConfiguration>,
    ISupportDefaultCustomConfiguration,
    IDisabledByDefault
{
    private MonsterAttributeScalerConfiguration? _configuration;

    private readonly ConcurrentDictionary<IAttackable, List<(AttributeDefinition Attribute, IElement Element)>> _scaledMonsters = new();

    private readonly ConcurrentDictionary<int, byte> _loggedMonsterTypes = new();

    private static readonly HashSet<AttributeDefinition> ScalableStatsExceptHealth =
    [
        Stats.MinimumPhysBaseDmg,
        Stats.MaximumPhysBaseDmg,
        Stats.AttackRatePvm,
        Stats.DefenseRatePvm,
        Stats.DefenseBase,
    ];

    /// <inheritdoc />
    public MonsterAttributeScalerConfiguration? Configuration
    {
        get => this._configuration;
        set => this._configuration = value;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new MonsterAttributeScalerConfiguration();

    /// <summary>
    /// Applies configured scaling to newly spawned monsters.
    /// </summary>
    public ValueTask ObjectAddedToMapAsync(GameMap map, ILocateable addedObject)
    {
        if (addedObject is not AttackableNpcBase monster)
        {
            return ValueTask.CompletedTask;
        }

        if (this.Configuration is not { Percentage: > 0 } config)
        {
            return ValueTask.CompletedTask;
        }

        if (!this._scaledMonsters.ContainsKey(monster))
        {
            var multiplier = 1.0f + config.Percentage / 100.0f;
            this.ApplyScaling(monster, multiplier);

            if (this._loggedMonsterTypes.TryAdd(monster.Definition.Number, 0))
            {
                Console.WriteLine($"[MonsterAttributeScaler] Scaled monster type {monster.Definition.Number} by {config.Percentage}%");
            }
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Cleans up the tracking dictionary when a monster is removed from a map.
    /// </summary>
    public ValueTask ObjectRemovedFromMapAsync(GameMap map, ILocateable removedObject)
    {
        if (removedObject is IAttackable attackable)
        {
            this.RemoveScaling(attackable);
        }

        return ValueTask.CompletedTask;
    }

    private void ApplyScaling(IAttackable monster, float multiplier)
    {
        this.RemoveScaling(monster);

        var tracker = this._scaledMonsters.GetOrAdd(monster, _ => new List<(AttributeDefinition, IElement)>());

        foreach (var stat in ScalableStatsExceptHealth)
        {
            var element = new SimpleElement(multiplier, AggregateType.Multiplicate);
            monster.Attributes.AddElement(element, stat);
            tracker.Add((stat, element));
        }

        if (monster is AttackableNpcBase npc)
        {
            var healthElement = new SimpleElement(multiplier, AggregateType.Multiplicate);
            monster.Attributes.AddElement(healthElement, Stats.MaximumHealth);
            tracker.Add((Stats.MaximumHealth, healthElement));

            npc.Health = Math.Max(1, (int)(npc.Health * multiplier));
        }
    }

    private void RemoveScaling(IAttackable monster)
    {
        if (!this._scaledMonsters.TryRemove(monster, out var tracker))
        {
            return;
        }

        float healthPercentage = 0f;
        if (monster is AttackableNpcBase { IsAlive: true } npc)
        {
            var maxHealth = monster.Attributes[Stats.MaximumHealth];
            if (maxHealth > 0)
            {
                healthPercentage = (float)npc.Health / maxHealth;
            }
        }

        foreach (var (attribute, element) in tracker)
        {
            monster.Attributes.RemoveElement(element, attribute);
        }

        if (monster is AttackableNpcBase npc)
        {
            if (npc.IsAlive)
            {
                var newMaxHealth = monster.Attributes[Stats.MaximumHealth];
                npc.Health = Math.Max(1, (int)(newMaxHealth * healthPercentage));
            }
            else
            {
                npc.Health = 0;
            }
        }
    }
}
