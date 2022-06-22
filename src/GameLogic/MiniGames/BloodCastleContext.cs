// <copyright file="BloodCastleContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using Nito.Disposables.Internals;

/// <summary>
/// The context of a blood castle game.
/// </summary>
/// <remarks>
/// A blood castle event works like that:
///   First, a player or his party (and probably another party), maximum 10 players, enter the blood castle.
///   The game has several states:
///   * After the event starts, first a certain amount of monsters have to be killed, so that a bridge appears on the way to the castle gate.
///   * The castle gate has to be destroyed
///   * Some stronger monsters wait after the castle gate. A certain amount of "Spirit Sorcerer" have to be killed.
///   * The statue appears, which needs to be killed.
///   * The statue drops an archangel weapon as quest item. This item has to be brought back to the archangel NPC.
/// The game has a time limit of usually 15 or 20 minutes.
/// After the time is up, or the quest item has been brought back, the players get some rewards:
/// 
/// Experience:
///   1. For each remaining second, a bonus experience is given as experience.
///   2. The player or party which destroyed the gate, gets extra experience. Dead party members get the half of the exp as bonus.
///   3. For killing the statue, the player/party gets a bonus experience.
///   4. For finishing the quest, the player/party gets a bonus experience.
///   5. To all previous exp rewards, bonuses from seals, maps etc. are applied.
/// Money:
///   According to the reward table - fixed values per blood castle level, depending if the player was in the winning party, or not.
///   The winner/winners party get roughly the double money value.
/// Score:
///   a) If the event was won by any participant, depending on the individual success state of a player,
///      it will get a different score rewarded. A player is categorized into these 5 states:
///      - unfinished event
///      - died during event
///      - winner
///      - member of winners party
///      - member of winners party, but died during event.
///   b) If the event wasn't won by any participant, players are getting a score penalty of 300.
/// </remarks>
public sealed class BloodCastleContext : MiniGameContext
{
    private const short CastleGateNumber = 131;
    private const short StatueOfSaintNumber = 132;

    private readonly ConcurrentDictionary<string, PlayerGameState> _gameStates = new ();

    private IReadOnlyCollection<(string Name, int Score, int BonusExp, int BonusMoney)>? _highScoreTable;
    private TimeSpan _remainingTime;

    private bool _gateDestroyed;

    private Player? _winner;
    private Player? _questItemOwner;
    private Item? _questItem;

    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastleContext"/> class.
    /// </summary>
    /// <param name="key">The key of this context.</param>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context, to which this game belongs.</param>
    /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
    public BloodCastleContext(MiniGameMapKey key, MiniGameDefinition definition, IGameContext gameContext, IMapInitializer mapInitializer)
        : base(key, definition, gameContext, mapInitializer)
    {
    }

    /// <inheritdoc />
    protected override Player? Winner => this._winner;

    /// <inheritdoc />
    protected override TimeSpan RemainingTime => this._remainingTime;

    /// <summary>
    /// Player interact with Archangel.
    /// </summary>
    /// <param name="player">The player who talks to Archangel.</param>
    /// <remarks>
    /// TODO: Replace magic values (category/dialog numbers) with constants or enums.
    /// </remarks>
    public void TalkToNpcArchangel(Player player)
    {
        if (this._winner is not null)
        {
            player.ViewPlugIns.GetPlugIn<IShowDialogPlugIn>()?.ShowDialog(1, 0x2E);
            return;
        }

        if (!this.IsEventRunning)
        {
            player.ViewPlugIns.GetPlugIn<IShowDialogPlugIn>()?.ShowDialog(1, 0x18);
            return;
        }

        if (!this.TryRemoveQuestItemFromPlayer(player))
        {
            player.ViewPlugIns.GetPlugIn<IShowDialogPlugIn>()?.ShowDialog(1, 0x18);
            return;
        }

        this._winner = player;
        player.ViewPlugIns.GetPlugIn<IShowDialogPlugIn>()?.ShowDialog(1, 0x17);
        this.FinishEvent();
    }

    /// <inheritdoc/>
    protected override void OnObjectRemovedFromMap(object? sender, (GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            if (this.IsEventRunning)
            {
                // Drop it, so that the remaining players can pick it up.
                this.TryDropQuestItemFromPlayer(player);
            }
            else
            {
                this.TryRemoveQuestItemFromPlayer(player);
            }

            this.UpdateState(BloodCastleStatus.Ended, player);
        }

        base.OnObjectRemovedFromMap(sender, args);
    }

