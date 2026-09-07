// <copyright file="BotGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
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

    private const int StartMoney = 100000;

    /// <summary>Number of inventory extensions (each 4 rows of 8 slots) a bot gets, so loot does not clog its backpack.</summary>
    private const int BotInventoryExtensions = 4;

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
    /// The outcome of deleting a single bot account.
    /// </summary>
    private enum BotAccountDeleteOutcome
    {
        /// <summary>
        /// The account was deleted.
        /// </summary>
        Deleted,

        /// <summary>
        /// The account was already gone; nothing had to be deleted.
        /// </summary>
        NotFound,

        /// <summary>
        /// The account could not be deleted.
        /// </summary>
        Failed,
    }

    /// <summary>
    /// Gets the deterministic, internal login name of the bot account with the given one-based index.
    /// </summary>
    /// <param name="index">The one-based account index.</param>
    /// <returns>The login name, e.g. <c>bot0001</c> (kept within the 10-character account name limit).</returns>
    public static string GetLoginName(int index) => $"{LoginPrefix}{index:D4}";

    /// <summary>
    /// Ensures that the configured number of bot accounts (each with the configured number of
    /// characters) exists. Only missing accounts are created.
    /// </summary>
    /// <param name="numberOfAccounts">The desired number of bot accounts.</param>
    /// <param name="charactersPerAccount">The desired number of characters per account.</param>
    /// <param name="profile">The startup profile of the generated characters (fresh or veteran).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of accounts that were newly created.</returns>
    public async ValueTask<int> EnsureBotsAsync(int numberOfAccounts, int charactersPerAccount, BotStartupProfile profile, CancellationToken cancellationToken = default)
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
        var maxLevel = Math.Min(profile.MaxLevel, experienceTable.Length - 1);
        var minLevel = Math.Clamp(profile.MinLevel, 1, maxLevel);

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
                var level = profile.GetStartLevel(minLevel, maxLevel);
                var seededResets = profile.GetSeededResets(maxSeededResets);
                var name = await this._nameGenerator.GenerateUniqueAsync(context, reservedNames, cancellationToken).ConfigureAwait(false);
                this.CreateCharacter(context, account, name, characterClass, level, slot, experienceTable, seededResets, profile.StarterItemLevel, profile.EquipStarterArmor, resetConfiguration);
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
    /// Deletes all bot accounts with their characters, item storages and items.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of deleted bot accounts.</returns>
    /// <exception cref="InvalidOperationException">At least one bot account could not be deleted.</exception>
    public async ValueTask<int> DeleteAllBotsAsync(CancellationToken cancellationToken = default)
    {
        using var context = this._gameContext.PersistenceContextProvider.CreateNewPlayerContext(this._gameContext.Configuration);
        var loginNames = await this.CollectBotLoginNamesAsync(context, cancellationToken).ConfigureAwait(false);

        var deleted = 0;
        var failed = 0;
        foreach (var loginName in loginNames)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var outcome = await this.TryDeleteBotAccountAsync(loginName, context, cancellationToken).ConfigureAwait(false);
            if (outcome == BotAccountDeleteOutcome.Deleted)
            {
                deleted++;
            }
            else if (outcome == BotAccountDeleteOutcome.Failed)
            {
                failed++;
            }
            else
            {
                // NotFound: the account was already gone, nothing to count.
            }
        }

        if (failed > 0)
        {
            throw new InvalidOperationException($"{failed} of {loginNames.Count} bot account(s) could not be deleted.");
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
    /// - the same split the bot keeps using for points it earns at runtime - and respects each stat's
    /// configured maximum (fun servers) as well as the bot's personal vitality target on reset-meta servers.
    /// </summary>
    private static void DistributeStatPoints(Character character, CharacterClass characterClass, bool resetMeta)
    {
        var points = character.LevelUpPoints;
        if (points <= 0)
        {
            return;
        }

        var weights = BotProgression.GetStatWeights(characterClass, character.Name);
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

    /// <summary>
    /// Collects the login names of all bot accounts, ordered by login name.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The login names of the bot accounts.</returns>
    /// <remarks>
    /// Collected first, deleted afterwards: the paging query orders by login name, so deleting while
    /// paging would shift the accounts which are not visited yet into the pages already passed.
    /// </remarks>
    private async ValueTask<List<string>> CollectBotLoginNamesAsync(IPlayerContext context, CancellationToken cancellationToken)
    {
        var loginNames = new List<string>();
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

            loginNames.AddRange(page.Where(account => account.IsBot).Select(account => account.LoginName));
            skip += page.Count;
        }

        return loginNames;
    }

    /// <summary>
    /// Tries to delete a single bot account with its characters, item storages, and items.
    /// </summary>
    /// <param name="loginName">The login name of the bot account.</param>
    /// <param name="context">The persistence context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outcome of the deletion.</returns>
    /// <remarks>
    /// One bad account returns <see cref="BotAccountDeleteOutcome.Failed"/> instead of aborting the
    /// whole purge; the caller fails the purge as a whole, so the flags stay set and it gets retried
    /// instead of switching the feature off with the bot accounts still in the database.
    /// Cancellation is never swallowed.
    /// </remarks>
    private async ValueTask<BotAccountDeleteOutcome> TryDeleteBotAccountAsync(string loginName, IPlayerContext context, CancellationToken cancellationToken)
    {
        Account? account = null;
        try
        {
            // Load the account again, this time with its whole graph: the paging query returns the
            // accounts untracked and without their characters, and deleting such a shallow account
            // leaves its item storages behind. A character's inventory is referenced by the character,
            // so no delete cascade ever reaches it - those storages, and every item lying in them, would
            // stay in the database forever as unreachable rows.
            account = await context.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false);
            if (account is null)
            {
                return BotAccountDeleteOutcome.NotFound;
            }

            foreach (var character in account.Characters)
            {
                if (character.Inventory is { } inventory)
                {
                    await context.DeleteAsync(inventory).ConfigureAwait(false);
                }
            }

            if (account.Vault is { } vault)
            {
                await context.DeleteAsync(vault).ConfigureAwait(false);
            }

            var deleteQueued = await context.DeleteAsync(account).ConfigureAwait(false);

            // Save per account, so a single failure does not roll back the accounts already deleted.
            // Only counted after the save went through: an account whose save throws must not count as deleted.
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            if (!deleteQueued)
            {
                // Not silent: a bot account which survives the purge is spawned again right after it.
                this._logger.LogWarning("Bot account '{LoginName}' could not be deleted.", loginName);
                return BotAccountDeleteOutcome.Failed;
            }

            return BotAccountDeleteOutcome.Deleted;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            this._logger.LogError(ex, "Failed to delete bot account '{LoginName}', skipping it.", loginName);

            if (account is not null)
            {
                // A failed save leaves the account graph in the change tracker as Deleted, which would
                // fail every following save as well - detach it so the next account starts clean.
                context.Detach(account);
            }

            return BotAccountDeleteOutcome.Failed;
        }
    }

    private void CreateCharacter(IPlayerContext context, Account account, string name, CharacterClass characterClass, int level, byte slot, long[] experienceTable, int seededResets, byte starterItemLevel, bool equipStarterArmor, ResetConfiguration? resetConfiguration)
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

        // Distinct, because a character class may define the same stat attribute more than once (data
        // which got duplicated by an update); a character must never hold an attribute twice.
        foreach (var attribute in characterClass.StatAttributes
                     .DistinctBy(a => a.Attribute)
                     .Select(a => context.CreateNew<StatAttribute>(a.Attribute, a.BaseValue)))
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
        // cycles unlocked; level-gated skills are checked against that level, not the current one.
        var highestLevelReached = seededResets > 0 && resetConfiguration is not null
            ? Math.Max(level, resetConfiguration.RequiredLevel)
            : level;
        this.LearnClassSkills(context, character, characterClass, highestLevelReached);

        character.Inventory = context.CreateNew<ItemStorage>();
        character.Inventory.Money = StartMoney;

        // A fresh character starts like a regular player's new character - weapon only, no armor -
        // and loots its first set like everyone else. Veterans keep the basic set, without which
        // they could not survive the maps their start level puts them on.
        var starterGear = new BotStarterGearEquipper(context, this._gameContext.Configuration, character, starterItemLevel);
        starterGear.EquipWeapon();
        if (equipStarterArmor)
        {
            starterGear.EquipArmorSet();
        }

        starterGear.AddPotions();

        account.Characters.Add(character);
    }

    /// <summary>
    /// Teaches the character the class skills appropriate to its level and stats - attack skills as well
    /// as the class's own buffs and heals (e.g. elf Heal/Greater Defense/Greater Damage). Only skills the
    /// class is qualified for are ever learned, gated by the skills' real learn requirements from the game
    /// configuration (total energy, leadership, character level, ...) evaluated against the stats the bot
    /// was just given - exactly the requirements a human player has to meet for the same skill. Item-bound
    /// skills follow the backfill rules (see <see cref="BotProgression.MayBackfillSkill"/>): orb/scroll
    /// skills only when their granting item is obtainable, so a bot cannot learn a scroll before the
    /// monster level where it starts to drop - and never when the skill comes from worn equipment or a
    /// pet, which the server grants temporarily on equip instead.
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
        var grantingItems = BotProgression.GetGrantingItems(this._gameContext.Configuration);
        foreach (var skill in this._gameContext.Configuration.Skills)
        {
            if (!BotProgression.MayBotOwnSkill(skill)
                || !skill.QualifiedCharacters.Contains(characterClass)
                || !BotProgression.MeetsRequirements(skill, GetValue)
                || !BotProgression.MayBackfillSkill(skill, grantingItems, characterClass, level, GetValue)
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
}
