// <copyright file="MiniGameRewardService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Collections.Concurrent;
using MUnique.OpenMU.DataModel.Statistics;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Gives the mini game rewards to the players and saves the ranking.
/// It also tracks which reward-relevant monsters have been killed, so that rewards which
/// require a kill can be evaluated.
/// </summary>
internal sealed class MiniGameRewardService
{
    private readonly MiniGameDefinition _definition;
    private readonly IGameContext _gameContext;
    private readonly GameMap _map;
    private readonly Func<IDropGenerator> _dropGeneratorProvider;
    private readonly Func<TimeSpan> _remainingTimeProvider;
    private readonly Func<Player?> _winnerProvider;
    private readonly ILogger _logger;
    private readonly object _owner;
    private readonly ConcurrentDictionary<MonsterDefinition, bool> _rewardRelatedKills;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniGameRewardService"/> class.
    /// </summary>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context, to which the game belongs.</param>
    /// <param name="map">The map on which the game takes place.</param>
    /// <param name="dropGeneratorProvider">A function which returns the currently used drop generator.</param>
    /// <param name="remainingTimeProvider">A function which returns the remaining time of the event.</param>
    /// <param name="winnerProvider">A function which returns the winner of the event, if any.</param>
    /// <param name="logger">The logger for this instance.</param>
    /// <param name="owner">The owner which is named in the log entries.</param>
    public MiniGameRewardService(
        MiniGameDefinition definition,
        IGameContext gameContext,
        GameMap map,
        Func<IDropGenerator> dropGeneratorProvider,
        Func<TimeSpan> remainingTimeProvider,
        Func<Player?> winnerProvider,
        ILogger logger,
        object owner)
    {
        this._definition = definition;
        this._gameContext = gameContext;
        this._map = map;
        this._dropGeneratorProvider = dropGeneratorProvider;
        this._remainingTimeProvider = remainingTimeProvider;
        this._winnerProvider = winnerProvider;
        this._logger = logger;
        this._owner = owner;
        this._rewardRelatedKills = new(
            this._definition.Rewards
                .Where(r => r.RequiredKill is not null)
                .Select(r => new KeyValuePair<MonsterDefinition, bool>(r.RequiredKill!, false))
                .Distinct());
    }

    /// <summary>
    /// Registers that a monster of the given definition has been killed.
    /// </summary>
    /// <param name="definition">The definition of the killed monster.</param>
    public void RegisterKill(MonsterDefinition definition)
    {
        this._rewardRelatedKills.TryUpdate(definition, true, false);
    }

    /// <summary>
    /// Gives the rewards to the player.
    /// </summary>
    /// <param name="player">The player who should receive the rewards.</param>
    /// <param name="rank">The rank of the player in the current game.</param>
    /// <returns>The bonus score and the given money.</returns>
    public async Task<(int BonusScore, int GivenMoney)> GiveRewardsAndGetBonusScoreAsync(Player player, int rank)
    {
        int bonusScore = 0;
        int givenMoney = 0;
        var rewards = this._definition.Rewards.Where(r => this.DoesRewardApply(player, rank, r));
        foreach (var reward in rewards)
        {
            var result = await this.GiveRewardAsync(player, reward).ConfigureAwait(false);
            bonusScore += result.BonusScore;
            givenMoney += result.GivenMoney;
        }

        return (bonusScore, givenMoney);
    }

