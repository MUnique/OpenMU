// <copyright file="IllusionTempleContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;

namespace MUnique.OpenMU.GameLogic.MiniGames;

/// <summary>
/// The context of an illusion temple game.
/// </summary>
/// <remarks>
/// An illusion temple event works like that:
///   Up to 10 players enter one of the six temples (maps 45 to 50), each temple covering its own
///   character level bracket. Unlike the other mini games, this one is team based and player versus
///   player: the participants are split into two teams which fight each other for the whole match.
///
///   A "Stone Statue" (NPC 380) holds the holy relic - only one is up at a time, randomly picked from
///   a pool of possible positions on the map.
///   * A player talks to the statue and becomes the relic's carrier; the statue breaks and disappears.
///   * The carrier is announced to everyone in the temple, so both teams know whom to chase.
///   * Carrying the relic to his own team's item storage (NPC 383 for the allied forces, 384 for the
///     illusion forces) scores a point for the carrier's team and spawns the next statue.
///   * When the carrier dies or leaves the event, the relic is dropped on the ground and can be picked
///     up again by anyone.
/// The game has a time limit. When it's up, the team with more points wins.
/// While the game is running, the clients get a cyclic state update with the remaining time, the
/// points of both teams and the positions of the own team's members.
///
/// After the game ended, the players get their rewards:
///
/// Experience:
///   Each player receives experience, which is reported back in the result packet, so that the client
///   can show it in the score board next to the player's name, team and class.
/// Items:
///   The winners request their reward explicitly after the result has been shown, and it's usually
///   granted as an item drop.
///
/// Additionally, four special skills (210 to 213 - Order of Protection, Restraint, Tracking and
/// Weaken) can only be used inside this event. They are not paid with mana, but with an own pool of
/// skill points which is tracked per player and reported to the client separately.
/// </remarks>
public sealed class IllusionTempleContext : MiniGameContext
{
    /// <summary>
    /// The number of the stone statue, which holds the sacred relic during a match.
    /// </summary>
    private const short StatueNPC = 380;

    /// <summary>
    /// How long the players see the "Preparation" state (still behind the arena barriers) before the
    /// battle actually starts.
    /// </summary>
    private static readonly TimeSpan PreparationDuration = TimeSpan.FromSeconds(20);

    /// <summary>
    /// How long it takes after a scored point for the next stone statue to spawn - matches the
    /// original event's regen delay.
    /// </summary>
    private static readonly TimeSpan StatueRespawnDelay = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The lowest NPC number of the roaming "Illusion Sorc. Spirit" arena monsters, across all temples.
    /// </summary>
    private const short ArenaMonsterRangeStart = 386;

    /// <summary>
    /// The highest NPC number of the roaming "Illusion Sorc. Spirit" arena monsters, across all temples.
    /// </summary>
    private const short ArenaMonsterRangeEnd = 399;

    /// <summary>
    /// The skill points a player starts a match with.
    /// </summary>
    private const byte InitialSkillPoints = 10;

    /// <summary>
    /// The maximum number of skill points a player can accumulate.
    /// </summary>
    private const byte MaximumSkillPoints = 90;

    /// <summary>
    /// The skill points awarded for killing an opposing player.
    /// </summary>
    private const byte SkillPointsPerPlayerKill = 1;

    /// <summary>
    /// The skill points awarded for killing one of the roaming arena monsters.
    /// </summary>
    private const byte SkillPointsPerMonsterKill = 2;

    /// <summary>
    /// The number of the "Order of Protection" special skill.
    /// </summary>
    private const ushort OrderOfProtectionSkillNumber = 210;

    /// <summary>
    /// The number of the "Restraint" special skill.
    /// </summary>
    private const ushort RestraintSkillNumber = 211;

    /// <summary>
    /// The number of the "Tracking" special skill.
    /// </summary>
    private const ushort TrackingSkillNumber = 212;

    /// <summary>
    /// The number of the "Weaken" special skill.
    /// </summary>
    private const ushort WeakenSkillNumber = 213;

    /// <summary>
    /// The skill point cost of every special skill.
    /// </summary>
    private const byte SpecialSkillCost = 10;

    /// <summary>
    /// The maximum distance from which the "Restraint" and "Weaken" special skills can target someone.
    /// </summary>
    private const int SpecialSkillMaximumDistance = 6;

    /// <summary>
    /// The number of the magic effect which the "Order of Protection" special skill applies to its
    /// caster. Matches the seeded magic effect of the same number - see
    /// <c>IllusionTempleInitializer.CreateSpecialSkillEffects</c>.
    /// </summary>
    private const short OrderOfProtectionEffectNumber = 210;

