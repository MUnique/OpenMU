// <copyright file="CastleSiegeNpcIntelligenceBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Base intelligence for stationary Castle Siege NPCs.
/// </summary>
public abstract class CastleSiegeNpcIntelligenceBase : INpcIntelligence
{
    private NonPlayerCharacter? _npc;

    /// <inheritdoc />
    public NonPlayerCharacter Npc
    {
        get => this._npc ?? throw Error.NotInitializedProperty(this);
        set => this._npc = value;
    }

    /// <inheritdoc />
    public bool CanWalkOnSafezone => false;

    /// <inheritdoc />
    public virtual void RegisterHit(IAttacker attacker)
    {
        // Castle Siege structures don't retaliate.
    }

    /// <inheritdoc />
    public virtual void Start()
    {
        // Most Castle Siege NPCs don't have autonomous behavior.
    }

    /// <inheritdoc />
    public virtual void Pause()
    {
        // Most Castle Siege NPCs don't have autonomous behavior.
    }

    /// <inheritdoc />
    public bool CanWalkOn(Point target) => false;
}
