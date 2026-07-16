// <copyright file="CombatHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using System.Collections.Concurrent;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Handles combat logic including target selection, attacks, combo attacks, and skill usage.
/// </summary>
public sealed class CombatHandler
{
    private const byte DefaultRange = 1;
    private const byte BowRange = 6;

    /// <summary>
    /// See <see cref="IsSafeTarget"/>: the largest share of the bot's maximum health a single average
    /// monster hit may take for the monster to count as safe. Sized so the bot survives several hits
    /// even when a few monsters aggro at once, with the healing handler (potions at 60%) keeping up.
    /// Tightened from 0.20 with the player-meta stat builds: their small health pools mean melee bots
    /// (which stand inside the monster pack) need a bigger margin per hit to survive a swarm.
    /// </summary>
    private const float SafeHitHealthShare = 0.15f;

    /// <summary>
    /// See <see cref="IsSafeTarget"/>: the bot's attack power must exceed the monster's defense by this
    /// factor. Without it a bot picks fights it can barely scratch - e.g. the Vulcanus tank monsters
    /// (defense ~340, health ~100k) shrug off a modestly geared bot's hits, the "fight" lasts minutes,
    /// and the accumulated damage kills the bot even though every single hit it takes looks survivable.
    /// </summary>
    private const float MinAttackAdvantage = 1.2f;

    /// <summary>
    /// See <see cref="IsSafeTarget"/>: the monster must die within this many net hits of the bot.
    /// The per-hit checks alone let a fighter "safely" besiege a 100k-health tank monster for ten
    /// minutes, until its potions ran dry and it died anyway - the fight length itself is the risk.
    /// At the offline AI's attack pace this bounds a kill to roughly a minute or two.
    /// </summary>
    private const int MaxHitsToKill = 100;

    /// <summary>
    /// See <see cref="IsSafeTarget"/>: how much longer a mastered bot may take to kill a monster which
    /// pays master experience. Master experience is only granted for monsters of at least
    /// <c>GameConfiguration.MinimumMonsterLevelForMasterExperience</c>, and those hold 40.000+ health -
    /// out of reach of the regular hit budget for a bot in the gear it collects from drops, which left
    /// mastered bots hunting monsters that pay them nothing at all. Since a character at the maximum
    /// level earns nothing else either, a long fight it survives beats a quick one worth zero: the
    /// budget is stretched for those monsters only, while the survivability check below is NOT - a bot
    /// still refuses a monster whose hits it cannot take.
    /// </summary>
    private const int MasterHitBudgetFactor = 3;
    private const int ComboFinisherDelayTicks = 3;
    private const int InterSkillDelayTicks = 1;
    private const int MinComboSkillCount = 3;

    /// <summary>After this many consecutive failed approaches the target counts as unreachable.</summary>
    private const int MaxApproachFailures = 3;

    private const short DrainLifeBaseSkillId = 214;
    private const short DrainLifeStrengthenerSkillId = 458;
    private const short DrainLifeMasterySkillId = 462;

    /// <summary>How long an unreachable target is ignored before it may be considered again.</summary>
    private static readonly TimeSpan UnreachableTargetBlacklistDuration = TimeSpan.FromSeconds(10);

    private static readonly TargetedSkillDefaultPlugin DefaultPlugin = new();

    /// <summary>
    /// Cache of the combat-relevant stats of monster definitions (config data, immutable at runtime),
    /// so the safety checks of hundreds of bots don't re-scan the attribute lists every tick. Keyed by
    /// the monster number rather than the definition instance, so a configuration reload (which builds
    /// new <see cref="MonsterDefinition"/> instances) reuses the entries instead of orphaning them.
    /// </summary>
    private static readonly ConcurrentDictionary<short, (int Level, float AverageDamage, float Defense, float Health, float AttackRate)> MonsterStatsCache = new();

    private readonly OfflinePlayer _player;
    private readonly IMuHelperSettings? _config;
    private readonly MovementHandler _movementHandler;
    private readonly ConditionalSkillSlot[] _conditionalSkillSlots;

