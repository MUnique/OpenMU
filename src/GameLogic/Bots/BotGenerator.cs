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
                var characterClass = creatableClasses.SelectRandom()!;
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

        character.Inventory = context.CreateNew<ItemStorage>();
        character.Inventory.Money = StartMoney;
        this.EquipStarterGear(context, character, level);

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
    private void EquipStarterGear(IPlayerContext context, Character character, int level)
    {
        var inventory = character.Inventory!;
        var itemLevel = (byte)Math.Clamp(level / 20, 0, 4);
        var mainStat = GetMainDamageStat(character)?.Definition;

        // The armor set is identified by the item NUMBER (5 = Leather, 2 = Pad, 10 = Vine), while the
        // item GROUP is the equipment type (7 helm, 8 armor, 9 pants, 10 gloves, 11 boots). Weapons use
        // their own group (1 axe, 4 bow, 5 staff) with number 0.
        byte armorSet;
        if (mainStat == Stats.BaseEnergy)
        {
            // Caster: Skull Staff + Pad set.
            this.AddEquippedItem(context, inventory, InventoryConstants.LeftHandSlot, 5, 0, itemLevel);
            armorSet = 2;
        }
        else if (mainStat == Stats.BaseAgility)
        {
            // Archer: Short Bow + arrows + Vine set.
            this.AddEquippedItem(context, inventory, InventoryConstants.RightHandSlot, 4, 0, itemLevel);
            this.AddEquippedItem(context, inventory, InventoryConstants.LeftHandSlot, 4, 15, 0, 255);
            armorSet = 10;
        }
        else
        {
            // Melee (strength / leadership): Small Axe + Leather set.
            this.AddEquippedItem(context, inventory, InventoryConstants.LeftHandSlot, 1, 0, itemLevel);
            armorSet = 5;
        }

        this.AddEquippedItem(context, inventory, InventoryConstants.HelmSlot, 7, armorSet, itemLevel);
        this.AddEquippedItem(context, inventory, InventoryConstants.ArmorSlot, 8, armorSet, itemLevel);
        this.AddEquippedItem(context, inventory, InventoryConstants.PantsSlot, 9, armorSet, itemLevel);
        this.AddEquippedItem(context, inventory, InventoryConstants.GlovesSlot, 10, armorSet, itemLevel);
        this.AddEquippedItem(context, inventory, InventoryConstants.BootsSlot, 11, armorSet, itemLevel);
    }

    private void AddEquippedItem(IPlayerContext context, ItemStorage inventory, byte slot, int group, int number, byte itemLevel, byte? durability = null)
    {
        var definition = this._gameContext.Configuration.Items.FirstOrDefault(d => d.Group == group && d.Number == number);
        if (definition is null)
        {
            return;
        }

        var item = context.CreateNew<Item>();
        item.Definition = definition;
        item.Level = itemLevel;
        item.Durability = durability ?? definition.Durability;
        item.ItemSlot = slot;
        inventory.Items.Add(item);
    }
}
