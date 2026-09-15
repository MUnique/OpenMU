// <copyright file="ActorHitRecorderPlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.TestActors;

using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.TestActors;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Tests;
using Moq;

/// <summary>
/// Tests the hit attribution: exactly one record per hit and involved actor, never a duplicate.
/// </summary>
[TestFixture]
public class ActorHitRecorderPlugInTests
{
    private static readonly HitInfo Hit = new(57, 3, DamageAttributes.Undefined);

    /// <summary>
    /// A hit taken from another player is recorded once, on the victim's log, naming the attacker.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ReceivedHitNamesItsAttackerAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var victim = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        attacker.SelectedCharacter!.Name = "Human";
        var lastSeq = victim.EventLog.LastSequence;

        new ActorHitRecorderPlugIn().AttackableGotHit(victim, attacker, Hit);

        var hits = victim.EventLog.Since(lastSeq).Where(e => e.Type == "hit").ToList();
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0].Fields.First(f => f.Name == "direction").Value, Is.EqualTo("received"));
        Assert.That(hits[0].Fields.First(f => f.Name == "attacker").Value, Is.EqualTo("Human"));
        Assert.That(hits[0].Fields.First(f => f.Name == "attacker_kind").Value, Is.EqualTo(ActorObjects.PlayerKind));
        Assert.That(hits[0].Fields.First(f => f.Name == "health_damage").Value, Is.EqualTo(57u));
    }

    /// <summary>
    /// A hit dealt to a non-actor is recorded once, on the attacking actor's log.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task DealtHitNamesItsTargetAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        var target = new Mock<IAttackable>();
        target.Setup(t => t.Id).Returns(123);
        var lastSeq = actor.EventLog.LastSequence;

        new ActorHitRecorderPlugIn().AttackableGotHit(target.Object, actor, Hit);

        var hits = actor.EventLog.Since(lastSeq).Where(e => e.Type == "hit").ToList();
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0].Fields.First(f => f.Name == "direction").Value, Is.EqualTo("dealt"));
        Assert.That(hits[0].Fields.First(f => f.Name == "target_id").Value, Is.EqualTo((ushort)123));
    }

    /// <summary>
    /// Actor versus actor: one record on each side, and never two on either.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ActorVersusActorRecordsOneHitOnEachSideAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var attacker = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        await using var victim = await ActorTestHelper.CreateActorAsync(gameContext, "test2", "Actor2").ConfigureAwait(false);
        var attackerSeq = attacker.EventLog.LastSequence;
        var victimSeq = victim.EventLog.LastSequence;

        new ActorHitRecorderPlugIn().AttackableGotHit(victim, attacker, Hit);

        var dealt = attacker.EventLog.Since(attackerSeq).Where(e => e.Type == "hit").ToList();
        var received = victim.EventLog.Since(victimSeq).Where(e => e.Type == "hit").ToList();
        Assert.That(dealt.Count, Is.EqualTo(1));
        Assert.That(received.Count, Is.EqualTo(1));
        Assert.That(dealt[0].Fields.First(f => f.Name == "direction").Value, Is.EqualTo("dealt"));
        Assert.That(dealt[0].Fields.First(f => f.Name == "target").Value, Is.EqualTo("Actor2"));
        Assert.That(dealt[0].Fields.First(f => f.Name == "target_kind").Value, Is.EqualTo(ActorObjects.ActorKind));
        Assert.That(received[0].Fields.First(f => f.Name == "direction").Value, Is.EqualTo("received"));
        Assert.That(received[0].Fields.First(f => f.Name == "attacker").Value, Is.EqualTo("Actor1"));
        Assert.That(received[0].Fields.First(f => f.Name == "attacker_kind").Value, Is.EqualTo(ActorObjects.ActorKind));
    }

    /// <summary>
    /// A hit which involves no actor at all records nothing.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task HitBetweenNonActorsRecordsNothingAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var bystander = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var victim = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var lastSeq = bystander.EventLog.LastSequence;

        new ActorHitRecorderPlugIn().AttackableGotHit(victim, attacker, Hit);

        Assert.That(bystander.EventLog.Since(lastSeq).Where(e => e.Type == "hit"), Is.Empty);
    }

    /// <summary>
    /// A bot is attributed as such, so a scenario can tell self-defence from a human's attack.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task BotAttackerIsAttributedAsBotAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var victim = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        bot.Account!.IsBot = true;
        bot.SelectedCharacter!.Name = "BotName";
        var lastSeq = victim.EventLog.LastSequence;

        new ActorHitRecorderPlugIn().AttackableGotHit(victim, bot, Hit);

        var hit = victim.EventLog.Since(lastSeq).Single(e => e.Type == "hit");
        Assert.That(hit.Fields.First(f => f.Name == "attacker_kind").Value, Is.EqualTo(ActorObjects.BotKind));
        Assert.That(hit.Fields.First(f => f.Name == "attacker").Value, Is.EqualTo("BotName"));
    }
}
