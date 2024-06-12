// <copyright file="GateNpc.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using System.Threading;

/// <summary>
/// Represents a <see cref="NonPlayerCharacter"/> which is a gate to another map.
/// When a player gets close to the gate, it will warp the player to the target gate,
/// if the player is in the same party as the summoner.
/// </summary>
public class GateNpc : NonPlayerCharacter, ISummonable
{
    private const int Range = 2;
    private readonly ILogger<GateNpc> _logger;
    private readonly Task _warpTask;
    private readonly CancellationTokenSource _cts;
    private int _enterCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="GateNpc" /> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map.</param>
    /// <param name="summonedBy">The summoned by.</param>
    /// <param name="targetGate">The target gate.</param>
    /// <param name="lifespan">The lifespan.</param>
    public GateNpc(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, Player summonedBy, ExitGate targetGate, TimeSpan lifespan)
        : base(spawnInfo, stats, map)
    {
        this.SummonedBy = summonedBy;
        this.TargetGate = targetGate;
        this._logger = summonedBy.GameContext.LoggerFactory.CreateLogger<GateNpc>();
        this._cts = new CancellationTokenSource(lifespan);
        this._warpTask = Task.Run(this.RunTaskAsync);
    }

    /// <inheritdoc />
    public Player SummonedBy { get; }

    /// <summary>
    /// Gets the target gate.
    /// </summary>
    public ExitGate TargetGate { get; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        this._cts.Cancel();
        this._cts.Dispose();
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await this._cts.CancelAsync().ConfigureAwait(false);
        this._cts.Dispose();

        await this._warpTask.ConfigureAwait(false);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    private async Task RunTaskAsync()
    {
        try
        {
            var cancellationToken = this._cts.Token;
            var playersInRange = new List<Player>();
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);

                if (!this.SummonedBy.IsAlive)
                {
                    this._logger.LogInformation("Closing the gate, player is dead or offline");
                    return;
                }

                playersInRange.Clear();
                if (this.SummonedBy.Party is { } party)
                {
                    playersInRange.AddRange(party.PartyList.OfType<Player>().Where(this.IsPlayerInRange));
                }
                else if (this.IsPlayerInRange(this.SummonedBy))
                {
                    playersInRange.Add(this.SummonedBy);
                }
                else
                {
                    // do nothing.
                }

                foreach (var player in playersInRange)
                {
                    this._logger.LogInformation("Player {player} passes the gate ({enterCount})", player, this._enterCount);
                    await player.WarpToAsync(this.TargetGate).ConfigureAwait(false);
                    this._enterCount++;
                    if (this._enterCount >= this.SummonedBy.GameContext.Configuration.MaximumPartySize)
                    {
                        this._logger.LogInformation("Closing the gate, maximum entrances reached ({enterCount})", this._enterCount);
                        return;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error in GateNpc task.");
        }
        finally
        {
            await this.DisposeAsync().ConfigureAwait(false);
        }
    }

    private bool IsPlayerInRange(Player player)
    {
        return player.IsActive() && player.IsInRange(this, Range) && this.CurrentMap == player.CurrentMap;
    }
}