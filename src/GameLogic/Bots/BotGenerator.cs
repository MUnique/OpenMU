// <copyright file="BotGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Generates and maintains the persistent population of bot accounts and their characters.
/// </summary>
/// <remarks>
/// Accounts are flagged with <see cref="Account.IsBot"/> so they can be reliably reloaded on
/// startup (instead of being regenerated) and purged on request. The account login names follow
/// a deterministic, internal scheme (<see cref="GetLoginName"/>) which is never shown to other
/// players; the player-visible character names are realistic and unique (see <see cref="BotNameGenerator"/>).
/// Generation is idempotent: only the missing accounts are created, so it is safe to run on every start.
/// </remarks>
internal sealed class BotGenerator
{
    private const string LoginPrefix = "bot";
    private const int MinLevel = 10;
    private const int MaxLevel = 80;
    private const int StartMoney = 100000;

    /// <summary>Upgrade level (+6) of the starter gear, giving fresh bots a survival buffer until they can warp.</summary>
    private const byte StarterItemLevel = 6;

    /// <summary>
    /// Skill-tier scaling: a class attack skill becomes learnable once the character level reaches roughly
    /// its attack-damage rating times this factor. Attack damage is a monotonic proxy for the skill tier
    /// within a class (e.g. Dark Wizard: Energy Ball 3 → Hellfire 120), so higher-level bots progressively
    /// unlock stronger spells while low-level ones only get the basics - always only for their own class.
    /// </summary>
    private const double SkillLearnDamageFactor = 0.9;

    /// <summary>Highest item group that is a melee weapon (0 sword, 1 axe, 2 mace, 3 spear).</summary>
    private const byte MaxMeleeGroup = 3;

    /// <summary>Item group of bows (need ammunition).</summary>
    private const byte BowGroup = 4;

    /// <summary>Item group of staves/sticks (casters).</summary>
    private const byte StaffGroup = 5;

    /// <summary>Item group of body armor; its item number identifies the armor set.</summary>
    private const byte ArmorGroup = 8;

    /// <summary>
    /// Armor set numbers tried in thematic order; the first the class is qualified for (by its chest piece)
    /// is used: 5 Leather (warriors), 2 Pad (wizards), 10 Vine (elves), 39 Mistery (summoners), then fallbacks.
    /// </summary>
    private static readonly byte[] ArmorSetCandidates = { 5, 2, 10, 39, 6, 0, 4, 8 };

    private readonly IGameContext _gameContext;
    private readonly ILogger _logger;
    private readonly BotNameGenerator _nameGenerator = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BotGenerator"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="logger">The logger.</param>
    public BotGenerator(IGameContext gameContext, ILogger logger)
    {
        this._gameContext = gameContext;
        this._logger = logger;
    }

    /// <summary>
    /// Gets the deterministic, internal login name of the bot account with the given one-based index.
    /// </summary>
    /// <param name="index">The one-based account index.</param>
    /// <returns>The login name, e.g. <c>bot0001</c> (kept within the 10 character account name limit).</returns>
    public static string GetLoginName(int index) => $"{LoginPrefix}{index:D4}";

