// <copyright file="SoccerBall.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// The implementation of a soccer ball.
/// They can be attacked, but take no damage. Instead, they move into the direction in which they got hit.
/// </summary>
public sealed class SoccerBall : NonPlayerCharacter, IAttackable, IMovable
{
    private readonly AsyncLock _moveLock = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="SoccerBall"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map on which this instance will spawn.</param>
    public SoccerBall(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map)
        : base(spawnInfo, stats, map)
    {
        this.MagicEffectList = new MagicEffectsList(this);
        this.Attributes = new AttributeSystem(Enumerable.Empty<IAttribute>(), Enumerable.Empty<IAttribute>(), Enumerable.Empty<AttributeRelationship>());
    }

    /// <summary>
    /// Occurs when it has been moved.
    /// </summary>
    public event AsyncEventHandler<(Point From, Point To)>? Moved;

    /// <inheritdoc />
    public event EventHandler<DeathInformation>? Died;

    /// <inheritdoc />
    public IAttributeSystem Attributes { get; }

    /// <inheritdoc />
    public MagicEffectsList MagicEffectList { get; }

    /// <inheritdoc />
    public bool IsAlive => true;

    /// <inheritdoc />
    public bool IsTeleporting => false;

    /// <inheritdoc />
    public DeathInformation? LastDeath => null;

    /// <inheritdoc />
    public async ValueTask AttackByAsync(IAttacker attacker, SkillEntry? skill, bool isCombo, double damageFactor = 1.0)
    {
        var direction = attacker.GetDirectionTo(this);
        await this.MoveToDirectionAsync(direction, skill is { }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask ReflectDamageAsync(IAttacker reflector, uint damage)
    {
        // A ball doesn't attack, so it doesn't reflect.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage)
    {
        // A ball doesn't take any damage
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask KillInstantlyAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async ValueTask MoveAsync(Point target)
    {
        var old = this.Position;
        await this.CurrentMap.MoveAsync(this, target, this._moveLock, MoveType.Instant).ConfigureAwait(false);
        if (this.Moved is { } eventHandler)
        {
            await eventHandler((From: old, To: target)).ConfigureAwait(false);
        }
    }

    private async ValueTask MoveToDirectionAsync(Direction direction, bool withSkill)
    {
        var terrain = this.CurrentMap.Terrain;
        var range = withSkill ? 3 : 2;
        var finalTarget = this.Position;
        for (int i = 0; i < range; i++)
        {
            var target = finalTarget.CalculateTargetPoint(direction);
            if (terrain.AIgrid[target.X, target.Y] == 1)
            {
                finalTarget = target;
            }
            else
            {
                break;
            }
        }

        if (finalTarget != this.Position)
        {
            await this.MoveAsync(finalTarget).ConfigureAwait(false);
        }
    }
}