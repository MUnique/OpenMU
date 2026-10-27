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

    /// <summary>
    /// Gets the synchronization lock for thread-safe access to this player's state.
    /// </summary>
    public object Lock { get; } = new();

    /// <summary>
    /// Gets or sets the remaining attack tokens in the token bucket.
    /// </summary>
    public double AttackTokens { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when attack tokens were last updated.
    /// </summary>
    public long? LastAttackTokenUpdateTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last issued alert.
    /// </summary>
    public long? LastAlertTimestamp { get; set; }

    /// <summary>
    /// Gets the timestamps of recorded warnings within the history window.
    /// </summary>
    public Queue<long> AlertTimes { get; } = new();

    /// <summary>
    /// Gets the sliding window tracking executed walk step costs.
    /// </summary>
    public WalkSpeedWindow WalkWindow { get; } = new();

    /// <summary>
    /// Gets or sets the accumulated walk violation score.
    /// </summary>
    public double ViolationScore { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the violation score was last updated or decayed.
    /// </summary>
    public long? LastScoreUpdateTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last received ping packet.
    /// </summary>
    public long? LastPingTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the client tick count reported in the last ping packet.
    /// </summary>
    public uint? LastClientTickCount { get; set; }

    /// <summary>
    /// Resets the walk speed sliding window.
    /// </summary>
    public void ResetWalk()
    {
        this.WalkWindow.Clear();
    }
}