    /// <inheritdoc />
    protected override void OnDestructibleDied(object? sender, DeathInformation e)
    {
        base.OnDestructibleDied(sender, e);
        var destructible = sender as Destructible;
        if (destructible is null)
        {
            return;
        }

        if (destructible.Definition.Number == StatueOfSaintNumber)
        {
            this.ForEachPlayerAsync(player =>
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?
                    .ShowMessage(e.KillerName + " has destroyed the Crystal Statue!", Interfaces.MessageType.GoldenCenter);
            }).ConfigureAwait(false);
        }
        else if (destructible.Definition.Number == CastleGateNumber)
        {
            this._gateDestroyed = true;
        }
        else
        {
            // we don't have others, so nothing to do
        }
    }

    /// <inheritdoc/>
    protected override void OnPlayerPickedUpItem(object? sender, ILocateable e)
    {
        var picker = sender as Player;

        if (e is DroppedItem { Item.Definition: { } definition } && definition.IsArchangelQuestItem())
        {
            this._questItemOwner = picker;
            _ = Task.Run(() => this.ForEachPlayerAsync(player =>
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?
                    .ShowMessage(picker!.Name + " has acquired the " + definition.Name, Interfaces.MessageType.GoldenCenter);
            }));
        }
    }

    /// <inheritdoc />
    protected override void OnGameStart(ICollection<Player> players)
    {
        foreach (var player in players)
        {
            this._gameStates.TryAdd(player.Name, new PlayerGameState(player));
        }

        _ = Task.Run(async () => await this.ShowRemainingTimeLoopAsync(this.GameEndedToken), this.GameEndedToken);
        base.OnGameStart(players);
    }

    /// <inheritdoc />
    protected override void GameEnded(ICollection<Player> finishers)
    {
        this.UpdateState(BloodCastleStatus.Ended);

        var sortedFinishers = finishers
            .Select(f => this._gameStates[f.Name])
            .WhereNotNull()
            .OrderBy(state => state.Score)
            .ToList();

        var scoreList = new List<(string Name, int Score, int BonusExp, int BonusMoney)>();
        int rank = 0;
        foreach (var state in sortedFinishers)
        {
            rank++;
            state.Rank = rank;
            var (bonusScore, givenMoney) = this.GiveRewardsAndGetBonusScore(state.Player, rank);
            state.AddScore(bonusScore);

            scoreList.Add((
                state.Player.Name,
                state.Score,
                this.Definition.Rewards.FirstOrDefault(r => r.RewardType == MiniGameRewardType.Experience && (r.Rank is null || r.Rank == rank))?.RewardAmount ?? 0,
                givenMoney));

            this.TryRemoveQuestItemFromPlayer(state.Player);
        }

        this._highScoreTable = scoreList.AsReadOnly();

        this.SaveRanking(sortedFinishers.Select(state => (state.Rank, state.Player.SelectedCharacter!, state.Score)));
        base.GameEnded(finishers);
    }

    /// <inheritdoc />
    protected override void ShowScore(Player player)
    {
        if (this._highScoreTable is { } table)
        {
            var isSuccessful = this._winner is not null;
            var (name, score, bonusMoney, bonusExp) = table.First(t => t.Name == player.Name);
            player.ViewPlugIns.GetPlugIn<IBloodCastleScoreTableViewPlugin>()?.ShowScoreTable(isSuccessful, name, score, bonusExp, bonusMoney);
        }
    }

    /// <inheritdoc />
    protected override void OnItemDroppedOnMap(DroppedItem item)
    {
        base.OnItemDroppedOnMap(item);
        if (item.Item.Definition.IsArchangelQuestItem())
        {
            this._questItem = item.Item;
        }
    }

    private async ValueTask ShowRemainingTimeLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            var timerInterval = TimeSpan.FromSeconds(1);
            using var timer = new PeriodicTimer(timerInterval);
            var maximumGameDuration = this.Definition.GameDuration;
            this._remainingTime = maximumGameDuration;

            this.UpdateState(BloodCastleStatus.Started);
            while (!cancellationToken.IsCancellationRequested
                   && this._remainingTime >= TimeSpan.Zero
                   && await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                if (this._remainingTime < maximumGameDuration && !this._gateDestroyed)
                {
                    this.UpdateState(BloodCastleStatus.GateNotDestroyed);
                }

                if (this._remainingTime < maximumGameDuration && this._gateDestroyed)
                {
                    this.UpdateState(BloodCastleStatus.GateDestroyed);
                }

                this._remainingTime = this._remainingTime.Subtract(timerInterval);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected exception when the game ends before running into the timeout.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error during update blood castle status: {0}", ex.Message);
        }
    }

    private void UpdateState(BloodCastleStatus status)
    {
        _ = Task.Run(() => this.ForEachPlayerAsync(player => this.UpdateState(status, player)));
    }

    private void UpdateState(BloodCastleStatus status, Player player)
    {
        player.ViewPlugIns.GetPlugIn<IBloodCastleStateViewPlugin>()?
            .UpdateState(
                status,
                this._remainingTime,
                this.NextEvent?.RequiredKills ?? 0,
                this.NextEvent?.ActualKills ?? 0,
                this._questItemOwner,
                this._questItem);
    }

    private bool TryRemoveQuestItemFromPlayer(Player player)
    {
        if (!player.TryGetQuestItem(out var item))
        {
            return false;
        }

        player.Inventory!.RemoveItem(item);
        player.PersistenceContext.Delete(item);
        player.ViewPlugIns.GetPlugIn<IItemRemovedPlugIn>()?.RemoveItem(item.ItemSlot);

        this._questItem = null;
        this._questItemOwner = null;

        return true;
    }

    private void TryDropQuestItemFromPlayer(Player player)
    {
        if (!player.TryGetQuestItem(out var item))
        {
            return;
        }

        var dropped = new DroppedItem(item, player.Position, this.Map, player);
        this.Map.Add(dropped);
        player.Inventory!.RemoveItem(item);
        player.ViewPlugIns.GetPlugIn<IItemDropResultPlugIn>()?.ItemDropResult(item.ItemSlot, true);

        this._questItemOwner = null;
    }

    private sealed class PlayerGameState
    {
        private int _score;

        public PlayerGameState(Player player)
        {
            if (player.SelectedCharacter?.CharacterClass is null)
            {
                throw new InvalidOperationException($"The player '{player}' is in the wrong state");
            }

            this.Player = player;
        }

        public Player Player { get; }

        public int Score => this._score;

        public int Rank { get; set; }

        public void AddScore(int value)
        {
            Interlocked.Add(ref this._score, value);
        }
    }
}