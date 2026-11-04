// <copyright file="GuardIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// An intelligence for guard NPCs.
/// </summary>
/// <remarks>
/// This behavior could be refined in the following way:
/// - When a player with active self defense against a PK talks to him, it could
///   set a follow mode on this player and remembers the PK as target.
/// - When the player walks back to the PK and the guard comes into range, the
///   guard stops following and starts attacking the PK.
/// - Follow mode is disabled after a timeout and the guard walks or teleports back.
/// </remarks>
public sealed class GuardIntelligence : BasicMonsterIntelligence
{
    private Point _spawnPoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuardIntelligence"/> class.
    /// </summary>
    public GuardIntelligence()
    {
        this.CanWalkOnSafezone = true;
    }

    /// <inheritdoc/>
    public override bool CanWalkOn(Point target)
    {
        return this.Monster.CurrentMap.Terrain.WalkMap[target.X, target.Y];
    }

    /// <inheritdoc />
    protected override async ValueTask<IAttackable?> SearchNextTargetAsync()
    {
        var currentMap = this.Npc.CurrentMap;
        var nextTarget = currentMap.GetAttackablesInRange(this.Npc.Position, this.Npc.Definition.ViewRange)
            .OfType<Player>()
            .Where(o => o is { SelectedCharacter.State: >= HeroState.PlayerKiller1stStage, IsAlive: true }
                && !o.IsAtSafezone())
            .MinBy(this.Npc.GetDistanceTo) as IAttackable;

        return nextTarget;
    }

    /// <inheritdoc />
    protected override void OnStart()
    {
        base.OnStart();

        if (this.Monster.SpawnArea.IsPoint())
        {
            this._spawnPoint = new Point(this.Monster.SpawnArea.X1, this.Monster.SpawnArea.Y1);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask TickWithoutTargetAsync()
    {
        if (Random.Shared.NextDouble() > 0.20f)
        {
            return;
        }

        if (this._spawnPoint != default
            && this._spawnPoint.EuclideanDistanceTo(this.Monster.Position) > 7)
        {
            // walk back towards the spawn point
            var middle = (this._spawnPoint / 2) + (this.Monster.Position / 2);
            var walkTarget = this.Monster.CurrentMap!.Terrain.GetRandomCoordinate(middle, 1);
            await this.Monster.WalkToAsync(walkTarget).ConfigureAwait(false);
            return;
        }

        // we move around randomly, so the monster does not look dead when watched from distance.
        if (await this.IsObservedByAttackerAsync().ConfigureAwait(false))
        {
            await this.Monster.RandomMoveAsync().ConfigureAwait(false);
        }
    }
}