    private IAttackable? _currentTarget;
    private int _nearbyMonsterCount;
    private int _currentComboStep;
    private int _skillCooldownTicks;
    private int _approachFailures;
    private ushort _unreachableTargetId;
    private DateTime _unreachableTargetUntilUtc = DateTime.MinValue;
    private SkillEntry? _tickBestSkill;
    private bool _tickBestSkillComputed;
    private DateTime _engageAtUtc = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombatHandler"/> class.
    /// </summary>
    /// <param name="player">The offline player.</param>
    /// <param name="config">The MU helper settings.</param>
    /// <param name="movementHandler">The movement handler.</param>
    public CombatHandler(OfflinePlayer player, IMuHelperSettings? config, MovementHandler movementHandler)
    {
        this._player = player;
        this._config = config;
        this._movementHandler = movementHandler;
        this._conditionalSkillSlots = config is null ? [] :
        [
            new ConditionalSkillSlot(config.ActivationSkill1Id, config.Skill1UseTimer, config.DelayMinSkill1, config.Skill1UseCondition, config.Skill1ConditionAttacking, config.Skill1SubCondition),
            new ConditionalSkillSlot(config.ActivationSkill2Id, config.Skill2UseTimer, config.DelayMinSkill2, config.Skill2UseCondition, config.Skill2ConditionAttacking, config.Skill2SubCondition),
        ];
    }

    /// <summary>
    /// Gets the remaining skill cooldown ticks.
    /// </summary>
    public int SkillCooldownTicks => this._skillCooldownTicks;

    /// <summary>
    /// Gets the hunting range in tiles.
    /// </summary>
    public byte HuntingRange => CalculateHuntingRange(this._config);

    /// <summary>
    /// Gets the position to hunt around. Dynamic so bots can roam between hunting grounds.
    /// </summary>
    private Point OriginPosition => this._player.HuntingOrigin;

    /// <summary>
    /// Gets a value indicating whether this session animates a server-side bot rather than the offline
    /// session of a real player. Bots trade a bit of hunting efficiency for looking human (reaction
    /// delay, target spread); a player's offline session must behave exactly as it did before the bots
    /// moved into this handler.
    /// </summary>
    private bool IsBot => this._player.Account?.IsBot == true;

    /// <summary>
    /// Calculates the hunting range in tiles from the specified configuration.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <returns>The hunting range.</returns>
    public static byte CalculateHuntingRange(IMuHelperSettings? config)
    {
        if (config is null)
        {
            return DefaultRange;
        }

        return (byte)Math.Max(DefaultRange, config.HuntingRange);
    }

    /// <summary>
    /// Determines whether the monster is one the bot can fight without dying, judged by the monster's
    /// REAL combat stats instead of its nominal level: the average hit it lands (its base damage minus
    /// the bot's PvM defense, the same subtraction the damage formula applies) must not exceed
    /// <see cref="SafeHitHealthShare"/> of the bot's maximum health. A monster's level says nothing
    /// about its punch - the high-end maps (Swamp of Calmness, LaCleon, the event fortresses) field
    /// "level ~120" monsters which hit for 1000-2300 base damage, several times what regular maps'
    /// monsters of the same level deal - so a level cap sent high-level bots in modest gear straight
    /// into a death loop there. Judging by damage also scales naturally with equipment: better armor
    /// raises the bot's defense and unlocks tougher maps, exactly like it does for a real player.
    /// The bot's own offense must in turn exceed the monster's defense (<see cref="MinAttackAdvantage"/>),
    /// so it never besieges a tank monster it can barely scratch, and the monster's level must not
    /// exceed the bot's own (on reset servers: its reset-aware effective level, see
    /// <see cref="BotResetHandler.GetEffectiveLevel"/>).
    /// Shared by the combat AI and the bot navigator, so a bot never stops travelling for (or engages)
    /// a monster it should not fight.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="monster">The monster definition.</param>
    public static bool IsSafeTarget(Player player, MonsterDefinition monster)
    {
        var (monsterLevel, averageDamage, monsterDefense, monsterHealth, monsterAttackRate) = GetMonsterCombatStats(monster);
        if (monsterLevel <= 0 || player.Attributes is not { } attributes)
        {
            return false;
        }

        // Reset-aware: on servers with the reset feature a freshly reset character is nominally a
        // low level again but keeps the strength of its resets - the effective level keeps it from
        // being locked out of the maps it just hunted on.
        if (monsterLevel > BotResetHandler.GetEffectiveLevel(player))
        {
            return false;
        }

        var netHit = Math.Max(0f, averageDamage - attributes[Stats.DefensePvm]) * GetExpectedHitShare(player, monsterAttackRate);
        if (netHit > SafeHitHealthShare * Math.Max(1f, attributes[Stats.MaximumHealth]))
        {
            return false;
        }

        var attackPower = GetAttackPower(player);
        if (attackPower <= monsterDefense * MinAttackAdvantage)
        {
            return false;
        }

        return (attackPower - monsterDefense) * GetHitBudget(player, monsterLevel) >= monsterHealth;
    }

