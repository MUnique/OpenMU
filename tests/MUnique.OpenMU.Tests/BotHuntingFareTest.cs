// <copyright file="BotHuntingFareTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using NUnit.Framework;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;
using MonsterSpawnArea = MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea;

/// <summary>
/// Tests for the Zen a bot keeps for the warp to its hunting ground, so a shopping trip can't strand it in town.
/// </summary>
[TestFixture]
public class BotHuntingFareTest
{
    /// <summary>
    /// The level of the bot. Its monsters stay well below, so every one of them is safe.
    /// </summary>
    private const int BotLevel = 100;

    /// <summary>
    /// The first number of the test monsters. <c>CombatHandler</c> caches the combat stats of a monster
    /// definition statically by its number, so the numbers must not collide with other tests.
    /// </summary>
    private const short FirstMonsterNumber = 30101;

    /// <summary>
    /// The bot keeps the fare to the map with the strongest monsters it can safely fight, not to the
    /// first or the cheapest one.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask FareOfTheStrongestGroundIsKeptAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        AddHuntingMap(gameContext, 1, monsterLevel: 20, fare: 3000, levelRequirement: 10, FirstMonsterNumber);
        AddHuntingMap(gameContext, 2, monsterLevel: 40, fare: 8000, levelRequirement: 50, FirstMonsterNumber + 1);
        var navigator = await CreateNavigatorAsync(gameContext).ConfigureAwait(false);

        Assert.That(navigator.GetHuntingFare(), Is.EqualTo(8000));
    }

    /// <summary>
    /// A map the bot may not warp to yet doesn't count, however strong its monsters are: the bot
    /// will never pay that fare.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask MapAboveTheWarpLevelIsIgnoredAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        AddHuntingMap(gameContext, 1, monsterLevel: 20, fare: 3000, levelRequirement: 10, FirstMonsterNumber + 2);
        AddHuntingMap(gameContext, 2, monsterLevel: 40, fare: 8000, levelRequirement: 400, FirstMonsterNumber + 3);
        var navigator = await CreateNavigatorAsync(gameContext).ConfigureAwait(false);

        Assert.That(navigator.GetHuntingFare(), Is.EqualTo(3000));
    }

    /// <summary>
    /// A bot without any legal warp (a fresh character below every warp level) keeps nothing back:
    /// its potion purchases stay exactly as before.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask NoLegalWarpKeepsNothingAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        AddHuntingMap(gameContext, 1, monsterLevel: 20, fare: 3000, levelRequirement: 400, FirstMonsterNumber + 4);
        var navigator = await CreateNavigatorAsync(gameContext).ConfigureAwait(false);

        Assert.That(navigator.GetHuntingFare(), Is.EqualTo(0));
    }

    /// <summary>
    /// A potion trip needs Zen beyond the fare, otherwise the bot walks to the merchant and may not buy
    /// anything - the loop the potion trigger was made to prevent.
    /// </summary>
    [Test]
    public void PotionMoneyMustExceedTheFare()
    {
        Assert.That(BotShoppingHandler.HasPotionMoney(5000, 0), Is.True, "without a fare, the usual minimum applies");
        Assert.That(BotShoppingHandler.HasPotionMoney(5000, 8000), Is.False, "the fare can't be spent on potions");
        Assert.That(BotShoppingHandler.HasPotionMoney(9000, 8000), Is.True, "Zen beyond the fare can");
    }

    private static async ValueTask<BotNavigator> CreateNavigatorAsync(IGameContext gameContext)
    {
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        bot.Attributes![Stats.Level] = BotLevel;

        // The bot only counts monsters it can safely kill (CombatHandler.IsSafeTarget), which requires
        // some attack power. The damage attributes are derived and can't be assigned, so the power
        // comes from a learned skill - the same way the hunting ground tests do it.
        await bot.SkillList!.AddLearnedSkillAsync(new TestSkill { AttackDamage = 100 }).ConfigureAwait(false);
        return new BotNavigator(bot);
    }

    /// <summary>
    /// Adds a fully walkable map with one spawn gate, one warp entry and one kind of weak monster.
    /// </summary>
    private static void AddHuntingMap(IGameContext gameContext, byte number, int monsterLevel, int fare, int levelRequirement, short monsterNumber)
    {
        var map = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            TerrainData = new byte[ushort.MaxValue + 3],
        };

        var gate = new MUnique.OpenMU.Persistence.BasicModel.ExitGate
        {
            Id = Guid.NewGuid(),
            Map = map,
            X1 = 10,
            Y1 = 10,
            X2 = 11,
            Y2 = 11,
            IsSpawnGate = true,
        };
        map.ExitGates.Add(gate);
        map.MonsterSpawns.Add(new MonsterSpawnArea
        {
            MonsterDefinition = CreateMonsterDefinition(monsterNumber, monsterLevel),
            GameMap = map,
            X1 = 20,
            X2 = 20,
            Y1 = 20,
            Y2 = 20,
            Quantity = 1,
            SpawnTrigger = SpawnTrigger.Automatic,
        });

        gameContext.Configuration.Maps.Add(map);
        gameContext.Configuration.WarpList.Add(new MUnique.OpenMU.Persistence.BasicModel.WarpInfo
        {
            Id = Guid.NewGuid(),
            Name = $"Map{number}",
            Gate = gate,
            LevelRequirement = levelRequirement,
            Costs = fare,
        });
    }

    private static MonsterDefinition CreateMonsterDefinition(short number, int level)
    {
        var definition = new MonsterDefinition
        {
            Id = Guid.NewGuid(),
            Number = number,
            ObjectKind = NpcObjectKind.Monster,
        };
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.Level, Value = level });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MinimumPhysBaseDmg, Value = 0 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumPhysBaseDmg, Value = 0 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.DefenseBase, Value = 0 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 1 });
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.AttackRatePvm, Value = 0 });
        return definition;
    }

    private sealed class TestSkill : Skill
    {
        public TestSkill()
        {
            this.Requirements = new List<AttributeRequirement>();
            this.ConsumeRequirements = new List<AttributeRequirement>();
            this.Range = 1;
            this.SkillType = SkillType.DirectHit;
            this.DamageType = DamageType.Curse;
        }
    }
}
