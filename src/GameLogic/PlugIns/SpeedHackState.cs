// <copyright file="SpeedHackState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Collections.Generic;

/// <summary>
/// The per-player state shared by the walk, attack and ping checks.
/// </summary>
internal sealed class SpeedHackState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeedHackState"/> class.
    /// </summary>
    /// <param name="maxAttackTokens">The initial number of attack tokens.</param>
    public SpeedHackState(double maxAttackTokens)
    {
        this.AttackTokens = maxAttackTokens;
    }

    public object Lock { get; } = new();

    public double AttackTokens { get; set; }

    public long? LastAttackTokenUpdateTimestamp { get; set; }

    public long? LastAlertTimestamp { get; set; }

    public Queue<long> AlertTimes { get; } = new();

    public WalkSpeedWindow WalkWindow { get; } = new();

    public double ViolationScore { get; set; }

    public long? LastScoreUpdateTimestamp { get; set; }

    public long? LastPingTimestamp { get; set; }

    public uint? LastClientTickCount { get; set; }

    public void ResetWalk()
    {
        this.WalkWindow.Clear();
    }
}