    /// <summary>
    /// The number of net hits the bot may take to kill the monster (see <see cref="MaxHitsToKill"/>),
    /// stretched by <see cref="MasterHitBudgetFactor"/> for a mastered bot fighting a monster which
    /// actually pays it master experience (see <see cref="MasterHitBudgetFactor"/>).
    /// </summary>
    private static int GetHitBudget(Player player, int monsterLevel)
    {
        var configuration = player.GameContext.Configuration;
        var isMastered = player.SelectedCharacter?.CharacterClass?.IsMasterClass == true
                         && (player.Attributes?[Stats.Level] ?? 0) >= configuration.MaximumLevel;

        return isMastered && monsterLevel >= configuration.MinimumMonsterLevelForMasterExperience
            ? MaxHitsToKill * MasterHitBudgetFactor
            : MaxHitsToKill;
    }

    /// <summary>
    /// Decrements the skill cooldown counter by one tick.
    /// </summary>
    public void DecrementCooldown()
    {
        if (this._skillCooldownTicks > 0)
        {
            this._skillCooldownTicks--;
        }
    }

    /// <summary>
    /// Performs combat attacks on targets.
    /// </summary>
    public async ValueTask PerformAttackAsync()
    {
        var previousTarget = this._currentTarget;
        this.RefreshTarget();

        if (this._currentTarget is null)
        {
            return;
        }

        // A human doesn't strike the very same instant a new target appears: give each fresh target a
        // small randomized reaction delay (the bot faces it, then engages) - the perfectly metronomic
        // instant-strike cadence is one of the clearest bot giveaways. Only bots pay for the disguise:
        // a player's own offline session must hunt exactly as fast as it always did.
        if (this.IsBot)
        {
            if (!ReferenceEquals(previousTarget, this._currentTarget))
            {
                this._engageAtUtc = DateTime.UtcNow.AddMilliseconds(Rand.NextInt(250, 900));
            }

            if (DateTime.UtcNow < this._engageAtUtc)
            {
                this._player.Rotation = this._player.GetDirectionTo(this._currentTarget);
                return;
            }
        }

        byte attackRange = this.GetEffectiveAttackRange();
        if (!this.IsTargetInAttackRange(this._currentTarget, attackRange))
        {
            if (await this._movementHandler.MoveCloserToTargetAsync(this._currentTarget, attackRange).ConfigureAwait(false))
            {
                this._approachFailures = 0;
            }
            else if (++this._approachFailures >= MaxApproachFailures)
            {
                // The target is in (Euclidean) range but no walkable path leads to it - e.g. a monster
                // across a wall or river. Blacklist it briefly and drop it, so the bot picks another
                // target (or moves on) instead of standing in front of the obstacle forever.
                this._unreachableTargetId = this._currentTarget.Id;
                this._unreachableTargetUntilUtc = DateTime.UtcNow + UnreachableTargetBlacklistDuration;
                this._currentTarget = null;
                this._approachFailures = 0;
            }

            return;
        }

        this._approachFailures = 0;

        if (this._config?.UseCombo == true)
        {
            await this.ExecuteComboAttackAsync().ConfigureAwait(false);
        }
        else
        {
            await this.ExecuteAttackAsync(this._currentTarget).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Performs health recovery through Drain Life attacks if configured.
    /// </summary>
    public async ValueTask PerformDrainLifeRecoveryAsync()
    {
        if (this._config is null || this._player.Attributes is null || !this._config.UseDrainLife)
        {
            return;
        }

        double maxHp = this._player.Attributes[Stats.MaximumHealth];
        if (maxHp <= 0)
        {
            return;
        }

        double hp = this._player.Attributes[Stats.CurrentHealth];
        int hpPercent = (int)(hp * 100.0 / maxHp);
        if (hpPercent <= this._config.HealThresholdPercent)
        {
            var drainSkill = this.FindDrainLifeSkill();
            if (drainSkill is not null)
            {
                this.RefreshTarget();
                if (this._currentTarget is not null)
                {
                    await this.ExecuteAttackAsync(this._currentTarget, drainSkill, false).ConfigureAwait(false);
                }
            }
        }
    }

    private static (int Level, float AverageDamage, float Defense, float Health, float AttackRate) GetMonsterCombatStats(MonsterDefinition monster)
    {
        return MonsterStatsCache.GetOrAdd(
            monster.Number,
            static (_, m) =>
            {
                float GetValue(AttributeDefinition attribute)
                    => m.Attributes.FirstOrDefault(a => a.AttributeDefinition == attribute)?.Value ?? 0f;

                var level = (int)GetValue(Stats.Level);
                var averageDamage = (GetValue(Stats.MinimumPhysBaseDmg) + GetValue(Stats.MaximumPhysBaseDmg)) / 2f;
                return (level, averageDamage, GetValue(Stats.DefenseBase), GetValue(Stats.MaximumHealth), GetValue(Stats.AttackRatePvm));
            },
            monster);
    }

    /// <summary>
    /// How much of the monster's average hit actually lands on the bot, over time: the engine rolls
    /// every monster swing against the bot's defense rate (see the hit chance in
    /// <see cref="AttackableExtensions"/>), so an agility-based character tanks by DODGING, not by
    /// soaking. Judging its safety by the raw hit alone declared every such build too squishy for
    /// anything past the starter maps - and left the whole caster population stuck on Lorencia.
    /// The dodge credit is capped (a bot must not bet its life on a lucky evade streak).
    /// </summary>
    private static float GetExpectedHitShare(Player player, float monsterAttackRate)
    {
        const float minimumAssumedHitChance = 0.25f;
        if (monsterAttackRate <= 0f || player.Attributes is not { } attributes)
        {
            return 1f;
        }

        var defenseRate = attributes[Stats.DefenseRatePvm];
        var hitChance = defenseRate < monsterAttackRate ? 1f - (defenseRate / monsterAttackRate) : 0.03f;
        return Math.Clamp(hitChance, minimumAssumedHitChance, 1f);
    }

    /// <summary>
    /// A rough estimate of the bot's punch: its best base damage kind (physical for fighters, wizardry
    /// for casters, curse for summoners) plus the strongest attack skill it has learned - enough to tell
    /// apart "kills this monster at a reasonable pace" from "barely scratches it".
    /// </summary>
    private static float GetAttackPower(Player player)
    {
        if (player.Attributes is not { } attributes)
        {
            return 0f;
        }

        var physical = (attributes[Stats.MinimumPhysBaseDmg] + attributes[Stats.MaximumPhysBaseDmg]) / 2f;

        // The Min/Max wizardry damage is what a caster actually hits with (energy feeds it, see the
        // class attribute relations); Stats.WizardryBaseDmg is only the bonus channel of the staff's
        // rise - reading it made every caster look like it had no offense at all, so it never passed
        // the checks below for anything but the starter maps and stayed there forever.
        var wizardry = (attributes[Stats.MinimumWizBaseDmg] + attributes[Stats.MaximumWizBaseDmg]) / 2f;
        var curse = (attributes[Stats.MinimumCurseBaseDmg] + attributes[Stats.MaximumCurseBaseDmg]) / 2f;
        var skillDamage = 0;
        foreach (var entry in player.SkillList?.Skills ?? [])
        {
            if (entry.Skill is { AttackDamage: > 0 } skill && skill.AttackDamage > skillDamage)
            {
                skillDamage = skill.AttackDamage;
            }
        }

        return Math.Max(physical, Math.Max(wizardry, curse)) + skillDamage;
    }

    private async ValueTask ExecuteAttackAsync(IAttackable target)
    {
        var skill = this.SelectAttackSkill();
        if (skill == null && this._config?.FallbackBasicAttack != true)
        {
            return;
        }

        await this.ExecuteAttackAsync(target, skill, false).ConfigureAwait(false);
    }

    private async ValueTask ExecuteAttackAsync(IAttackable target, SkillEntry? skillEntry, bool isCombo)
    {
        // Last line of defense for the "bot must never become an outlaw" invariant: no strike ever
        // leaves this handler against a player who isn't a legal PvP target right now.
        if (target is Player playerTarget && !BotPvpRules.IsLegalPvpTarget(this._player, playerTarget))
        {
            return;
        }

        this._player.Rotation = this._player.GetDirectionTo(target);

        if (skillEntry?.Skill is not { } skill)
        {
            await this.ExecutePhysicalAttackAsync(target).ConfigureAwait(false);
            return;
        }

        if (skill.SkillType is SkillType.AreaSkillAutomaticHits or SkillType.AreaSkillExplicitTarget or SkillType.AreaSkillExplicitHits)
        {
            await this.ExecuteAreaSkillAttackAsync(target, skillEntry, isCombo).ConfigureAwait(false);
        }
        else
        {
            await this.ExecuteTargetedSkillAttackAsync(target, skill).ConfigureAwait(false);
        }
    }

    private void RefreshTarget()
    {
        // The best-skill choice is cached for the duration of one tick (it is needed for both the range
        // check and the actual attack); a new tick starts with a fresh choice.
        this._tickBestSkill = null;
        this._tickBestSkillComputed = false;

        // Self-defense has priority over farming: a player who recently attacked this bot becomes the
        // target, as long as they are still viable and anywhere near. Without this the bot placidly
        // keeps hitting monsters while a player kills it. The aggressor memory only sets the PRIORITY,
        // though - whether the bot may actually strike is decided by BotPvpRules per attack: the grudge
        // outlives the game's self-defense window, and striking outside of it would turn the bot into
        // an outlaw (see BotPvpRules.IsLegalPvpTarget).
        if (this._config?.UseSelfDefense == true
            && this._player.RecentAggressor is { } aggressor
            && aggressor.IsInRange(this._player.Position, this.HuntingRange * 2)
            && BotPvpRules.IsLegalPvpTarget(this._player, aggressor))
        {
            this._currentTarget = aggressor;
            return;
        }

        if (this._currentTarget is { } t && !this.IsTargetStillValid(t))
        {
            this._currentTarget = null;
        }

        if (this._currentTarget is null)
        {
            var targets = this.GetAttackableTargetsInHuntingRange().ToList();
            var candidates = targets
                .Where(m => m.Id != this._unreachableTargetId || DateTime.UtcNow >= this._unreachableTargetUntilUtc)
                .OrderBy(m => m.GetDistanceTo(this._player))
                .Take(this.IsBot ? 2 : 1)
                .ToList();

            // A bot chooses randomly among the two nearest candidates instead of strictly the nearest
            // one: with many bots on one ground, deterministic nearest-first makes them all dogpile the
            // same monster and roam as a pack, which looks distinctly bot-like and wastes damage on
            // overkill. A player's own offline session keeps hitting the nearest monster - it is his
            // character's hunting efficiency, not a crowd to camouflage.
            this._currentTarget = candidates.SelectRandom();
            this._nearbyMonsterCount = targets.Count;
        }
        else
        {
            this._nearbyMonsterCount = this.GetAttackableTargetsInHuntingRange().Count();
        }
    }

    private IEnumerable<Monster> GetAttackableMonstersInHuntingRange()
    {
        if (this._player.CurrentMap is not { } map)
        {
            return [];
        }

        return map.GetAttackablesInRange(this.OriginPosition, this.HuntingRange)
            .OfType<Monster>()
            .Where(this.IsMonsterAttackable);
    }

    /// <summary>
    /// The regular target pool are the attackable monsters; inside a mini game which allows player
    /// killing (Chaos Castle) the other participants join it - there everyone is opposition, and a
    /// bot which placidly farms monsters while being cut down would be the obvious odd one out.
    /// </summary>
    private IEnumerable<IAttackable> GetAttackableTargetsInHuntingRange()
    {
        if (this._player.CurrentMap is not { } map)
        {
            return [];
        }

        var freeForAll = this._player.CurrentMiniGame is { AllowPlayerKilling: true };
        return map.GetAttackablesInRange(this.OriginPosition, this.HuntingRange)
            .Where(attackable => attackable switch
            {
                Monster monster => this.IsMonsterAttackable(monster),
                Player player => freeForAll && this.IsEventRivalAttackable(player),
                _ => false,
            });
    }

    private bool IsEventRivalAttackable(Player target)
    {
        return !ReferenceEquals(target, this._player)
               && target.IsAlive
               && !target.IsAtSafezone()
               && !target.IsTeleporting
               && BotPvpRules.IsLegalPvpTarget(this._player, target);
    }

    private bool IsTargetInAttackRange(IAttackable target, byte range)
    {
        return target.IsInRange(this._player.Position, range);
    }

    private bool IsTargetStillValid(IAttackable target)
    {
        // A player target must stay legal for the whole fight: the self-defense window can expire
        // mid-fight (the player stopped hitting back and ran), and every further strike past that
        // point would be an unprovoked attack that escalates the bot's own hero state.
        if (target is Player playerTarget && !BotPvpRules.IsLegalPvpTarget(this._player, playerTarget))
        {
            return false;
        }

        return target.IsAlive
               && !target.IsAtSafezone()
               && !target.IsTeleporting
               && target.IsInRange(this.OriginPosition, this.HuntingRange);
    }

    private bool IsMonsterAttackable(Monster monster)
    {
        return monster.IsAlive
               && !monster.IsAtSafezone()
               && monster.Definition.ObjectKind == NpcObjectKind.Monster
               && this.IsWithinSafeHuntLevel(monster);
    }

    /// <summary>
    /// With <see cref="IMuHelperSettings.OnlyHuntSafeMonsters"/> (server-side bots), the combat AI only
    /// engages monsters which pass the same <see cref="IsSafeTarget"/> check the bot navigator hunts by.
    /// Without this, a bot travelling through hostile territory picks a fight with any monster that
    /// comes within range - including ones far too strong - and dies. Human offline sessions keep the
    /// unrestricted behavior, since the player chose their hunting spot deliberately.
    /// </summary>
    private bool IsWithinSafeHuntLevel(Monster monster)
    {
        if (this._config?.OnlyHuntSafeMonsters != true)
        {
            return true;
        }

        if (this._player.CurrentMiniGame is not null)
        {
            // Inside a mini game event the opposition is not the bot's choice - it fights what
            // the event throws at it, like every other participant. Refusing "unsafe" waves
            // would leave the bot idling in the middle of a Blood Castle.
            return true;
        }

        return IsSafeTarget(this._player, monster.Definition);
    }

    private async ValueTask ExecutePhysicalAttackAsync(IAttackable target)
    {
        await target.AttackByAsync(this._player, null, false).ConfigureAwait(false);

        await this._player.ForEachWorldObserverAsync<IShowAnimationPlugIn>(
            p => p.ShowAnimationAsync(this._player, 120, target, this._player.Rotation),
            includeThis: true).ConfigureAwait(false);
    }

    private async ValueTask ExecuteAreaSkillAttackAsync(IAttackable target, SkillEntry skillEntry, bool isCombo)
    {
        var skill = skillEntry.Skill!;

        if (isCombo)
        {
            await this._player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(
                p => p.ShowComboAnimationAsync(this._player, target),
                includeThis: true).ConfigureAwait(false);
        }

        var rotationByte = (byte)(this._player.Position.GetAngleDegreeTo(target.Position) / 360.0 * 255.0);
        await this._player.ForEachWorldObserverAsync<IShowAreaSkillAnimationPlugIn>(
            p => p.ShowAreaSkillAnimationAsync(this._player, skill, target.Position, rotationByte),
            includeThis: true).ConfigureAwait(false);

        var monstersInRange = this._player.CurrentMap?
            .GetAttackablesInRange(target.Position, skill.Range)
            .OfType<Monster>()
            .Where(this.IsMonsterAttackable)
            ?? [];

        foreach (var monster in monstersInRange)
        {
            await monster.AttackByAsync(this._player, skillEntry, isCombo).ConfigureAwait(false);
        }

        // A player target is hit by the area skill as well. Outside of free-for-all events ONLY the
        // target itself (the self-defense aggressor): any bystanding player in the blast radius is
        // deliberately spared - a bot's self-defense must never splash uninvolved players, no
        // matter what it casts. Inside a mini game with free player killing (Chaos Castle) there
        // are no uninvolved players, so the skill splashes the other participants like any area
        // skill would. The legality re-check right at the strike closes the last race: the target
        // was legal when it was picked, but the situation may have changed in the meantime.
        IEnumerable<Player> playerTargets;
        if (this._player.CurrentMiniGame is { AllowPlayerKilling: true })
        {
            playerTargets = this._player.CurrentMap?
                    .GetAttackablesInRange(target.Position, skill.Range)
                    .OfType<Player>()
                    .Where(p => !ReferenceEquals(p, this._player))
                ?? [];
        }
        else
        {
            playerTargets = target is Player playerTarget ? [playerTarget] : [];
        }

        foreach (var player in playerTargets)
        {
            if (player.IsAlive && !player.IsAtSafezone()
                && BotPvpRules.IsLegalPvpTarget(this._player, player))
            {
                await player.AttackByAsync(this._player, skillEntry, isCombo).ConfigureAwait(false);
            }
        }
    }

    private async ValueTask ExecuteTargetedSkillAttackAsync(IAttackable target, Skill skill)
    {
        var strategy = this._player.GameContext.PlugInManager.GetStrategy<short, ITargetedSkillPlugin>(skill.Number)
            ?? DefaultPlugin;
        await strategy.PerformSkillAsync(this._player, target, (ushort)skill.Number).ConfigureAwait(false);
    }

    private SkillEntry? SelectAttackSkill()
    {
        if (this._config is null)
        {
            return null;
        }

        // If no skills are configured at all, don't attack - unless the AI is allowed to pick a skill
        // on its own (bots), in which case we fall through to the automatic selection below.
        if (this._config.BasicSkillId == 0
            && this._config.ActivationSkill1Id == 0
            && this._config.ActivationSkill2Id == 0
            && !this._config.AutoSelectBestSkill)
        {
            return null;
        }

        foreach (var slot in this._conditionalSkillSlots)
        {
            var skill = this.EvaluateConditionalSkill(slot);
            if (skill is not null && this.HasEnoughResources(skill))
            {
                return skill;
            }
        }

        if (this._config.BasicSkillId > 0)
        {
            var basicSkill = this._player.SkillList?.GetSkill((ushort)this._config.BasicSkillId);
            if (basicSkill is not null && this.HasEnoughResources(basicSkill))
            {
                return basicSkill;
            }
        }

        // No explicitly configured skill fired: let the AI pick the strongest affordable learned attack
        // skill. This scales with the character's level and mana pool, so higher-level bots naturally cast
        // stronger spells, and drop back to a basic attack (via FallbackBasicAttack) only when out of mana.
        if (this._config.AutoSelectBestSkill)
        {
            return this.SelectBestAffordableSkill();
        }

        return null;
    }

    /// <summary>
    /// Picks the strongest attack skill the character has learned and can currently afford (enough mana
    /// and ability). Only attack skills (direct hit or area damage) are considered; learned skills are
    /// always class-qualified, so this can never cast a skill the class is not entitled to.
    /// </summary>
    private SkillEntry? SelectBestAffordableSkill()
    {
        if (this._tickBestSkillComputed)
        {
            // Computed once per tick: both the attack-range check and the attack itself need it.
            return this._tickBestSkill;
        }

        this._tickBestSkillComputed = true;
        if (this._player.SkillList is not { } skillList)
        {
            return null;
        }

        SkillEntry? best = null;
        var bestDamage = 0;
        foreach (var entry in skillList.Skills)
        {
            if (entry.Skill is not { } skill || skill.AttackDamage <= 0)
            {
                continue;
            }

            if (skill.SkillType is not (SkillType.DirectHit
                or SkillType.AreaSkillAutomaticHits
                or SkillType.AreaSkillExplicitHits
                or SkillType.AreaSkillExplicitTarget))
            {
                continue;
            }

            if (skill.AttackDamage > bestDamage && this.HasEnoughResources(entry))
            {
                best = entry;
                bestDamage = skill.AttackDamage;
            }
        }

        this._tickBestSkill = best;
        return best;
    }

    /// <summary>
    /// Evaluates whether the skill in the given slot should fire this tick.
    /// </summary>
    private SkillEntry? EvaluateConditionalSkill(ConditionalSkillSlot slot)
    {
        if (slot.SkillId <= 0)
        {
            return null;
        }

        if (slot.UseTimer && !slot.UseCondition)
        {
            if (slot.TimerIntervalSeconds <= 0)
            {
                return null;
            }

            var secondsSinceLastUse = (DateTime.UtcNow - slot.LastUseTime).TotalSeconds;
            if (secondsSinceLastUse >= slot.TimerIntervalSeconds)
            {
                slot.LastUseTime = DateTime.UtcNow;
                return this._player.SkillList?.GetSkill((ushort)slot.SkillId);
            }

            return null;
        }

        if (slot.UseCondition && !slot.UseTimer)
        {
            int threshold = slot.SubCondition switch
            {
                0 => 2,
                1 => 3,
                2 => 4,
                3 => 5,
                _ => int.MaxValue,
            };

            int monsterCount = slot.ConditionAttacking
                ? this.CountMonstersAttackingPlayer()
                : this._nearbyMonsterCount;

            if (monsterCount >= threshold)
            {
                return this._player.SkillList?.GetSkill((ushort)slot.SkillId);
            }
        }

        return null;
    }

    private int CountMonstersAttackingPlayer()
    {
        return this.GetAttackableMonstersInHuntingRange()
            .Count(m => m.IsTargetingPlayer(this._player));
    }

    /// <summary>
    /// Checks that the player has enough mana AND ability (AG) to cast the skill.
    /// Both resources are consumed by skills via <see cref="Skill.ConsumeRequirements"/>.
    /// </summary>
    private bool HasEnoughResources(SkillEntry? skillEntry)
    {
        if (skillEntry?.Skill is not { } skill || this._player.Attributes is null)
        {
            return true;
        }

        foreach (var requirement in skill.ConsumeRequirements)
        {
            int required = this._player.GetRequiredValue(requirement);
            if (this._player.Attributes[requirement.Attribute] < required)
            {
                return false;
            }
        }

        return true;
    }

    private async ValueTask ExecuteComboAttackAsync()
    {
        if (this._currentTarget is null)
        {
            this._currentComboStep = 0;
            return;
        }

        var ids = this.GetConfiguredComboSkillIds();
        if (ids.Count < MinComboSkillCount)
        {
            await this.ExecuteAttackAsync(this._currentTarget).ConfigureAwait(false);
            return;
        }

        var skillId = (ushort)ids[this._currentComboStep % ids.Count];
        var skillEntry = this._player.SkillList?.GetSkill(skillId);

        if (skillEntry?.Skill is { } currentSkill && !this._currentTarget.IsInRange(this._player.Position, (byte)currentSkill.Range + 1))
        {
            this._currentComboStep = 0;
            return;
        }

        bool isAreaSkill = skillEntry?.Skill?.SkillType is SkillType.AreaSkillAutomaticHits or SkillType.AreaSkillExplicitTarget or SkillType.AreaSkillExplicitHits;
        var comboState = this._player.ComboState;
        var stateBefore = comboState?.CurrentState;

        if (isAreaSkill)
        {
            bool isActuallyCombo = false;
            if (skillEntry?.Skill is { } skill && comboState is not null)
            {
                isActuallyCombo = await comboState.RegisterSkillAsync(skill).ConfigureAwait(false);
            }

            await this.ExecuteAttackAsync(this._currentTarget, skillEntry, isActuallyCombo).ConfigureAwait(false);

            if (isActuallyCombo)
            {
                this._skillCooldownTicks = ComboFinisherDelayTicks;
                this._currentComboStep = 0;
            }
            else
            {
                this._skillCooldownTicks = InterSkillDelayTicks;
                this._currentComboStep++;
            }
        }
        else
        {
            await this.ExecuteAttackAsync(this._currentTarget, skillEntry, false).ConfigureAwait(false);

            var stateAfter = comboState?.CurrentState;
            if (stateBefore != comboState?.InitialState && stateAfter == comboState?.InitialState)
            {
                this._skillCooldownTicks = ComboFinisherDelayTicks;
                this._currentComboStep = 0;
            }
            else
            {
                this._skillCooldownTicks = InterSkillDelayTicks;
                this._currentComboStep++;
            }
        }
    }

    private List<int> GetConfiguredComboSkillIds()
    {
        var ids = new List<int>();
        if (this._config is not null)
        {
            if (this._config.BasicSkillId > 0)
            {
                ids.Add(this._config.BasicSkillId);
            }

            if (this._config.ActivationSkill1Id > 0)
            {
                ids.Add(this._config.ActivationSkill1Id);
            }

            if (this._config.ActivationSkill2Id > 0)
            {
                ids.Add(this._config.ActivationSkill2Id);
            }
        }

        return ids;
    }

    private byte GetEffectiveAttackRange()
    {
        if (this._config is null)
        {
            return DefaultRange;
        }

        var skillIds = this._config.UseCombo
            ? this.GetConfiguredComboSkillIds()
            : (this._config.BasicSkillId > 0 ? [this._config.BasicSkillId] : []);

        if (skillIds.Count > 0)
        {
            var ranges = skillIds
                .Select(id => this._player.SkillList?.GetSkill((ushort)id)?.Skill?.Range ?? 0)
                .Where(r => r > 0)
                .ToList();

            if (ranges.Count > 0)
            {
                return (byte)ranges.Min();
            }
        }

        // Bots have no configured skill IDs but auto-select their attack skill; use the range of the skill
        // they would actually cast now, so ranged casters attack from a distance instead of closing to melee.
        if (this._config.AutoSelectBestSkill
            && this.SelectBestAffordableSkill()?.Skill?.Range is { } autoRange
            && autoRange > 0)
        {
            return (byte)autoRange;
        }

        if (this._player.Attributes is { } attributes
            && (attributes[Stats.IsBowEquipped] > 0 || attributes[Stats.IsCrossBowEquipped] > 0))
        {
            return BowRange;
        }

        return DefaultRange;
    }

    private SkillEntry? FindDrainLifeSkill()
    {
        return this._player.SkillList?.Skills.FirstOrDefault(s =>
            s.Skill is { Number: DrainLifeBaseSkillId or DrainLifeStrengthenerSkillId or DrainLifeMasterySkillId });
    }

    /// <summary>
    /// Holds the configuration for one conditional skill slot, along with its mutable last-use timestamp.
    /// </summary>
    private sealed class ConditionalSkillSlot(int skillId, bool useTimer, int timerIntervalSeconds, bool useCondition, bool conditionAttacking, int subCondition)
    {
        public int SkillId { get; } = skillId;
        public bool UseTimer { get; } = useTimer;
        public int TimerIntervalSeconds { get; } = timerIntervalSeconds;
        public bool UseCondition { get; } = useCondition;
        public bool ConditionAttacking { get; } = conditionAttacking;
        public int SubCondition { get; } = subCondition;
        public DateTime LastUseTime { get; set; }
    }
}