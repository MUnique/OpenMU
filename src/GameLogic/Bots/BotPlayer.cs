// <copyright file="BotPlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Threading;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// A connection-less bot player. It reuses the whole offline-player intelligence (combat, buffs,
/// healing, pickup) and adds a <see cref="BotNavigator"/> which makes it roam to level-appropriate
/// hunting grounds instead of standing on a fixed spawn position.
/// </summary>
public sealed class BotPlayer : OfflinePlayer
{
    /// <summary>
    /// After this many AI ticks failing in a row, the bot is considered broken and gets restarted.
    /// The engine's attribute system is not thread-safe, and a lost race can corrupt a character's
    /// attribute graph for good: every following tick throws, the bot stops playing and floods the log
    /// with the same exception until the server restarts. A fresh login rebuilds the graph and heals it,
    /// which is exactly what a player would do. The threshold is high enough that a single failing tick
    /// (a transient race, a monster which just died) is simply skipped, like before.
    /// </summary>
    private const int ConsecutiveFailuresUntilRestart = 20;

    private BotNavigator? _navigator;

    private int _consecutiveTickFailures;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotPlayer"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    public BotPlayer(IGameContext gameContext)
        : base(gameContext)
    {
    }

    /// <inheritdoc />
    public override bool RespawnAndContinue => true;

    /// <summary>
    /// Gets or sets a value indicating whether this bot evolved into its master class and still needs
    /// the "relog" which mounts the master attributes (see <see cref="BotManager.RestartBotAsync"/>).
    /// Set from the bot's own tick, where the evolution runs; acted upon by the maintenance pass, which
    /// is the only place allowed to restart a bot (a restart from within the bot's own timer callback
    /// would tear down the very loop it runs in).
    /// </summary>
    public bool AwaitsMasterRestart { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this bot's AI keeps failing and it has to be restarted
    /// (see <see cref="ConsecutiveFailuresUntilRestart"/>). Acted upon by the maintenance pass, which is
    /// the only place allowed to restart a bot.
    /// </summary>
    public bool AwaitsFaultRestart { get; set; }

    /// <inheritdoc />
    public override async ValueTask StopAsync()
    {
        await this.StopNavigatorAsync().ConfigureAwait(false);
        await base.StopAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    internal override void OnAiTickSucceeded()
    {
        if (this._consecutiveTickFailures > 0)
        {
            Interlocked.Exchange(ref this._consecutiveTickFailures, 0);
        }
    }

    /// <inheritdoc />
    internal override void OnAiTickFailed()
    {
        if (Interlocked.Increment(ref this._consecutiveTickFailures) == ConsecutiveFailuresUntilRestart)
        {
            // Deliberately '==', not '>=': this arms the restart exactly once, at the tick that crosses
            // the threshold, so the log gets one line and the flag is raised once. Further failures keep
            // incrementing the counter (which stays above the threshold) but don't re-fire; the
            // maintenance pass consumes AwaitsFaultRestart and the restart resets the counter to 0.
            this.Logger.LogWarning(
                "Bot '{Name}' failed {Count} AI ticks in a row and gets restarted to heal it.",
                this.Name,
                ConsecutiveFailuresUntilRestart);
            this.AwaitsFaultRestart = true;
        }
    }

    /// <inheritdoc />
    protected override void StartIntelligence()
    {
        base.StartIntelligence();
        this._navigator = new BotNavigator(this);
        this._navigator.Start();
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await this.StopNavigatorAsync().ConfigureAwait(false);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    private async ValueTask StopNavigatorAsync()
    {
        if (this._navigator is { } navigator)
        {
            this._navigator = null;
            await navigator.DisposeAsync().ConfigureAwait(false);
        }
    }
}
