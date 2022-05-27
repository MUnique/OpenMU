// <copyright file="BloodCastleContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.Character;
using Nito.Disposables.Internals;
using System.Collections.Concurrent;
using System.Threading;

/// <summary>
/// The context of a blood castle game.
/// </summary>
public sealed class BloodCastleContext : MiniGameContext
{
    private readonly IGameContext _gameContext;
    private readonly ConcurrentDictionary<string, PlayerGameState> _gameStates = new ();

    private IReadOnlyCollection<(string Name, int Score, int BonusMoney, int BonusExp)>? _highScoreTable;

    private ushort _winnerId;
    private ushort _itemOwner;
    private int _monsterDiedCount;

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
        this._gameContext = gameContext;
    }

    /// <summary>
    /// Player interact with Archangel.
    /// </summary>
    /// <param name="player">The player who talks to Archangel.</param>
    public void TalkToNpcArchangel(Player player)
    {
        if (this._winnerId > 0)
        {
            player.ViewPlugIns.GetPlugIn<IShowDialogPlugIn>()?.ShowDialog(1, 0x2E);
            return;
        }

        if (this.State != MiniGameState.Playing)
        {
            player.ViewPlugIns.GetPlugIn<IShowDialogPlugIn>()?.ShowDialog(1, 0x18);
            return;
        }

        var item = player.Inventory?.Items.FirstOrDefault(item => item.Definition?.Group == 13 && item.Definition.Number == 19);
    }

    /// <summary>
    /// Do something when a destructible of the game has been died.
    /// </summary>
    /// <param name="attacker">The player which killed destructible.</param>
    /// <param name="destructible">The destructible was killed by player.</param>
    public void OnDestructibleDied(Player attacker, Destructible destructible)
    {
        switch (destructible.Definition.Number)
        {
            case 131:
                _ = this.ForEachPlayerAsync(player => this.DoorToggle(player));
                break;

            case 132:
                var item = this._gameContext.PersistenceContextProvider.CreateNewContext().CreateNew<Item>();
                item.Definition = this._gameContext.Configuration.Items.FirstOrDefault(def => def.Group == 13 && def.Number == 19);
                var droppedItem = new DroppedItem(item, new Pathfinding.Point(14, 95), this.Map, attacker);
                this.Map.Add(droppedItem);
                break;

            default:
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnMonsterDied(object? sender, DeathInformation e)
    {
        base.OnMonsterDied(sender, e);

        if (this._gameStates.TryGetValue(e.KillerName, out var state))
        {
            state.AddScore(this.Definition.GameLevel);
        }

        this._monsterDiedCount++;

        if (this._monsterDiedCount == 50)
        {
            _ = this.ForEachPlayerAsync(player => this.BridgeToggle(player));
        }
    }

    /// <inheritdoc />
    protected override void OnGameStart(ICollection<Player> players)
    {
        foreach (var player in players)
        {
            this._gameStates.TryAdd(player.Name, new PlayerGameState(player));

            this.EntranceToggle(player);
        }

        base.OnGameStart(players);
    }

    /// <inheritdoc />
    protected override void GameEnded(ICollection<Player> finishers)
    {
        var sortedFinishers = finishers
            .Select(f => this._gameStates[f.Name])
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

        this._highScoreTable = scoreList.AsReadOnly();

        this.SaveRanking(sortedFinishers.Select(state => (state.Rank, state.Player.SelectedCharacter!, state.Score)));
        base.GameEnded(finishers);
    }

    /// <inheritdoc />
    protected override void ShowScore(Player player)
    {
        if (this._highScoreTable is { } table
            && this._gameStates.TryGetValue(player.Name, out var state))
        {
            player.ViewPlugIns.GetPlugIn<IMiniGameScoreTableViewPlugin>()?.ShowScoreTable((byte)state.Rank, table);
        }
    }

    private void EntranceToggle(Player player)
    {
        var areas = new List<(byte startX, byte startY, byte endX, byte endY)>
        {
            (13, 15, 15, 23),
        };

        player.ViewPlugIns.GetPlugIn<IChangeTerrainAttributesViewPlugin>()?
            .ChangeAttributes(false, TerrainAttributeType.Blocked, true, areas);
    }

    private void BridgeToggle(Player player)
    {
        var areas = new List<(byte startX, byte startY, byte endX, byte endY)>
        {
            (13, 70, 15, 75),
        };

        player.ViewPlugIns.GetPlugIn<IChangeTerrainAttributesViewPlugin>()?
            .ChangeAttributes(false, TerrainAttributeType.NoGround, true, areas);
    }

    private void DoorToggle(Player player)
    {
        var areas = new List<(byte startX, byte startY, byte endX, byte endY)>
        {
            (13, 70, 15, 80),
            (11, 80, 25, 89),
            (08, 80, 10, 83),
        };

        player.ViewPlugIns.GetPlugIn<IChangeTerrainAttributesViewPlugin>()?
            .ChangeAttributes(false, TerrainAttributeType.Blocked, true, areas);
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