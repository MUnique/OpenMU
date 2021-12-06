﻿// <copyright file="NullMonsterIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// The monster intelligence which does nothing.
/// </summary>
public sealed class NullMonsterIntelligence : INpcIntelligence
{
    private NonPlayerCharacter? _npc;

    /// <inheritdoc />
    public NonPlayerCharacter Npc
    {
        get => this._npc ?? throw Error.NotInitializedProperty(this);
        set => this._npc = value;
    }

    /// <inheritdoc />
    public void RegisterHit(IAttacker attacker)
    {
        // do nothing
    }

    /// <inheritdoc />
    public void Start()
    {
        // do nothing
    }
}