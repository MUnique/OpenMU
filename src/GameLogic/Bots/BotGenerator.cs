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
using MUnique.OpenMU.GameLogic.Resets;
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

    /// <summary>
    /// BCrypt work factor for a bot account's password. The password is a random <see cref="Guid"/>
    /// which is discarded immediately and never used to log in - a bot is a connection-less
    /// <c>OfflinePlayer</c>, so no client ever authenticates against it. A minimal factor is therefore
    /// safe (a 128-bit random secret is infeasible to brute-force regardless of the factor) and keeps
    /// generating a large population from becoming a multi-minute BCrypt bottleneck, while still storing
    /// a valid BCrypt hash. The default factor is kept for real accounts.
    /// </summary>
    private const int BotPasswordWorkFactor = 4;

    private const int MinLevel = 10;

    /// <summary>
    /// The highest generated level. High enough that the upper maps (Tarkan, Aida, Kanturu, ...) get a
    /// resident bot population and that some bots start beyond the class evolution level
    /// (<see cref="BotProgression.ClassEvolutionLevel"/>) - those are created as their second-generation
    /// class right away, like a player who did the class quest long ago.
    /// </summary>
    private const int MaxLevel = 250;

    /// <summary>
    /// Skew of the level distribution: values above 1 make low and mid levels more common than high
    /// ones, like a real server's population pyramid (an even spread would feel top-heavy).
    /// </summary>
    private const double LevelSkew = 1.6;
    private const int StartMoney = 100000;

    /// <summary>Upgrade level (+6) of the starter gear, giving fresh bots a survival buffer until they can warp.</summary>
    private const byte StarterItemLevel = 6;

    /// <summary>Number of inventory extensions (each 4 rows of 8 slots) a bot gets, so loot does not clog its backpack.</summary>
    private const int BotInventoryExtensions = 4;

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

        // On servers with the reset feature the existing population has resets, so freshly generated
        // bots get a random reset history too - a visitor should meet believable veterans (even TOP,
        // max-reset characters), not a population uniformly starting from zero. Only possible when the
        // configuration bounds the resets; unlimited-reset servers keep unseeded bots.
        var resetConfiguration = BotResetHandler.GetResetConfiguration(this._gameContext);
        var maxSeededResets = resetConfiguration?.ResetLimit is > 0 ? resetConfiguration.ResetLimit.Value : 0;

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
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString(), BotPasswordWorkFactor);
            account.IsBot = true;
            account.Vault = context.CreateNew<ItemStorage>();

            for (byte slot = 0; slot < perAccount; slot++)
            {
                var characterClass = classQueue.Count > 0 ? classQueue.Dequeue() : creatableClasses.SelectRandom()!;
                var level = minLevel + (int)((maxLevel - minLevel) * Math.Pow(Rand.NextInt(0, 1001) / 1000.0, LevelSkew));
                var seededResets = maxSeededResets > 0 ? Rand.NextInt(0, maxSeededResets + 1) : 0;
                var name = await this._nameGenerator.GenerateUniqueAsync(context, reservedNames, cancellationToken).ConfigureAwait(false);
                this.CreateCharacter(context, account, name, characterClass, level, slot, experienceTable, seededResets, resetConfiguration);
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
            var removed = 0;
            foreach (var bot in bots)
            {
                if (await context.DeleteAsync(bot).ConfigureAwait(false))
                {
                    deleted++;
                    removed++;
                }
                else
                {
                    // Not silent: a bot account which survives the reset is spawned again right after
                    // it, and the paging below would never look at it a second time.
                    this._logger.LogWarning("Bot account '{LoginName}' could not be deleted for the bot reset.", bot.LoginName);
                }
            }

            // Commit this page's deletions before paging on, so the ordering used by the next query
            // reflects the removals: what is left of this page now occupies [skip, skip + kept).
            if (removed > 0)
            {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            skip += page.Count - removed;
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
    /// Spends the character's level-up points, so a high-level bot actually has high-level stats.
    /// Without this a generated level-80 bot would fight with level-1 base stats (tiny health and
    /// damage) and die instantly. The split follows the class build in <see cref="BotProgression"/>
    /// for the server's meta profile (reset vs classic) - the same split the bot keeps using for
    /// points it earns at runtime - and respects each stat's configured maximum (fun servers) as
    /// well as the bot's personal vitality target on reset-meta servers.
    /// </summary>
    private static void DistributeStatPoints(Character character, CharacterClass characterClass, bool resetMeta)
    {
        var points = character.LevelUpPoints;
        if (points <= 0)
        {
            return;
        }

        var weights = BotProgression.GetStatWeights(characterClass, character.Name, resetMeta);
        var vitalityTarget = resetMeta ? BotProgression.GetVitalityTarget(character.Name) : (int?)null;

        long CapacityOf(AttributeDefinition stat)
        {
            var attribute = character.Attributes.FirstOrDefault(a => a.Definition == stat);
            if (attribute is null)
            {
                return 0;
            }

            var classBase = characterClass.StatAttributes.FirstOrDefault(a => a.Attribute == stat);
            var capacity = long.MaxValue;
            if (classBase?.Attribute?.MaximumValue is { } maximumValue)
            {
                capacity = (long)maximumValue - (long)attribute.Value;
            }

            if (vitalityTarget is { } target && stat == Stats.BaseVitality)
            {
                var invested = (long)attribute.Value - (long)(classBase?.BaseValue ?? 0f);
                capacity = Math.Min(capacity, target - invested);
            }

            return capacity;
        }

        foreach (var (stat, amount) in BotProgression.SplitPoints(points, weights, CapacityOf))
        {
            var attribute = character.Attributes.FirstOrDefault(a => a.Definition == stat);
            if (attribute is not null)
            {
                attribute.Value += amount;
                character.LevelUpPoints -= amount;
            }
        }
    }

    /// <summary>
    /// Calculates the level-up points a character with the given reset history would have available,
    /// so a seeded bot invests the same total a player of that history would: the points granted by
    /// the resets themselves (looped through <see cref="ResetProgressionCalculator"/>, so tiers,
    /// multipliers and the replace/add mode of the server's configuration all apply) plus the points
    /// earned by leveling. With <see cref="ResetConfiguration.ResetStats"/> the level points of the
    /// finished cycles were invested and wiped again at each reset, so only the current cycle's count;
    /// without it every cycle's investment survived and still counts.
    /// </summary>
    private static int CalculateLevelUpPoints(CharacterClass characterClass, int level, int seededResets, ResetConfiguration? resetConfiguration)
    {
        var pointsPerLevel = (int)characterClass.StatAttributes.First(a => a.Attribute == Stats.PointsPerLevelUp).BaseValue;
        if (seededResets <= 0 || resetConfiguration is null)
        {
            return (level - 1) * pointsPerLevel;
        }

        var pointsPerResetOverride = (int)(characterClass.StatAttributes.FirstOrDefault(a => a.Attribute == Stats.PointsPerReset)?.BaseValue ?? 0f);
        var resetPoints = 0;
        for (var reset = 0; reset < seededResets; reset++)
        {
            var progression = ResetProgressionCalculator.Calculate(reset, pointsPerResetOverride, resetConfiguration);
            resetPoints = resetConfiguration.ReplacePointsPerReset
                ? progression.TotalPointsAfterReset
                : resetPoints + progression.PointsForReset;
        }

        var currentCyclePoints = Math.Max(0, level - resetConfiguration.LevelAfterReset) * pointsPerLevel;
        if (resetConfiguration.ResetStats)
        {
            return resetPoints + currentCyclePoints;
        }

        var firstCyclePoints = Math.Max(0, resetConfiguration.RequiredLevel - 1) * pointsPerLevel;
        var laterCyclesPoints = (seededResets - 1) * Math.Max(0, resetConfiguration.RequiredLevel - resetConfiguration.LevelAfterReset) * pointsPerLevel;
        return resetPoints + firstCyclePoints + laterCyclesPoints + currentCyclePoints;
    }

    private void CreateCharacter(IPlayerContext context, Account account, string name, CharacterClass characterClass, int level, byte slot, long[] experienceTable, int seededResets, ResetConfiguration? resetConfiguration)
    {
        // A character generated beyond the class evolution level was created as its second-generation
        // class right away - like a player who completed the class quest long ago. Everything downstream
        // (stat weights, skills, gear) keys off the evolved class. A character with a seeded reset
        // history evolved in its first cycle at the latest - provided the reset's required level lies
        // beyond the evolution level (the check below), which makes it pass the evolution on the way
        // to its first reset regardless of its current in-cycle level.
        var passedEvolutionInEarlierCycle = seededResets > 0 && resetConfiguration?.RequiredLevel >= BotProgression.ClassEvolutionLevel;
        if ((level >= BotProgression.ClassEvolutionLevel || passedEvolutionInEarlierCycle)
            && BotProgression.GetEvolutionTarget(characterClass) is { } evolvedClass)
        {
            characterClass = evolvedClass;
        }

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
        if (seededResets > 0
            && character.Attributes.FirstOrDefault(a => a.Definition == Stats.Resets) is { } resetsAttribute)
        {
            // Persisted exactly like a real player's resets (a per-character stat attribute), so the
            // reset counter, the effective level and the reset limit all see the seeded history.
            resetsAttribute.Value = seededResets;
        }

        character.Experience = experienceTable[Math.Min(level, experienceTable.Length - 1)];
        character.LevelUpPoints = CalculateLevelUpPoints(characterClass, level, seededResets, resetConfiguration);
        character.InventoryExtensions = BotInventoryExtensions;
        DistributeStatPoints(character, characterClass, resetConfiguration is not null);

        // Skills survive resets, so a seeded veteran knows everything the highest level of its past
        // cycles unlocked - level-gated skills are checked against that level, not the current one.
        var highestLevelReached = seededResets > 0 && resetConfiguration is not null
            ? Math.Max(level, resetConfiguration.RequiredLevel)
            : level;
        this.LearnClassSkills(context, character, characterClass, highestLevelReached);

        character.Inventory = context.CreateNew<ItemStorage>();
        character.Inventory.Money = StartMoney;
        this.EquipStarterGear(context, character, resetConfiguration is not null);

        account.Characters.Add(character);
    }

    /// <summary>
    /// Teaches the character the class skills appropriate to its level and stats - attack skills as well
    /// as the class's own buffs and heals (e.g. elf Heal/Greater Defense/Greater Damage). Only skills the
    /// class is qualified for are ever learned, gated by the skills' real learn requirements from the game
    /// configuration (total energy, leadership, character level, ...) evaluated against the stats the bot
    /// was just given - exactly the requirements a human player has to meet for the same skill.
    /// </summary>
    private void LearnClassSkills(IPlayerContext context, Character character, CharacterClass characterClass, int level)
    {
        float? GetValue(AttributeDefinition attribute)
        {
            if (BotProgression.TotalToBaseStat(attribute) is not { } baseStat)
            {
                return null;
            }

            return baseStat == Stats.Level
                ? level
                : character.Attributes.FirstOrDefault(a => a.Definition == baseStat)?.Value;
        }

        var learnedNumbers = new HashSet<short>(character.LearnedSkills.Select(s => s.Skill!.Number));
        foreach (var skill in this._gameContext.Configuration.Skills)
        {
            if (!BotProgression.IsBotLearnableSkill(skill)
                || !skill.QualifiedCharacters.Contains(characterClass)
                || !BotProgression.MeetsRequirements(skill, GetValue)
                || !learnedNumbers.Add(skill.Number))
            {
                continue;
            }

            var entry = context.CreateNew<SkillEntry>();
            entry.Skill = skill;
            entry.Level = 0;
            character.LearnedSkills.Add(entry);
        }
    }

    /// <summary>
    /// Equips the bot with a basic, class-appropriate weapon and armor set (mirrors the low-level test
    /// account gear), so it is not naked and punching with its fists. The item level scales modestly
    /// with the bot level for a bit more defense/damage without raising the equip requirements too high.
    /// </summary>
    private void EquipStarterGear(IPlayerContext context, Character character, bool resetMeta)
    {
        var inventory = character.Inventory!;
        var characterClass = character.CharacterClass!;

        // Data-driven so every class gets gear it is actually QUALIFIED to wear (a Dark Lord must never
        // end up in a Pad/wizard set). We pick the most basic options (lowest DropLevel) the class can use:
        // - a weapon from the weapon groups (0 sword, 1 axe, 2 mace, 3 spear, 4 bow, 5 staff),
        // - the armor set whose chest piece (group 8) has the lowest DropLevel; its NUMBER identifies the set,
        //   and the equipment type is the GROUP (7 helm, 8 armor, 9 pants, 10 gloves, 11 boots).
        // The weapon type follows the bot's BUILD (BotProgression.IsPreferredWeaponGroup - the same rule the
        // later upgrades use), so an energy-specced Magic Gladiator starts with a staff instead of a blade.
        // The Small Axe is qualified for almost every class, so without this filter casters and archers would
        // all end up with one.
        bool IsPreferredWeapon(ItemDefinition definition)
            => BotProgression.IsPreferredWeaponGroup(characterClass, character.Name, resetMeta, (byte)definition.Group);

        // Ammunition shares the bow group (Bolt/Arrows have DropLevel 0), so without this filter every
        // archer would get a bolt stack as its "weapon" and end up punching with its fists.
        var weapon = this._gameContext.Configuration.Items
                .Where(d => IsPreferredWeapon(d) && !d.IsAmmunition && d.QualifiedCharacters.Contains(characterClass))
                .MinBy(d => d.DropLevel)
            ?? this._gameContext.Configuration.Items
                .Where(d => d.Group <= StaffGroup && !d.IsAmmunition && d.QualifiedCharacters.Contains(characterClass))
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

        this.AddPotions(context, inventory);
    }

    private void AddPotions(IPlayerContext context, ItemStorage inventory)
    {
        // A stack of Large Healing Potions so the offline HealingHandler has something to drink, and a
        // stack of Large Mana Potions so casters can keep casting instead of degrading to weak melee once
        // their mana runs dry. The BotNavigator tops both up at runtime, so the bot never runs out.
        // Durability holds the stack count.
        this.AddPotionStack(context, inventory, 3, InventoryConstants.EquippableSlotsCount);      // Large Healing Potion, first backpack slot
        this.AddPotionStack(context, inventory, 6, (byte)(InventoryConstants.EquippableSlotsCount + 1)); // Large Mana Potion, second backpack slot
    }

    private void AddPotionStack(IPlayerContext context, ItemStorage inventory, byte potionNumber, byte slot)
    {
        var potion = this._gameContext.Configuration.Items.FirstOrDefault(d => d.Group == 14 && d.Number == potionNumber);
        if (potion is null)
        {
            return;
        }

        var item = context.CreateNew<Item>();
        item.Definition = potion;

        // Only a handful of charges to start with: fresh bots head to the merchant right away and buy
        // their supplies with their starting Zen, kicking off the shopping economy from minute one
        // (kept just above the emergency top-up threshold, so the economy path - not the fallback - runs).
        item.Durability = Rand.NextInt(10, 16);
        item.ItemSlot = slot;
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
