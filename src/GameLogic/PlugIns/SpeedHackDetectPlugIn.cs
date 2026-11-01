// <copyright file="SpeedHackDetectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A feature plugin that provides configuration and acts as a trigger control for speedhack anti-cheat checks.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectPlugIn_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectPlugIn_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
[Guid("A95A8D2F-A0C3-442E-995C-005B5C1B42D2")]
public class SpeedHackDetectPlugIn : IFeaturePlugIn, ISupportCustomConfiguration<SpeedHackDetectConfiguration>, ISupportDefaultCustomConfiguration, ISpeedHackCheatCheckPlugIn, IAttackSpeedCheckPlugIn, IPingCheckPlugIn, IWalkRequestValidationPlugIn, IWalkStepPlugIn, IMovementStateResetPlugIn
{
    private readonly ConditionalWeakTable<Player, SpeedHackState> _playerStates = new();
    private readonly TimeProvider _timeProvider;
    private readonly WalkSpeedCheck _walkCheck;
    private readonly AttackSpeedCheck _attackCheck;
    private readonly PingCheck _pingCheck;
    private readonly SpeedHackViolationRecorder _violationRecorder;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeedHackDetectPlugIn"/> class.
    /// </summary>
    public SpeedHackDetectPlugIn()
        : this(TimeProvider.System)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeedHackDetectPlugIn"/> class.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    internal SpeedHackDetectPlugIn(TimeProvider timeProvider)
    {
        this._timeProvider = timeProvider;
        this._walkCheck = new WalkSpeedCheck(timeProvider);
        this._attackCheck = new AttackSpeedCheck(timeProvider);
        this._pingCheck = new PingCheck(timeProvider);
        this._violationRecorder = new SpeedHackViolationRecorder(timeProvider);
    }

    /// <inheritdoc/>
    public SpeedHackDetectConfiguration? Configuration { get; set; }

    /// <inheritdoc/>
    public object CreateDefaultConfig() => new SpeedHackDetectConfiguration();

    /// <inheritdoc/>
    public async ValueTask ValidateWalkRequestAsync(Player player, Memory<WalkingStep> steps, WalkRequestValidationArgs eventArgs)
    {
        var speedHackArgs = new SpeedHackCheckEventArgs();
        await this.WalkCheatCheckAsync(player, steps, speedHackArgs).ConfigureAwait(false);

        // Soft violations are only recorded; just a Reject blocks the walk.
        eventArgs.IsWalkAllowed = speedHackArgs.Severity is not SpeedHackViolationSeverity.Reject;
    }

