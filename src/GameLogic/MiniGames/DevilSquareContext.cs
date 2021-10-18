// <copyright file="DevilSquareContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using Nito.Disposables.Internals;

    /// <summary>
    /// The context of a devil square game.
    /// </summary>
    public sealed class DevilSquareContext : MiniGameContext
    {
        private readonly ConcurrentDictionary<string, PlayerGameState> gameStates = new ();

        private IReadOnlyCollection<(string Name, int Score, int BonusMoney, int BonusExp)>? highScoreTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevilSquareContext"/> class.
        /// </summary>
        /// <param name="key">The key of this context.</param>
        /// <param name="definition">The definition of the mini game.</param>
        /// <param name="gameContext">The game context, to which this game belongs.</param>
        /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
        public DevilSquareContext(MiniGameMapKey key, MiniGameDefinition definition, IGameContext gameContext, IMapInitializer mapInitializer)
            : base(key, definition, gameContext, mapInitializer)
        {
        }

        /// <inheritdoc />
        protected override void OnMonsterDied(object? sender, DeathInformation e)
        {
            base.OnMonsterDied(sender, e);

            if (this.gameStates.TryGetValue(e.KillerName, out var state))
            {
                state.AddScore(this.Definition.GameLevel);
                // todo add money? -> in the original servers, in DS drops no money!
            }
        }

        /// <inheritdoc />
        protected override void OnGameStart(ICollection<Player> players)
        {
            foreach (var player in players)
            {
                this.gameStates.TryAdd(player.Name, new PlayerGameState(player));
            }

            base.OnGameStart(players);
        }

        /// <inheritdoc />
        protected override void GameEnded(ICollection<Player> finishers)
        {
            var sortedFinishers = finishers
                .Select(f => this.gameStates[f.Name])
                .WhereNotNull()
                .OrderBy(state => state.Score)
                .ToList();

            var scoreList = new List<(string Name, int Score, int BonusMoney, int BonusExp)>();
            int rank = 0;
            foreach (var state in sortedFinishers)
            {
                rank++;
                state.Rank = rank;
                this.GiveRewards(state.Player, rank);
                scoreList.Add((
                    state.Player.Name,
                    state.Score,
                    this.Definition.Rewards.FirstOrDefault(r => r.RewardType == MiniGameRewardType.Money && (r.Rank is null || r.Rank == rank))?.RewardAmount ?? 0,
                    this.Definition.Rewards.FirstOrDefault(r => r.RewardType == MiniGameRewardType.Experience && (r.Rank is null || r.Rank == rank))?.RewardAmount ?? 0));
            }

            this.highScoreTable = scoreList.AsReadOnly();

            this.SaveRanking(sortedFinishers.Select(state => (Rank: state.Rank, state.Player.SelectedCharacter!, state.Score)));
            base.GameEnded(finishers);
        }

        /// <inheritdoc />
        protected override void ShowScore(Player player)
        {
            if (this.highScoreTable is { } table
                && this.gameStates.TryGetValue(player.Name, out var state))
            {
                player.ViewPlugIns.GetPlugIn<IMiniGameScoreTableViewPlugin>()?.ShowScoreTable((byte)state.Rank, table);
            }
        }

        private sealed class PlayerGameState
        {
            private readonly Character character;
            private readonly long experienceAtStart;
            private readonly int moneyAtStart;
            private int score;

            public PlayerGameState(Player player)
            {
                if (player.SelectedCharacter?.CharacterClass is null)
                {
                    throw new InvalidOperationException($"The player '{player}' is in the wrong state");
                }

                this.Player = player;
                this.character = player.SelectedCharacter;
                this.experienceAtStart = this.character.CharacterClass.IsMasterClass ? this.character.MasterExperience : this.character.Experience;
                this.moneyAtStart = player.Money;
            }

            public Player Player { get; }

            public int Score => this.score;

            public int GainedExperience => (int)(this.character.Experience - this.experienceAtStart);

            public int GainedMoney => this.Player.Money - this.moneyAtStart;

            public int Rank { get; set; }

            public void AddScore(int value)
            {
                Interlocked.Add(ref this.score, value);
            }
        }
    }
}