    /// <summary>
    /// Saves the ranking of this game.
    /// </summary>
    /// <param name="scoreEntries">The entries of the ranking.</param>
    public async ValueTask SaveRankingAsync(IEnumerable<(int Rank, Character Character, int Score)> scoreEntries)
    {
        if (!this._definition.SaveRankingStatistics)
        {
            return;
        }

        try
        {
            using var context = this._gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(MiniGameRankingEntry), false, this._gameContext.Configuration);
            var instanceId = GuidV7.NewGuid();
            var timestamp = DateTime.UtcNow;
            foreach (var score in scoreEntries)
            {
                var entry = context.CreateNew<MiniGameRankingEntry>();
                entry.GameInstanceId = instanceId;
                entry.Rank = score.Rank;
                entry.Score = score.Score;
                entry.Character = score.Character;
                entry.MiniGame = this._definition;
                entry.Timestamp = timestamp;

                // todo: Consider "winning", too. E.g. in Chaos Castle a player which died last, might not be the winner, but is saved with rank 1.
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "{context}: Error while saving mini game ranking: {ex}", this._owner, ex);
        }
    }

    private async ValueTask<(int BonusScore, int GivenMoney)> GiveRewardAsync(Player player, MiniGameReward reward)
    {
        switch (reward.RewardType)
        {
            case MiniGameRewardType.Experience:
                await player.AddExperienceAsync(reward.RewardAmount, null).ConfigureAwait(false);
                break;
            case MiniGameRewardType.ExperiencePerRemainingSeconds:
                var seconds = (int)this._remainingTimeProvider().TotalSeconds;
                if (seconds > 0)
                {
                    await player.AddExperienceAsync(seconds * reward.RewardAmount, null).ConfigureAwait(false);
                }

                break;
            case MiniGameRewardType.Money:
                if (!player.TryAddMoney(reward.RewardAmount))
                {
                    await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.AwardMoneyFailByFullInventory)).ConfigureAwait(false);
                }

                return (0, reward.RewardAmount);
            case MiniGameRewardType.Item:
                await this.GiveItemRewardAsync(player, reward).ConfigureAwait(false);
                break;
            case MiniGameRewardType.ItemDrop:
                await this.GiveItemRewardAsync(player, reward, true).ConfigureAwait(false);
                break;
            case MiniGameRewardType.Score:
                return (reward.RewardAmount, 0);
            case MiniGameRewardType.Undefined:
                this._logger.LogWarning($"Undefined reward type in {reward.GetId()}");
                break;
            default:
                this._logger.LogError($"Reward type {reward.RewardType} in {reward.GetId()} is not implemented!");
                throw new NotImplementedException($"Reward type {reward.RewardType} is not implemented");
        }

        return (0, 0);
    }

    private async ValueTask GiveItemRewardAsync(Player player, MiniGameReward reward, bool drop = false)
    {
        if (reward.ItemReward is null)
        {
            this._logger.LogWarning("{context}: Item reward is not set in {reward}", this._owner, reward.GetId());
            return;
        }

        for (int i = 0; i < reward.RewardAmount; i++)
        {
            if (reward.ItemReward.Chance < 1
                && reward.ItemReward.Chance != 0 // If we don't add a chance (legacy), we assume that it's 1.
                && !Rand.NextRandomBool(reward.ItemReward.Chance))
            {
                this._logger.LogDebug("{context}: No item has been generated by reward {reward} for player {player} due to missed chance ({reward.ItemReward.Chance}).", this._owner, reward.GetId(), player, reward.ItemReward.Chance);
                continue;
            }

            var item = this._dropGeneratorProvider().GenerateItemDrop(reward.ItemReward);
            if (item is null)
            {
                this._logger.LogDebug("{context}: No item has been generated by reward {reward} for player {player}.", this._owner, reward.GetId(), player);
                return;
            }

            var droppedItem = new DroppedItem(item, player.RandomPosition, this._map, player, player.GetAsEnumerable());

            var shouldDrop = drop || !(player.Inventory is not null && await player.Inventory.AddItemAsync(item).ConfigureAwait(false));
            if (shouldDrop)
            {
                this._logger.LogDebug("{context}: Reward {item} for {player} has been dropped by players coordinates {position}.", this._owner, item, player, player.Position);
                await this._map.AddAsync(droppedItem).ConfigureAwait(false);
            }
        }
    }

    private bool DoesRewardApply(Player player, int playerRank, MiniGameReward reward)
    {
        var winner = this._winnerProvider();
        if (reward.Rank is not null && reward.Rank != playerRank)
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.Alive) && (!player.IsAlive || player.CurrentMap != this._map))
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.Dead) && (player.IsAlive && player.CurrentMap == this._map))
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.WinnerExists) && winner is null)
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.WinnerNotExists) && winner is not null)
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.Winner) && winner != player)
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.Loser)
            && (winner == player || (player.Party == winner?.Party && player.Party is not null)))
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.WinningParty)
            && (winner?.Party is null || winner.Party != player.Party))
        {
            return false;
        }

        if (reward.RequiredSuccess.HasFlag(MiniGameSuccessFlags.WinnerOrInWinningParty)
            && (winner?.Party is null || winner.Party != player.Party)
            && winner != player)
        {
            return false;
        }

        if (reward.RequiredKill is { } requiredKill
            && (!this._rewardRelatedKills.TryGetValue(requiredKill, out var killed)
                || !killed))
        {
            return false;
        }

        return true;
    }
}
