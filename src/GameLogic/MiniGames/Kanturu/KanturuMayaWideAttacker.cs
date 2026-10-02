// <copyright file="KanturuMayaWideAttacker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// Runs Maya's wide area attack: broadcast, damage, and the pendant kill.
/// Every player is damaged and those without the Moonstone Pendant die outright.
/// </summary>
internal sealed class KanturuMayaWideAttacker
{
    private readonly GameMap _map;
    private readonly Func<Func<Player, Task>, ValueTask> _forEachPlayerAsync;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuMayaWideAttacker"/> class.
    /// </summary>
    /// <param name="map">The map of the event.</param>
    /// <param name="forEachPlayerAsync">Executes an action for each player.</param>
    /// <param name="logger">The logger.</param>
    public KanturuMayaWideAttacker(GameMap map, Func<Func<Player, Task>, ValueTask> forEachPlayerAsync, ILogger logger)
    {
        this._map = map;
        this._forEachPlayerAsync = forEachPlayerAsync;
        this._logger = logger;
    }

    /// <summary>
    /// Runs the attack periodically until cancelled, alternating between storm and
    /// stone rain rounds.
    /// </summary>
    /// <param name="interval">The interval between two rounds.</param>
    /// <param name="isPaused">Whether the attack is currently paused, e.g. during standby.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task RunAsync(TimeSpan interval, Func<bool> isPaused, CancellationToken cancellationToken)
    {
        var isStorm = true;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (!isPaused())
            {
                await this.ExecuteRoundAsync(isStorm).ConfigureAwait(false);
            }

            isStorm = !isStorm;
        }
    }

    /// <summary>
    /// Executes one round of the attack: alternating storm and stone rain animation,
    /// damage for every player, death for those without the Moonstone Pendant.
    /// </summary>
    /// <param name="showStorm">Whether to show the storm instead of the stone rain animation.</param>
    public async Task ExecuteRoundAsync(bool showStorm)
    {
        await this.BroadcastAsync(showStorm).ConfigureAwait(false);
        await this.ApplyDamageAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Finds the monster which deals the attack: a living Maya monster if there is one,
    /// otherwise any other living monster of the map.
    /// </summary>
    /// <param name="monsters">The monsters to choose from.</param>
    /// <returns>The attacker, or <c>null</c> when no monster is alive.</returns>
    internal static Monster? FindAttacker(IEnumerable<Monster> monsters)
    {
        Monster? fallback = null;
        foreach (var monster in monsters)
        {
            if (!monster.IsAlive)
            {
                continue;
            }

            if (monster.Definition is { } definition && KanturuEventDefinition.MayaMonsterNumbers.Contains(definition.Number))
            {
                return monster;
            }

            fallback ??= monster;
        }

        return fallback;
    }

    private ValueTask BroadcastAsync(bool showStorm)
    {
        return this._forEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                p.ShowMayaWideAreaAttackAsync(showStorm)).AsTask());
    }

    private async Task ApplyDamageAsync()
    {
        var attacker = FindAttacker(this._map.GetAttackablesInRange(KanturuContext.MapCenter, byte.MaxValue).OfType<Monster>());
        if (attacker is null)
        {
            return;
        }

        // Players finished below can't be killed inside ForEachPlayerAsync: it holds a
        // reader lock which the removal from the map would wait for as a writer.
        var withoutPendant = new ConcurrentBag<Player>();
        var requirements = this._map.Definition.MapRequirements;
        await this._forEachPlayerAsync(async player =>
        {
            if (!player.IsActive() || !player.IsAlive)
            {
                return;
            }

            await player.AttackByAsync(attacker, null, false).ConfigureAwait(false);

            if (player.IsAlive && requirements is { Count: > 0 }
                && KanturuRequiredItemHelper.GetRequiredItems(player, requirements).Count == 0)
            {
                withoutPendant.Add(player);
            }
        }).ConfigureAwait(false);

        await this.FinishWithoutPendantAsync(withoutPendant).ConfigureAwait(false);
    }

    private async Task FinishWithoutPendantAsync(ConcurrentBag<Player> players)
    {
        foreach (var player in players)
        {
            try
            {
                if (player.IsAlive)
                {
                    this._logger.LogInformation("Kanturu: {Player} has no Moonstone Pendant and is killed by Maya's wide area attack.", player);
                    await player.KillInstantlyAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Unexpected error when killing {Player} without the Moonstone Pendant.", player);
            }
        }
    }
}
