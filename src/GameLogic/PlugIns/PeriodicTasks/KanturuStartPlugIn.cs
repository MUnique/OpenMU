// <copyright file="KanturuStartPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables the periodic start of the Kanturu Refinery Tower event.
/// </summary>
[PlugIn]
[Display(Name = nameof(KanturuStartPlugIn), Description = "Kanturu Refinery Tower event")]
[Guid("A8F3C2D1-9E74-4ECB-8963-08A3697278C4")]
public sealed class KanturuStartPlugIn : MiniGameStartBasePlugIn<KanturuStartConfiguration, KanturuGameServerState>
{
    /// <inheritdoc />
    public override MiniGameType Key => MiniGameType.Kanturu;

    /// <inheritdoc />
    public override object CreateDefaultConfig()
    {
        return KanturuStartConfiguration.Default;
    }

    /// <inheritdoc />
    public override async ValueTask DisposeRunningGamesAsync(IGameContext gameContext)
    {
        // A game master restart supersedes the tower window: clear it so the forced
        // start below isn't blocked by it. The regular schedule stays blocked.
        await KanturuTowerWindow.SetOpenUntilUtcAsync(
            gameContext,
            null,
            gameContext.LoggerFactory.CreateLogger(this.GetType())).ConfigureAwait(false);

        await base.DisposeRunningGamesAsync(gameContext).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override KanturuGameServerState CreateState(IGameContext gameContext)
    {
        return new KanturuGameServerState(gameContext);
    }

    /// <inheritdoc />
    protected override bool IsPreviousEventStillRunning(KanturuGameServerState state)
    {
        // While the tower window is open, the regular schedule must not start a new
        // event — not even when no game currently runs, e.g. after a server restart.
        // The window is read live, so this also works where no asynchronous call is possible.
        if (KanturuTowerWindow.GetOpenUntilUtc(state.Context) is { } until && until > DateTime.UtcNow)
        {
            return true;
        }

        // A hosted tower doesn't block the next run on its own; only the open window
        // does (see above). A leftover tower game is disposed in OnStartedAsync.
        return base.IsPreviousEventStillRunning(state)
            && state.Context.MiniGames.GetRunningMiniGames(MiniGameType.Kanturu)
                .Any(game => game is not KanturuContext tower || !tower.TowerMode);
    }

    /// <inheritdoc />
    protected override async ValueTask OnStartedAsync(KanturuGameServerState state)
    {
        // Hygiene for stale tower games, e.g. a tower whose window just expired while
        // its exit phases still run: the new event must not reuse their map.
        foreach (var game in state.Context.MiniGames.GetRunningMiniGames(MiniGameType.Kanturu))
        {
            if (game is KanturuContext { TowerMode: true })
            {
                await game.DisposeAsync().ConfigureAwait(false);
            }
        }

        await KanturuTowerWindow.SetOpenUntilUtcAsync(
            state.Context,
            null,
            state.Context.LoggerFactory.CreateLogger(this.GetType())).ConfigureAwait(false);

        await base.OnStartedAsync(state).ConfigureAwait(false);
    }
}