    /// <inheritdoc/>
    public ValueTask ResetMovementStateAsync(Player player)
    {
        this._walkCheck.Reset(this.GetState(player));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public void WalkStepPerformed(Player player, WalkingStep step, TimeSpan stepDelay)
    {
        if (IsServerControlled(player) || this.Configuration is not { } config)
        {
            return;
        }

        this._walkCheck.RecordStep(player, this.GetState(player), config, step, stepDelay);
    }

    /// <summary>
    /// Checks the walk speed and records a violation, if the walk has to be rejected.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="steps">The steps requested by the player.</param>
    /// <param name="eventArgs">The event args to mark a violation.</param>
    public async ValueTask WalkCheatCheckAsync(Player player, Memory<WalkingStep> steps, SpeedHackCheckEventArgs eventArgs)
    {
        if (steps.IsEmpty || IsServerControlled(player))
        {
            return;
        }

        var config = this.Configuration;
        if (config is null)
        {
            return;
        }

        var state = this.GetState(player);
        var violationSeverity = SpeedHackViolationSeverity.None;
        var positionMismatchSeverity = SpeedHackViolationSeverity.None;
        var startPoint = steps.Span[0].From;
        lock (state.Lock)
        {
            if (player.IsAtSafezone())
            {
                state.ResetWalk();
            }
            else
            {
                var walkEvaluation = this._walkCheck.Evaluate(player, config, state, this._timeProvider.GetTimestamp());
                violationSeverity = walkEvaluation.Severity;
                if (!walkEvaluation.IsViolation)
                {
                    positionMismatchSeverity = this._walkCheck.PenalizePositionMismatch(player, config, state, startPoint.EuclideanDistanceTo(player.Position));
                }
            }
        }

        if (violationSeverity is not SpeedHackViolationSeverity.None)
        {
            eventArgs.Severity = violationSeverity;
            await this._violationRecorder.RecordAsync(player, state, config, violationSeverity).ConfigureAwait(false);
        }
        else if (positionMismatchSeverity is not SpeedHackViolationSeverity.None)
        {
            // PlayerMovement validates the offset independently and resynchronizes the client
            // when it's too large. The mismatch still contributes to warnings and punishments.
            await this._violationRecorder.RecordAsync(player, state, config, positionMismatchSeverity).ConfigureAwait(false);
        }
        else
        {
            // Neither the walk pace check nor the position mismatch recorded a violation.
        }
    }

    /// <inheritdoc/>
    public async ValueTask AttackCheatCheckAsync(Player player, Skill? skill, SpeedHackCheckEventArgs eventArgs)
    {
        if (player.Attributes is null || IsServerControlled(player))
        {
            return;
        }

        var config = this.Configuration;
        if (config is null)
        {
            return;
        }

        var state = this.GetState(player);
        var severity = this._attackCheck.Evaluate(player, config, state, skill);
        if (severity is SpeedHackViolationSeverity.None)
        {
            return;
        }

        eventArgs.Severity = severity;
        await this._violationRecorder.RecordAsync(player, state, config, severity).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask AttackCheatCheckAsync(Player player, SpeedHackCheckEventArgs eventArgs) =>
        this.AttackCheatCheckAsync(player, null, eventArgs);

    /// <inheritdoc/>
    public ValueTask PingCheatCheckAsync(Player player, uint tickCount, ushort attackSpeed)
    {
        if (IsServerControlled(player) || this.Configuration is not { } config)
        {
            return ValueTask.CompletedTask;
        }

        this._pingCheck.Evaluate(player, config, this.GetState(player), tickCount, attackSpeed);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Gets the warning count for the player (used in diagnostics/testing).
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The warning count.</returns>
    public int GetWarningCount(Player player) => this._violationRecorder.GetWarningCount(this.GetState(player));

    /// <summary>
    /// Clears the alert debounce for the player (used in diagnostics/testing).
    /// </summary>
    /// <param name="player">The player.</param>
    public void ResetAlertDebounce(Player player) => this._violationRecorder.ResetDebounce(this.GetState(player));

    /// <summary>
    /// Gets the current violation score for the player (used in diagnostics/testing).
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The current violation score.</returns>
    internal double GetViolationScore(Player player)
    {
        var state = this.GetState(player);
        lock (state.Lock)
        {
            return state.ViolationScore;
        }
    }

    /// <summary>
    /// Determines whether the player's actions originate on the server instead of a game client.
    /// These checks validate what a client claims about its own timing, so there is nothing to
    /// validate for an offline player: the server itself paces its walks and attacks. Its MU Helper
    /// tick performs the same work cycle as the original client helper (recover, then attack), which
    /// draws two attack tokens from one 500 ms tick and eventually empties the bucket - so leaving
    /// these players in would only ever produce false positives, and with the default configuration
    /// those are answered with a persisted account ban.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns><c>true</c> if the player is controlled by the server; otherwise, <c>false</c>.</returns>
    private static bool IsServerControlled(Player player) => player is Offline.OfflinePlayer;

    private SpeedHackState GetState(Player player)
    {
        return this._playerStates.GetValue(player, p => new SpeedHackState(this.Configuration?.MaxAttackTokens ?? 5.0));
    }
}