    /// <summary>
    /// The number of the magic effect which the "Restraint" special skill applies to its target.
    /// Matches the seeded magic effect of the same number - see
    /// <c>IllusionTempleInitializer.CreateSpecialSkillEffects</c>.
    /// </summary>
    private const short RestraintEffectNumber = 211;

    /// <summary>
    /// Team A and Team B.
    /// </summary>
    private readonly ConcurrentDictionary<Player, IllusionTempleTeam> _teams = new();

    /// <summary>
    /// The skill points of each participant, which fuel the event's special skills (210 to 213) instead
    /// of mana.
    /// </summary>
    private readonly ConcurrentDictionary<Player, byte> _skillPoints = new();

    /// <summary>
    /// The experience granted to a winner, so that it can be reported back in the result packet.
    /// </summary>
    private readonly ConcurrentDictionary<Player, int> _grantedExperience = new();

    /// <summary>
    /// The rank of every winner at the moment the game ended, kept around so that <see cref="ClaimRewardAsync"/>
    /// can apply rank-restricted rewards whenever the player actually claims them.
    /// </summary>
    private readonly ConcurrentDictionary<Player, int> _winnerRanks = new();

    /// <summary>
    /// The players who already claimed their reward, so that clicking the result dialog's close button
    /// more than once can't be used to farm item rewards.
    /// </summary>
    private readonly ConcurrentDictionary<Player, bool> _claimedRewards = new();

    /// <summary>
    /// The spawn point of the allied forces, in the chamber which the map reserves for this team at the
    /// north western corner. It's the target of the map's spawn gates 148 to 153, one per temple.
    /// </summary>
    /// <remarks>
    /// The chamber is closed off from the battle ground - the barriers between them are hardcoded at
    /// client side and are only removed when the client is told that the battle started. So the players
    /// wait here until the event sends that state, and walk into the arena afterwards.
    /// </remarks>
    private readonly Point alliedForcesCoordinates = new Point(141, 41);

    /// <summary>
    /// The spawn point of the illusion forces, in the chamber at the south eastern corner. It's the
    /// target of the map's spawn gates 154 to 159, one per temple.
    /// </summary>
    private readonly Point illusionForcesCoordinates = new Point(194, 124);

    /// <summary>
    /// Remaning Time of IT
    /// </summary>
    private TimeSpan _remainingTime;

    /// <summary>
    /// The player who currently carries the holy relic, or <c>null</c> if nobody currently does.
    /// </summary>
    private Player? _relicCarrier;

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleContext"/> class.
    /// </summary>
    /// <param name="key">The key of this context.</param>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context, to which this game belongs.</param>
    /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
    public IllusionTempleContext(MiniGameMapKey key, MiniGameDefinition definition, IGameContext gameContext, IMapInitializer mapInitializer)
        : base(key, definition, gameContext, mapInitializer)
    {
    }

    /// <inheritdoc />
    /// <remarks>
    /// The whole event is a fight between two teams, so killing another participant must never be
    /// punished as a regular player kill.
    /// </remarks>
    public override bool AllowPlayerKilling => true;

    /// <summary>
    /// Gets the score of both teams.
    /// </summary>
    public IllusionTempleScore Score { get; } = new();

    /// <inheritdoc />
    protected override TimeSpan RemainingTime => this._remainingTime;

    /// <inheritdoc />
    /// <remarks>
    /// Returns a player of the leading team, so that the mini game definition's reward conditions
    /// (which classify a winner by his party, see <see cref="GameEndedAsync"/>) can tell winners and
    /// losers apart.
    /// </remarks>
    protected override Player? Winner => this.Score.LeadingTeam is { } leadingTeam
        ? this._teams.FirstOrDefault(entry => entry.Value == leadingTeam).Key
        : null;

    /// <inheritdoc />
    /// <remarks>
    /// Two teams fighting each other need at least 2 players - configurable per temple via
    /// <see cref="MiniGameDefinition.MinimumPlayerCount"/> in the admin panel.
    /// </remarks>
    protected override int MinimumPlayerCount => this.Definition.MinimumPlayerCount > 0 ? this.Definition.MinimumPlayerCount : 2;

    /// <summary>
    /// Gets the spawn gate of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The gate where the player is teleported to.</returns>
    public override ExitGate? GetSpawnGate(Player player)
    {
        if (!this._teams.TryGetValue(player, out var team))
        {
            return null;
        }

        // The areas match the spawn gates which the map defines for the two teams: 148 to 153 for the
        // allied forces and 154 to 159 for the illusion forces, one of each per temple.
        var (start, end) = team == IllusionTempleTeam.AlliedForces
            ? (new Point(141, 41), new Point(146, 45))
            : (new Point(194, 124), new Point(198, 127));

        return new ExitGate
        {
            Map = this.Map.Definition,
            X1 = start.X,
            Y1 = start.Y,
            X2 = end.X,
            Y2 = end.Y,
        };
    }

