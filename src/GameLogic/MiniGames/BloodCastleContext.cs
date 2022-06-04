// <copyright file="BloodCastleContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Pathfinding;
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

    private IReadOnlyCollection<(string Name, int Score, int BonusExp, int BonusMoney)>? _highScoreTable;

    private uint _monsterKilled;
    private bool _gateDestroyed;
    private bool _statueSpawned;
    private bool _bridgeToggled;

    private Player? _winner;

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
        if (this._winner is not null)
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

        if (item is null)
        {
            player.ViewPlugIns.GetPlugIn<IShowDialogPlugIn>()?.ShowDialog(1, 0x18);
            return;
        }

        this._winner = player;

        player.Inventory!.RemoveItem(item);
        player.PersistenceContext.Delete(item);
        player.ViewPlugIns.GetPlugIn<Views.Inventory.IItemRemovedPlugIn>()?.RemoveItem(item.ItemSlot);
        player.ViewPlugIns.GetPlugIn<IShowDialogPlugIn>()?.ShowDialog(1, 0x17);
    }

    /// <inheritdoc />
    protected override void OnDestructibleDied(object? sender, DeathInformation e)
    {
        var destructible = sender as Destructible;
        if (destructible is null)
        {
            return;
        }

        if (destructible.Definition.Number == 131)
        {
            this._gateDestroyed = true;
            this._monsterKilled = 0;
            this.GateToggle(true);
        }

        if (destructible.Definition.Number == 132)
        {
            using var context = this._gameContext.PersistenceContextProvider.CreateNewContext();
            var item = context.CreateNew<Item>();
            item.Definition = this._gameContext.Configuration.Items.FirstOrDefault(def => def.Group == 13 && def.Number == 19);

            var dropper = this._gameStates.FirstOrDefault(s => s.Key == e.KillerName).Value.Player;
            var dropped = new DroppedItem(item, new Point(14, 95), this.Map, dropper);
            this.Map.Add(dropped);
        }
    }

    /// <inheritdoc />
    protected override void OnMonsterDied(object? sender, DeathInformation e)
    {
        if (this._gameStates.TryGetValue(e.KillerName, out var state))
        {
            state.AddScore(this.Definition.GameLevel);
        }

        var monster = sender as Monster;
        if (monster is null)
        {
            return;
        }

        if (!this._bridgeToggled)
        {
            if (this._monsterKilled < 40)
            {
                this._monsterKilled++;
            }

            if (this._monsterKilled == 40)
            {
                this.BridgeToggle(true);
            }
        }

        if (!this._statueSpawned && this._gateDestroyed)
        {
            var monsterNumbers = new[] { 089, 095, 112, 118, 124, 130, 143, 433 };

            if (this._monsterKilled < 2 && monsterNumbers.Contains(monster.Definition.Number))
            {
                this._monsterKilled++;
            }

            if (this._monsterKilled == 2)
            {
                this.SpawnStatue();
                this._statueSpawned = true;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnGameStart(ICollection<Player> players)
    {
        foreach (var player in players)
        {
            this._gameStates.TryAdd(player.Name, new PlayerGameState(player));
        }

        this.EntranceToggle(true);
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

        var scoreList = new List<(string Name, int Score, int BonusExp, int BonusMoney)>();
        int rank = 0;
        foreach (var state in sortedFinishers)
        {
            rank++;
            state.Rank = rank;
            this.GiveRewards(state.Player, rank);
            scoreList.Add((
                state.Player.Name,
                state.Score,
                this.Definition.Rewards.FirstOrDefault(r => r.RewardType == MiniGameRewardType.Experience && (r.Rank is null || r.Rank == rank))?.RewardAmount ?? 0,
                this.Definition.Rewards.FirstOrDefault(r => r.RewardType == MiniGameRewardType.Money && (r.Rank is null || r.Rank == rank))?.RewardAmount ?? 0));
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
            var (name, score, bonusMoney, bonusExp) = table.First(t => t.Name == player.Name);
            player.ViewPlugIns.GetPlugIn<IBloodCastleScoreTableViewPlugin>()?.ShowScoreTable(true, name, score, bonusExp, bonusMoney);
        }
    }

    private void EntranceToggle(bool value)
    {
        var areas = new List<(byte startX, byte startY, byte endX, byte endY)>
        {
            (13, 15, 15, 23),
        };

        this.UpdateWalkMapClient(areas, TerrainAttributeType.Blocked, value);
        this.UpdateWalkMapServer(areas, value);
    }

    private void BridgeToggle(bool value)
    {
        var areas = new List<(byte startX, byte startY, byte endX, byte endY)>
        {
            (13, 70, 15, 75),
        };

        this.UpdateWalkMapClient(areas, TerrainAttributeType.NoGround, value);
        this.UpdateWalkMapServer(areas, value);
    }

    private void GateToggle(bool value)
    {
        var areas = new List<(byte startX, byte startY, byte endX, byte endY)>
        {
            (13, 76, 15, 79),
            (11, 80, 25, 89),
            (08, 80, 10, 83),
        };

        this.UpdateWalkMapClient(areas, TerrainAttributeType.Blocked, value);
        this.UpdateWalkMapServer(areas, value);
    }

    private void SpawnStatue()
    {
        // Statue of Saint
        var monsterDef = this._gameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == 132);
        if (monsterDef is null)
        {
            return;
        }

        var position = new Point(014, 095);
        var area = new MonsterSpawnArea
        {
            GameMap = this.Map.Definition,
            MonsterDefinition = monsterDef,
            SpawnTrigger = SpawnTrigger.Automatic,
            Direction = Direction.SouthWest,
            Quantity = 1,
            X1 = position.X,
            X2 = position.X,
            Y1 = position.Y,
            Y2 = position.Y,
        };

        var statue = new Destructible(area, monsterDef, this.Map);
        statue.Initialize();
        this.Map.Add(statue);
    }

    private void UpdateWalkMapServer(
        List<(byte startX, byte startY, byte endX, byte endY)> areas,
        bool value)
    {
        foreach (var (startX, startY, endX, endY) in areas)
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    this.Map.Terrain.WalkMap[x, y] = value;
                }
            }
        }
    }

    private void UpdateWalkMapClient(
        List<(byte startX, byte startY, byte endX, byte endY)> areas,
        TerrainAttributeType type,
        bool value)
    {
        _ = this.ForEachPlayerAsync(player =>
        {
            player.ViewPlugIns.GetPlugIn<IChangeTerrainAttributesViewPlugin>()?
                .ChangeAttributes(false, type, value, areas);
        });
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