    /// <summary>
    /// Ensures that the configured number of bot accounts (each with the configured number of
    /// characters) exists. Only missing accounts are created.
    /// </summary>
    /// <param name="numberOfAccounts">The desired number of bot accounts.</param>
    /// <param name="charactersPerAccount">The desired number of characters per account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of accounts that were newly created.</returns>
    public async ValueTask<int> EnsureBotsAsync(int numberOfAccounts, int charactersPerAccount, CancellationToken cancellationToken = default)
    {
        var creatableClasses = this._gameContext.Configuration.CharacterClasses
            .Where(c => c is { CanGetCreated: true, HomeMap: not null })
            .ToList();
        if (creatableClasses.Count == 0)
        {
            this._logger.LogWarning("No creatable character classes found - cannot generate bots.");
            return 0;
        }

        var perAccount = Math.Clamp(
            Math.Min(charactersPerAccount, this._gameContext.Configuration.MaximumCharactersPerAccount),
            1,
            BotConfiguration.MaxCharactersPerAccountLimit);

        var experienceTable = this._gameContext.ExperienceTable;
        var maxLevel = Math.Min(MaxLevel, experienceTable.Length - 1);
        var minLevel = Math.Clamp(MinLevel, 1, maxLevel);

        using var context = this._gameContext.PersistenceContextProvider.CreateNewPlayerContext(this._gameContext.Configuration);
        var reservedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var created = 0;

        // Build a balanced, shuffled queue of classes so the whole population is evenly split across
        // all creatable classes. Independent random draws leave visible skew at this scale (e.g. 11
        // Summoners vs 4 Elves for 50 bots); the quota queue guarantees ~even counts, drawn per character.
        var classQueue = BuildBalancedClassQueue(creatableClasses, numberOfAccounts * perAccount);

        for (var i = 1; i <= numberOfAccounts; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var loginName = GetLoginName(i);
            var existing = await context.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false);
            if (existing is not null)
            {
                continue;
            }

            var account = context.CreateNew<Account>();
            account.LoginName = loginName;
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString());
            account.IsBot = true;
            account.Vault = context.CreateNew<ItemStorage>();

            for (byte slot = 0; slot < perAccount; slot++)
            {
                var characterClass = classQueue.Count > 0 ? classQueue.Dequeue() : creatableClasses.SelectRandom()!;
                var level = Rand.NextInt(minLevel, maxLevel + 1);
                var name = await this._nameGenerator.GenerateUniqueAsync(context, reservedNames, cancellationToken).ConfigureAwait(false);
                this.CreateCharacter(context, account, name, characterClass, level, slot, experienceTable);
            }