    /// <summary>
    /// Handles a player talking to the stone statue (NPC 380) which holds the sacred relic.
    /// </summary>
    /// <param name="player">The player who talked to the statue.</param>
    public async ValueTask TalkToNpcStoneStatueAsync(Player player)
    {
        if (this._relicCarrier is not null)
        {
            // Somebody already carries the relic - the statue that granted it must already be gone.
            return;
        }

        var cursedCastleWater = player.GameContext.Configuration.Items.First(item => item.Group == 14 && item.Number == 64);

        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = cursedCastleWater;

        var invIndex = player.Inventory?.CheckInvSpace(item);
        if (invIndex is null)
        {
            await player.ShowBlueMessageAsync("Your Inventory is full!").ConfigureAwait(false);
            return;
        }

        await player.Inventory!.AddItemAsync(item).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
        await this.ShowGoldenMessageAsync(nameof(PlayerMessage.IllusionTempleRelicPickedUpFormat), player.Name).ConfigureAwait(false);

        if (player.OpenedNpc is { } statue)
        {
            await statue.DisposeAsync().ConfigureAwait(false);
        }

        this._relicCarrier = player;
        await this.ForEachPlayerAsync(p => p.InvokeViewPlugInAsync<IIllusionTempleHolyItemRelicsViewPlugIn>(
            vp => vp.ShowHolyItemRelicsAsync(player.Id, player.Name)).AsTask()).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles a player talking to the team storage (NPC 383/384) which gets the sacred relic.
    /// </summary>
    /// <param name="npcNumber">NPC number.</param>
    /// <param name="player">The player who talked to the statue.</param>
    public async ValueTask TalkToNpcTeamStorageAsync(int npcNumber, Player player)
    {
            if (player != this._relicCarrier)
            {
                return;
            }

            var relicItem = player.Inventory?.Items
            .FirstOrDefault(i => i.Definition?.Group == 14 && i.Definition?.Number == 64);

            if (relicItem is null)
            {
                this._relicCarrier = null;
                return;
            }

            if (!this._teams.TryGetValue(player, out var playerTeam))
            {
                return;
            }

            if (npcNumber == 383 && playerTeam == IllusionTempleTeam.AlliedForces)
            {
                this.Score.IncreaseScore(IllusionTempleTeam.AlliedForces);
            }
            else if (npcNumber == 384 && playerTeam == IllusionTempleTeam.IllusionForces)
            {
                this.Score.IncreaseScore(IllusionTempleTeam.IllusionForces);
            }
            else
            {
                return;
            }

            await player.Inventory!.RemoveItemAsync(relicItem).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IItemDropResultPlugIn>(p => p.ItemDropResultAsync(relicItem.ItemSlot, true)).ConfigureAwait(false);
            this._relicCarrier = null;

            // Push the new score to the clients right away, instead of waiting for the next tick of
            // the cyclic state update (ShowRemainingTimeLoopAsync).
            await this.UpdateStateForAllAsync().ConfigureAwait(false);

            // The next statue doesn't appear immediately - just like in the original event, there's a
            // short delay after a scored point. This runs in the background so the delivering player's
            // action isn't held up by it.
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(StatueRespawnDelay, this.GameEndedToken).ConfigureAwait(false);
                    await this.SpawnRandomStatueAsync().ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // The event ended before the respawn delay elapsed.
                }
            }, this.GameEndedToken);
    }

    /// <summary>
    /// Spawns the stone statue at a random position from the map's pool of statue spawn points.
    /// </summary>
    private async ValueTask SpawnRandomStatueAsync()
    {
        try
        {
            var statueSpawns = this.Map.Definition.MonsterSpawns
                .Where(spawn => spawn.MonsterDefinition?.Number == StatueNPC)
                .ToList();
            if (statueSpawns.Count == 0)
            {
                this.Logger.LogWarning("No stone statue spawn points found on map {Map}.", this.Map.Definition.Name);
                return;
            }

            var spawnArea = statueSpawns[Rand.NextInt(0, statueSpawns.Count)];
            var statue = new NonPlayerCharacter(spawnArea, spawnArea.MonsterDefinition!, this.Map);
            statue.Initialize();
            await this.Map.AddAsync(statue).ConfigureAwait(false);
            statue.OnSpawn();

            await this.ShowGoldenMessageAsync(nameof(PlayerMessage.IllusionTempleStatueSpawnedMessage)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error while spawning the illusion temple stone statue.");
        }
    }

    /// <summary>
    /// Handles a player's request to use one of the four special skills (210 to 213), which are paid
    /// with the event's own skill point pool instead of mana.
    /// </summary>
    /// <param name="player">The player who requests to use the skill.</param>
    /// <param name="skillNumber">The number of the requested skill.</param>
    /// <param name="targetObjectIndex">The map object index of the skill's target, if any.</param>
    public async ValueTask UseSkillAsync(Player player, ushort skillNumber, ushort targetObjectIndex)
    {
        if (!this._teams.ContainsKey(player))
        {
            return;
        }

        if (this._skillPoints.GetValueOrDefault(player, InitialSkillPoints) < SpecialSkillCost)
        {
            await player.InvokeViewPlugInAsync<IIllusionTempleSkillUsageResultViewPlugin>(
                p => p.ShowSkillUsageResultAsync(false, skillNumber, player.Id, targetObjectIndex)).ConfigureAwait(false);
            return;
        }

        var target = player.CurrentMap?.GetObject(targetObjectIndex) as IAttackable;
        var success = skillNumber switch
        {
            OrderOfProtectionSkillNumber => await this.UseOrderOfProtectionAsync(player).ConfigureAwait(false),
            RestraintSkillNumber => await this.UseRestraintAsync(player, target).ConfigureAwait(false),
            TrackingSkillNumber => await this.UseTrackingAsync(player).ConfigureAwait(false),
            WeakenSkillNumber => await this.UseWeakenAsync(player, target).ConfigureAwait(false),
            _ => false,
        };

        if (success)
        {
            await this.AwardSkillPointsAsync(player, -SpecialSkillCost).ConfigureAwait(false);
        }

        await player.InvokeViewPlugInAsync<IIllusionTempleSkillUsageResultViewPlugin>(
            p => p.ShowSkillUsageResultAsync(success, skillNumber, player.Id, target?.Id ?? 0)).ConfigureAwait(false);
    }

    /// <summary>
    /// Skill 210 - grants the caster a temporary damage reduction.
    /// </summary>
    private async ValueTask<bool> UseOrderOfProtectionAsync(Player player)
    {
        var effectDefinition = player.GameContext.Configuration.MagicEffects.FirstOrDefault(e => e.Number == OrderOfProtectionEffectNumber);
        if (effectDefinition is null)
        {
            return false;
        }

        var elements = effectDefinition.PowerUpDefinitions
            .Select(powerUp => new MagicEffect.ElementWithTarget(player.Attributes!.CreateElement(powerUp), powerUp.TargetAttribute!))
            .ToArray();
        var duration = effectDefinition.Duration?.ConstantValue.Value ?? 15f;
        var magicEffect = new MagicEffect(TimeSpan.FromSeconds(duration), effectDefinition, elements);
        await player.MagicEffectList.AddEffectAsync(magicEffect).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Skill 211 - roots the target in place for a while, within <see cref="SpecialSkillMaximumDistance"/>.
    /// </summary>
    private async ValueTask<bool> UseRestraintAsync(Player player, IAttackable? target)
    {
        if (target is null || target == player || player.GetDistanceTo(target) > SpecialSkillMaximumDistance)
        {
            return false;
        }

        var effectDefinition = player.GameContext.Configuration.MagicEffects.FirstOrDefault(e => e.Number == RestraintEffectNumber);
        if (effectDefinition is null)
        {
            return false;
        }

        var elements = effectDefinition.PowerUpDefinitions
            .Select(powerUp => new MagicEffect.ElementWithTarget(target.Attributes.CreateElement(powerUp), powerUp.TargetAttribute!))
            .ToArray();
        var duration = effectDefinition.Duration?.ConstantValue.Value ?? 15f;
        var magicEffect = new MagicEffect(TimeSpan.FromSeconds(duration), effectDefinition, elements);
        await target.MagicEffectList.AddEffectAsync(magicEffect).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Skill 212 - teleports the caster next to the current relic carrier. Fails if the caster is
    /// stunned or frozen, nobody currently carries the relic, or the caster is the carrier himself.
    /// </summary>
    private async ValueTask<bool> UseTrackingAsync(Player player)
    {
        if (this._relicCarrier is not { } carrier
            || carrier == player
            || player.Attributes![Stats.IsStunned] > 0
            || player.Attributes![Stats.IsFrozen] > 0)
        {
            return false;
        }

        await player.MoveAsync(carrier.Position).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Skill 213 - instantly halves the target's current shield, within <see cref="SpecialSkillMaximumDistance"/>.
    /// </summary>
    private ValueTask<bool> UseWeakenAsync(Player player, IAttackable? target)
    {
        if (target is null || target == player || player.GetDistanceTo(target) > SpecialSkillMaximumDistance)
        {
            return ValueTask.FromResult(false);
        }

        target.Attributes[Stats.CurrentShield] /= 2;
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Split the players into the two teams and place them at their team's spawn point.
    /// </remarks>
    protected override async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        if (players.Count < this.MinimumPlayerCount)
        {
            // Not enough players made it into the arena (e.g. some left again during the countdown) -
            // there's nobody to split into two teams, so the match can't be played.
            this.FinishEvent();
            return;
        }

        var playersArray = players.ToArray();

        // Random players to add to the teams
        for (var i = playersArray.Length - 1; i > 0; i--)
        {
            var j = Rand.NextInt(0, i + 1);
            (playersArray[i], playersArray[j]) = (playersArray[j], playersArray[i]);
        }

        var gameContext = playersArray[0].GameContext;
        var alliedParty = gameContext.PartyManager.CreateParty();
        var illusionParty = gameContext.PartyManager.CreateParty();

        for (var i = 0; playersArray.Length > i; i++)
        {
            // Adding player to team AlliedForces or IllusionForces
            var player = playersArray[i];
            player.Party = null;
            var team = i % 2 == 0 ? IllusionTempleTeam.AlliedForces : IllusionTempleTeam.IllusionForces;
            if (!this._teams.TryAdd(player, team))
            {
                this.Logger.LogWarning("Player {Player} was already assigned to a team.", player.Name);
                continue;
            }

            this._skillPoints[player] = InitialSkillPoints;
            var party = team == IllusionTempleTeam.AlliedForces ? alliedParty : illusionParty;
            if (!await party.AddAsync(player).ConfigureAwait(false))
            {
                // The player still takes part in the event and is assigned to a team - he just doesn't
                // show up in the party window of his team mates.
                this.Logger.LogWarning(
                    "Player {Player} doesn't fit into the party of team {Team}: the party is limited to {MaxPartySize} members, while the event allows {MaximumPlayerCount} players.",
                    player.Name,
                    team,
                    party.MaxPartySize,
                    this.Definition.MaximumPlayerCount);
            }

            await this.TeleportToStartCoordinatesAsync(team, player).ConfigureAwait(false);
        }

        await base.OnGameStartAsync(players).ConfigureAwait(false);

        // The client keeps its event interface closed and the arena barriers up until it's told that
        // the battle started - the barrier areas are hardcoded at client side, so this is the only way
        // to open them. The two states are deliberately separated by a short delay, so the players
        // actually get to see the preparation phase before the barriers drop - sending both back to
        // back made the client skip straight to the battle without any noticeable wait.
        var templeNumber = (byte)this.Definition.GameLevel;
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IIllusionTempleEventStateViewPlugIn>(
            p => p.ChangeEventStateAsync(templeNumber, IllusionTempleEventStatus.Preparation)).AsTask()).ConfigureAwait(false);

        try
        {
            await Task.Delay(PreparationDuration, this.GameEndedToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // The event was already finished (e.g. not enough players) while the preparation phase
            // was still running.
            return;
        }

        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IIllusionTempleEventStateViewPlugIn>(
            p => p.ChangeEventStateAsync(templeNumber, IllusionTempleEventStatus.BattleStarted)).AsTask()).ConfigureAwait(false);
        await this.ShowGoldenMessageAsync(nameof(PlayerMessage.IllusionTempleBattleStartedMessage)).ConfigureAwait(false);

        await this.SpawnRandomStatueAsync().ConfigureAwait(false);

        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IIllusionTempleSkillPointUpdateViewPlugin>(
            p => p.UpdateSkillPointsAsync(this._skillPoints.GetValueOrDefault(player, InitialSkillPoints))).AsTask()).ConfigureAwait(false);

        // The cyclic state update runs until the game ends - the token makes sure that it doesn't
        // outlive the context and keeps sending to players who already left the event.
        _ = Task.Run(async () => await this.ShowRemainingTimeLoopAsync(this.GameEndedToken).ConfigureAwait(false), this.GameEndedToken);
    }

    /// <summary>
    /// Awards skill points to a player, capped at <see cref="MaximumSkillPoints"/>, and informs him
    /// about his new balance.
    /// </summary>
    /// <param name="player">The player who is awarded the points.</param>
    /// <param name="amount">The number of points to award.</param>
    private async ValueTask AwardSkillPointsAsync(Player player, int amount)
    {
        if (!this._teams.ContainsKey(player))
        {
            return;
        }

        var newBalance = (byte)Math.Clamp(this._skillPoints.GetValueOrDefault(player, InitialSkillPoints) + amount, 0, MaximumSkillPoints);
        this._skillPoints[player] = newBalance;

        await player.InvokeViewPlugInAsync<IIllusionTempleSkillPointUpdateViewPlugin>(p => p.UpdateSkillPointsAsync(newBalance)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// When the dead player carried the relic, drop it so that it can be picked up again. When the
    /// killer took part in the event on the opposing team, he's awarded skill points for the kill.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    protected override async void OnPlayerDied(object? sender, DeathInformation e)
    {
        base.OnPlayerDied(sender, e);

        try
        {
            if (sender is not Player deadPlayer)
            {
                return;
            }

            await this.DropRelicIfCarriedByAsync(deadPlayer).ConfigureAwait(false);

            if (deadPlayer.CurrentMap?.GetObject(e.KillerId) is Player killer
                && killer != deadPlayer
                && this._teams.TryGetValue(killer, out var killerTeam)
                && this._teams.TryGetValue(deadPlayer, out var deadPlayerTeam)
                && killerTeam != deadPlayerTeam)
            {
                await this.AwardSkillPointsAsync(killer, SkillPointsPerPlayerKill).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error while dropping the illusion temple relic after a player died.");
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Killing one of the roaming "Illusion Sorc. Spirit" arena monsters (386 to 399) grants the killer
    /// skill points for the event's special skills.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    protected override async void OnMonsterDied(object? sender, DeathInformation e)
    {
        base.OnMonsterDied(sender, e);

        try
        {
            if (sender is not AttackableNpcBase monster
                || monster.Definition.Number < ArenaMonsterRangeStart
                || monster.Definition.Number > ArenaMonsterRangeEnd)
            {
                return;
            }

            if (monster.CurrentMap?.GetObject(e.KillerId) is Player killer && this._teams.ContainsKey(killer))
            {
                await this.AwardSkillPointsAsync(killer, SkillPointsPerMonsterKill).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error while awarding illusion temple skill points for an arena monster kill.");
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// When the player who left carried the relic (character switch, disconnect or leaving the event
    /// on purpose), drop it so that it can be picked up again by the remaining participants. The player
    /// himself is sent back to Devias and removed from his temporary event party, just like a player who
    /// finishes a match normally in <see cref="GameEndedAsync"/>. If fewer than <see cref="MinimumPlayerCount"/>
    /// players remain afterwards, the match can't continue and is ended right away.
    /// </remarks>
    protected override async ValueTask OnObjectRemovedFromMapAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            await this.DropRelicIfCarriedByAsync(player).ConfigureAwait(false);

            if (player.Party is { } party)
            {
                await party.KickMySelfAsync(player).ConfigureAwait(false);
            }

            var devias = player.GameContext.Configuration.Maps.First(map => map.Number == 2);
            await player.WarpToAsync(new ExitGate
            {
                Map = devias,
                X1 = 197,
                Y1 = 35,
                X2 = 218,
                Y2 = 50,
            }).ConfigureAwait(false);

            // Otherwise he'd keep showing up as an alive team mate on the mini map of his former team.
            this._teams.TryRemove(player, out _);
            this._skillPoints.TryRemove(player, out _);
        }

        await base.OnObjectRemovedFromMapAsync(args).ConfigureAwait(false);

        if (args.Object is Player && this.PlayerCount < this.MinimumPlayerCount)
        {
            this.FinishEvent();
        }
    }

    /// <summary>
    /// Drops the holy relic on the ground, if the specified player currently carries it. Works for any
    /// reason the player stops carrying it that isn't already a ground drop by itself (death, leaving
    /// the event) - a voluntary drop through the normal drop-item action is instead caught by
    /// <see cref="OnItemDroppedOnMap"/>, which is the single place that actually clears
    /// <see cref="_relicCarrier"/> and announces it, so that both paths behave the same and don't
    /// announce the drop twice.
    /// </summary>
    /// <param name="player">The player who might carry the relic.</param>
    private async ValueTask DropRelicIfCarriedByAsync(Player player)
    {
        if (player != this._relicCarrier)
        {
            return;
        }

        var relicItem = player.Inventory?.Items
            .FirstOrDefault(i => i.Definition?.Group == 14 && i.Definition?.Number == 64);

        if (relicItem is null || player.CurrentMap is not { } map)
        {
            this._relicCarrier = null;
            return;
        }

        var droppedItem = new DroppedItem(relicItem, player.Position, map, player);
        await map.AddAsync(droppedItem).ConfigureAwait(false);
        await player.Inventory!.RemoveItemAsync(relicItem).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemDropResultPlugIn>(p => p.ItemDropResultAsync(relicItem.ItemSlot, true)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Catches every way the holy relic can end up on the ground - a voluntary drop through the normal
    /// drop-item action, as well as the ones triggered by <see cref="DropRelicIfCarriedByAsync"/> (death,
    /// leaving the event) - and is the single place that clears <see cref="_relicCarrier"/> and announces
    /// it, so a manual drop is tracked exactly like the other cases.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    protected override async void OnItemDroppedOnMap(DroppedItem item)
    {
        base.OnItemDroppedOnMap(item);

        try
        {
            if (this._relicCarrier is not { } carrier
                || item.Item.Definition?.Group != 14
                || item.Item.Definition?.Number != 64)
            {
                return;
            }

            this._relicCarrier = null;
            await this.ShowGoldenMessageAsync(nameof(PlayerMessage.IllusionTempleRelicDroppedFormat), carrier.Name).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error while handling a dropped illusion temple relic.");
        }
    }

    /// <summary>
    /// Will be called when an item has been picked up by player.
    /// </summary>
    /// <param name="args">The event parameters.</param>
    protected async override ValueTask OnPlayerPickedUpItemAsync((Player Picker, ILocateable DroppedItem) args)
    {
        if (this._relicCarrier is null
            && args.DroppedItem is DroppedItem droppedItem
            && droppedItem.Item.Definition?.Group == 14
            && droppedItem.Item.Definition?.Number == 64)
        {
            this._relicCarrier = args.Picker;
            await this.ShowGoldenMessageAsync(nameof(PlayerMessage.IllusionTempleRelicPickedUpFormat), args.Picker.Name).ConfigureAwait(false);

            // The client needs to be told who the new carrier is - otherwise it keeps showing the
            // previous one (or nobody) as the hero on its mini map.
            await this.ForEachPlayerAsync(p => p.InvokeViewPlugInAsync<IIllusionTempleHolyItemRelicsViewPlugIn>(
                vp => vp.ShowHolyItemRelicsAsync(args.Picker.Id, args.Picker.Name)).AsTask()).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Show the individual result of the player - his team, its score and the gained experience.
    /// </remarks>
    protected async override ValueTask ShowScoreAsync(Player player)
    {
        var results = this._teams
        .Select(entry => (
            entry.Key.Name,
            MapNumber: (byte)this.Map.Definition.Number,
            Team: entry.Value,
            CharacterClass: (byte)(entry.Key.SelectedCharacter?.CharacterClass?.Number ?? 0),
            AddedExperience: this._grantedExperience.GetValueOrDefault(entry.Key)))
        .ToList();

        await player.InvokeViewPlugInAsync<IIllusionTempleScoreTableViewPlugIn>(p => p.ShowScoreTableAsync(this.Score.AlliedForcesScore, this.Score.IllusionForcesScore, results)).ConfigureAwait(false);
        await base.ShowScoreAsync(player).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles a player claiming his reward after the result dialog has been shown, in reaction to the
    /// <c>IllusionTempleRewardRequest</c> (0xBF05) packet - the client sends it when the player clicks the
    /// "Close" button on the result dialog. Experience has already been granted automatically in
    /// <see cref="GameEndedAsync"/>, so this only grants the remaining reward types (e.g. an item drop)
    /// to winners, and finally warps the requesting player to Devias - regardless of whether he won,
    /// lost, or already claimed his reward before.
    /// </summary>
    /// <param name="player">The player who claims his reward.</param>
    public async ValueTask ClaimRewardAsync(Player player)
    {
        if (this._claimedRewards.TryAdd(player, true)
            && this._teams.TryGetValue(player, out var team)
            && this.Score.LeadingTeam == team)
        {
            var rank = this._winnerRanks.GetValueOrDefault(player, 1);
            var remainingRewards = this.Definition.Rewards.Where(r =>
                r.RewardType is not (MiniGameRewardType.Experience or MiniGameRewardType.ExperiencePerRemainingSeconds)
                && this.DoesRewardApply(player, rank, r));
            foreach (var reward in remainingRewards)
            {
                await this.GiveRewardAsync(player, reward).ConfigureAwait(false);
            }
        }

        var devias = player.GameContext.Configuration.Maps.First(map => map.Number == 2);
        await player.WarpToAsync(new ExitGate
        {
            Map = devias,
            X1 = 197,
            Y1 = 35,
            X2 = 218,
            Y2 = 50,
        }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// The rewards are only granted to the members of the leading team. The success flags of the mini
    /// game definition can't decide that on their own, because they classify a winner by his party -
    /// and this event doesn't allow parties. So the winners are determined here by their team, and the
    /// definition only decides what they receive. On a draw, nobody wins and nobody is rewarded.
    /// Experience is granted right away, so it can be reported in the result packet - the remaining
    /// reward types (e.g. an item drop) are granted later, when the player actually claims them via
    /// <see cref="ClaimRewardAsync"/>.
    /// </remarks>
    protected override async ValueTask GameEndedAsync(ICollection<Player> finishers)
    {
        if (this.Score.LeadingTeam is { } winningTeam)
        {
            var winners = finishers
                .Where(player => this._teams.TryGetValue(player, out var team) && team == winningTeam)
                .ToList();

            var rank = 0;
            foreach (var winner in winners)
            {
                rank++;
                this._winnerRanks[winner] = rank;

                var experienceRewards = this.Definition.Rewards
                    .Where(r => r.RewardType is MiniGameRewardType.Experience or MiniGameRewardType.ExperiencePerRemainingSeconds
                                && this.DoesRewardApply(winner, rank, r))
                    .ToList();
                this._grantedExperience[winner] = experienceRewards.Sum(r => r.RewardAmount);

                foreach (var reward in experienceRewards)
                {
                    await this.GiveRewardAsync(winner, reward).ConfigureAwait(false);
                }
            }
        }

        // base.GameEndedAsync() shows the score table to every finisher (via ShowScoreAsync), which
        // reads this._teams - so it has to run before anyone leaves the map. Warping a player off this
        // map fires OnObjectRemovedFromMapAsync, which removes him from _teams; doing that first would
        // leave the score table empty for everyone (and the client, unable to find itself in the
        // now-empty participant list, appears to fall back to declaring both sides victorious). Players
        // stay on this map afterward - they leave individually via ClaimRewardAsync when they close their
        // own result dialog, or automatically once the base class's exit duration elapses
        // (MiniGameContext.ShutdownGameAsync -> MovePlayersToSafezoneAsync).
        await base.GameEndedAsync(finishers).ConfigureAwait(false);
    }

    private async ValueTask TeleportToStartCoordinatesAsync(IllusionTempleTeam team, Player player)
    {
        var cordinatesAlliedForces = this.alliedForcesCoordinates;
        var cordinatesIllusionForces = this.illusionForcesCoordinates;
        if (team == IllusionTempleTeam.AlliedForces)
        {
            cordinatesAlliedForces += new Point(1, 0); // every player on differend point (x,y)
            await player.MoveAsync(cordinatesAlliedForces).ConfigureAwait(false);
        }
        else
        {
            cordinatesIllusionForces += new Point(1, 0); // every player on differend point (x,y)
            await player.MoveAsync(cordinatesIllusionForces).ConfigureAwait(false);
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

            await this.UpdateStateForAllAsync().ConfigureAwait(false);
            while (!cancellationToken.IsCancellationRequested
                   && this._remainingTime >= TimeSpan.Zero
                   && await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                this._remainingTime = this._remainingTime.Subtract(timerInterval);
                await this.UpdateStateForAllAsync().ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected exception when the game ends before running into the timeout.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error during update of the illusion temple state: {0}", ex.Message);
        }
    }

    private ValueTask UpdateStateForAllAsync()
    {
        return this.ForEachPlayerAsync(player => this.UpdateStateAsync(player).AsTask());
    }

    private ValueTask UpdateStateAsync(Player player)
    {
        if (!this._teams.TryGetValue(player, out var ownTeam))
        {
            // The player is on the map, but wasn't assigned to a team - there is nothing to tell him.
            return ValueTask.CompletedTask;
        }

        // Only the own team is reported: the client shows these players on its mini map, and knowing
        // where the enemies are would take the whole hunt out of the event.
        var teamMembers = this._teams
            .Where(entry => entry.Value == ownTeam && entry.Key != player)
            .Select(entry => (
                PlayerId: entry.Key.Id,
                MapNumber: (byte)(entry.Key.CurrentMap?.Definition.Number ?? 0),
                PositionX: entry.Key.Position.X,
                PositionY: entry.Key.Position.Y))
            .ToList();

        (ushort PlayerId, byte PositionX, byte PositionY)? relicCarrier = this._relicCarrier is { } carrier
            ? (carrier.Id, carrier.Position.X, carrier.Position.Y)
            : null;

        return player.InvokeViewPlugInAsync<IIllusionTempleStateViewPlugin>(
            p => p.UpdateStateAsync(
                this._remainingTime,
                this.Score.AlliedForcesScore,
                this.Score.IllusionForcesScore,
                ownTeam,
                teamMembers,
                relicCarrier));
    }
}