            // Save per account so a single failure does not roll back already generated accounts,
            // and re-runs simply resume where they left off (idempotent).
            if (await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false))
            {
                created++;
                this._logger.LogInformation("Generated bot account '{LoginName}' with {Count} character(s).", loginName, perAccount);
            }
        }

        return created;
    }

    /// <summary>
    /// Builds a shuffled queue of character classes with even quotas across <paramref name="classes"/>,
    /// so the generated population is balanced instead of relying on the variance of independent random
    /// draws. The order is randomized so accounts do not get a predictable class pattern.
    /// </summary>
    private static Queue<CharacterClass> BuildBalancedClassQueue(IList<CharacterClass> classes, int total)
    {
        var pool = new List<CharacterClass>(total);
        for (var n = 0; n < total; n++)
        {
            // Even quotas: class index cycles, so each class appears total/count times (+1 for the first remainder classes).
            pool.Add(classes[n % classes.Count]);
        }

        // Fisher-Yates shuffle so the balanced pool is handed out in random order.
        for (var n = pool.Count - 1; n > 0; n--)
        {
            var j = Rand.NextInt(0, n + 1);
            (pool[n], pool[j]) = (pool[j], pool[n]);
        }

        return new Queue<CharacterClass>(pool);
    }

    /// <summary>
    /// Deletes all bot accounts (and, by cascade, their characters and owned data).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of deleted bot accounts.</returns>
    public async ValueTask<int> DeleteAllBotsAsync(CancellationToken cancellationToken = default)
    {
        using var context = this._gameContext.PersistenceContextProvider.CreateNewPlayerContext(this._gameContext.Configuration);
        var deleted = 0;
        const int pageSize = 100;
        var skip = 0;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var page = (await context.GetAccountsOrderedByLoginNameAsync(skip, pageSize, cancellationToken).ConfigureAwait(false)).ToList();
            if (page.Count == 0)
            {
                break;
            }

            var bots = page.Where(a => a.IsBot).ToList();
            foreach (var bot in bots)
            {
                if (await context.DeleteAsync(bot).ConfigureAwait(false))
                {
                    deleted++;
                }
            }

            // Commit this page's deletions before paging on, so the ordering used by the next query
            // reflects the removals: the non-bot accounts of this page now occupy [skip, skip + nonBotCount).
            if (bots.Count > 0)
            {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            skip += page.Count - bots.Count;
        }

        return deleted;
    }

    private static byte[] CreateDefaultKeyConfiguration()
    {
        // Mirrors CreateCharacterAction: bind Q to the healing potion and W to the mana potion,
        // leave E and R unbound. An all-zero blob would otherwise bind the apple (a heal) to all slots.
        const byte healingPotion = 1;
        const byte manaPotion = 4;
        const byte unbound = 0xFF;

        var keyConfiguration = new byte[30];
        keyConfiguration[21] = healingPotion; // Q
        keyConfiguration[22] = manaPotion; // W
        keyConfiguration[23] = unbound; // E
        keyConfiguration[25] = unbound; // R
        return keyConfiguration;
    }

    private void CreateCharacter(IPlayerContext context, Account account, string name, CharacterClass characterClass, int level, byte slot, long[] experienceTable)
    {
        var character = context.CreateNew<Character>();
        character.CharacterClass = characterClass;
        character.Name = name;
        character.CharacterSlot = slot;
        character.CreateDate = DateTime.UtcNow;
        character.KeyConfiguration = CreateDefaultKeyConfiguration();

        foreach (var attribute in characterClass.StatAttributes.Select(a => context.CreateNew<StatAttribute>(a.Attribute, a.BaseValue)))
        {
            character.Attributes.Add(attribute);
        }

        character.CurrentMap = characterClass.HomeMap;
        var spawnGate = character.CurrentMap!.ExitGates.Where(g => g.IsSpawnGate).SelectRandom();
        if (spawnGate is not null)
        {
            character.PositionX = (byte)Rand.NextInt(spawnGate.X1, spawnGate.X2);
            character.PositionY = (byte)Rand.NextInt(spawnGate.Y1, spawnGate.Y2);
        }

        var levelAttribute = character.Attributes.First(a => a.Definition == Stats.Level);
        levelAttribute.Value = level;
        character.Experience = experienceTable[Math.Min(level, experienceTable.Length - 1)];
        character.LevelUpPoints = (int)((level - 1)
            * characterClass.StatAttributes.First(a => a.Attribute == Stats.PointsPerLevelUp).BaseValue);
        DistributeStatPoints(character);

        this.LearnClassSkills(context, character, characterClass, level);

        character.Inventory = context.CreateNew<ItemStorage>();
        character.Inventory.Money = StartMoney;
        this.EquipStarterGear(context, character);

        account.Characters.Add(character);
    }

    /// <summary>
    /// Spends the character's level-up points, so a high-level bot actually has high-level stats.
    /// Without this a generated level-80 bot would fight with level-1 base stats (tiny health and
    /// damage) and die instantly. Half of the points go into vitality (health/survival) and half into
    /// the class's main damage stat.
    /// </summary>
    private static void DistributeStatPoints(Character character)
    {
        var points = character.LevelUpPoints;
        if (points <= 0)
        {
            return;
        }

        var mainStat = GetMainDamageStat(character);
        var vitality = character.Attributes.FirstOrDefault(a => a.Definition == Stats.BaseVitality);
        if (mainStat is null || vitality is null)
        {
            return;
        }

        var toVitality = points / 2;
        vitality.Value += toVitality;
        mainStat.Value += points - toVitality;
        character.LevelUpPoints = 0;
    }

    /// <summary>
    /// Teaches the character the class attack skills appropriate to its level, so bots fight with spells and
    /// skills instead of only their weapon. Only skills the class is qualified for are ever learned (so a bot
    /// can never end up with another class's magic), gated by a per-skill learn level derived from the skill's
    /// attack damage (its tier). Combined with the runtime auto-selection in <see cref="Offline.CombatHandler"/>,
    /// the character casts the strongest of these it can currently afford.
    /// </summary>
    private void LearnClassSkills(IPlayerContext context, Character character, CharacterClass characterClass, int level)
    {
        var learnedNumbers = new HashSet<short>(character.LearnedSkills.Select(s => s.Skill!.Number));
        foreach (var skill in this._gameContext.Configuration.Skills)
        {
            if (skill.AttackDamage <= 0
                || skill.SkillType is not (SkillType.DirectHit
                    or SkillType.AreaSkillAutomaticHits
                    or SkillType.AreaSkillExplicitHits
                    or SkillType.AreaSkillExplicitTarget))
            {
                continue;
            }

            if (!skill.QualifiedCharacters.Contains(characterClass) || !learnedNumbers.Add(skill.Number))
            {
                continue;
            }

            var learnLevel = Math.Max(1, (int)Math.Ceiling(skill.AttackDamage * SkillLearnDamageFactor));
            if (level < learnLevel)
            {
                learnedNumbers.Remove(skill.Number);
                continue;
            }

            var entry = context.CreateNew<SkillEntry>();
            entry.Skill = skill;
            entry.Level = 0;
            character.LearnedSkills.Add(entry);
        }
    }

    private static StatAttribute? GetMainDamageStat(Character character)
    {
        return character.Attributes
            .Where(a => a.Definition == Stats.BaseStrength
                        || a.Definition == Stats.BaseAgility
                        || a.Definition == Stats.BaseEnergy
                        || a.Definition == Stats.BaseLeadership)
            .OrderByDescending(a => a.Value)
            .FirstOrDefault();
    }

    /// <summary>
    /// Equips the bot with a basic, class-appropriate weapon and armor set (mirrors the low-level test
    /// account gear), so it is not naked and punching with its fists. The item level scales modestly
    /// with the bot level for a bit more defense/damage without raising the equip requirements too high.
    /// </summary>
    private void EquipStarterGear(IPlayerContext context, Character character)
    {
        var inventory = character.Inventory!;
        var characterClass = character.CharacterClass!;

        // Data-driven so every class gets gear it is actually QUALIFIED to wear (a Dark Lord must never
        // end up in a Pad/wizard set). We pick the most basic options (lowest DropLevel) the class can use:
        // - a weapon from the weapon groups (0 sword, 1 axe, 2 mace, 3 spear, 4 bow, 5 staff),
        // - the armor set whose chest piece (group 8) has the lowest DropLevel; its NUMBER identifies the set,
        //   and the equipment type is the GROUP (7 helm, 8 armor, 9 pants, 10 gloves, 11 boots).
        // Choose the weapon type by the class's intrinsic stats: archers (agility) get a bow, pure casters
        // (energy) a staff, everyone else (warriors and the Magic Gladiator hybrid) a melee blade. The Small
        // Axe is qualified for almost every class, so without this casters and archers would all end up with one.
        float ClassStat(AttributeDefinition attribute)
            => characterClass.StatAttributes.FirstOrDefault(a => a.Attribute == attribute)?.BaseValue ?? 0f;
        var strength = ClassStat(Stats.BaseStrength);
        var agility = ClassStat(Stats.BaseAgility);
        var energy = ClassStat(Stats.BaseEnergy);
        Func<ItemDefinition, bool> isPreferredWeapon;
        if (agility > strength && agility > energy)
        {
            isPreferredWeapon = d => d.Group == BowGroup;
        }
        else if (energy > strength)
        {
            isPreferredWeapon = d => d.Group == StaffGroup;
        }
        else
        {
            isPreferredWeapon = d => d.Group <= MaxMeleeGroup;
        }

        var weapon = this._gameContext.Configuration.Items
                .Where(d => isPreferredWeapon(d) && d.QualifiedCharacters.Contains(characterClass))
                .MinBy(d => d.DropLevel)
            ?? this._gameContext.Configuration.Items
                .Where(d => d.Group <= StaffGroup && d.QualifiedCharacters.Contains(characterClass))
                .MinBy(d => d.DropLevel);
        if (weapon is not null)
        {
            if (weapon.Group == BowGroup)
            {
                // Bows need ammunition; the arrows go into the left hand.
                this.AddEquippedItem(context, inventory, characterClass, InventoryConstants.RightHandSlot, weapon);
                this.AddAmmunition(context, inventory);
            }
            else
            {
                this.AddEquippedItem(context, inventory, characterClass, InventoryConstants.LeftHandSlot, weapon);
            }
        }

        // Choose a thematically appropriate armor set the class can wear, tried in order (warriors -> Leather,
        // wizards -> Pad, elves -> Vine, summoners -> Mistery, then fallbacks). Each piece is added only if the
        // class is qualified for it, so e.g. the Magic Gladiator keeps the set but skips the helm it can't wear.
        foreach (var set in ArmorSetCandidates)
        {
            if (this._gameContext.Configuration.Items.FirstOrDefault(d => d.Group == ArmorGroup && d.Number == set) is not { } chest
                || !chest.QualifiedCharacters.Contains(characterClass))
            {
                continue;
            }

            this.EquipArmorPiece(context, inventory, characterClass, InventoryConstants.HelmSlot, 7, set);
            this.EquipArmorPiece(context, inventory, characterClass, InventoryConstants.ArmorSlot, 8, set);
            this.EquipArmorPiece(context, inventory, characterClass, InventoryConstants.PantsSlot, 9, set);
            this.EquipArmorPiece(context, inventory, characterClass, InventoryConstants.GlovesSlot, 10, set);
            this.EquipArmorPiece(context, inventory, characterClass, InventoryConstants.BootsSlot, 11, set);
            break;
        }

        this.AddHealthPotions(context, inventory);
    }

    private void AddHealthPotions(IPlayerContext context, ItemStorage inventory)
    {
        // A stack of Large Healing Potions so the offline HealingHandler has something to drink. The
        // BotNavigator tops this up at runtime, so the bot never runs dry. Durability holds the stack count.
        var potion = this._gameContext.Configuration.Items.FirstOrDefault(d => d.Group == 14 && d.Number == 3);
        if (potion is null)
        {
            return;
        }

        var item = context.CreateNew<Item>();
        item.Definition = potion;
        item.Durability = 255;
        item.ItemSlot = InventoryConstants.EquippableSlotsCount; // first backpack slot
        inventory.Items.Add(item);
    }

    private void EquipArmorPiece(IPlayerContext context, ItemStorage inventory, CharacterClass characterClass, byte slot, int group, int number)
    {
        var definition = this._gameContext.Configuration.Items.FirstOrDefault(d => d.Group == group && d.Number == number);
        if (definition is null || !definition.QualifiedCharacters.Contains(characterClass))
        {
            return;
        }

        this.AddEquippedItem(context, inventory, characterClass, slot, definition);
    }

    private void AddEquippedItem(IPlayerContext context, ItemStorage inventory, CharacterClass characterClass, byte slot, ItemDefinition definition)
    {
        if (!definition.QualifiedCharacters.Contains(characterClass))
        {
            return;
        }

        var item = context.CreateNew<Item>();
        item.Definition = definition;
        item.Level = StarterItemLevel;
        item.Durability = definition.Durability;
        item.ItemSlot = slot;
        inventory.Items.Add(item);
    }

    private void AddAmmunition(IPlayerContext context, ItemStorage inventory)
    {
        var arrows = this._gameContext.Configuration.Items.FirstOrDefault(d => d.Group == 4 && d.Number == 15);
        if (arrows is null)
        {
            return;
        }

        var item = context.CreateNew<Item>();
        item.Definition = arrows;
        item.Durability = 255;
        item.ItemSlot = InventoryConstants.LeftHandSlot;
        inventory.Items.Add(item);
    }
}
