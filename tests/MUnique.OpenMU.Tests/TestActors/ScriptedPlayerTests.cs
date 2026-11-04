// <copyright file="ScriptedPlayerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.TestActors;

using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.TestActors;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Tests for the actor itself: its login sequence and its recording views.
/// </summary>
[TestFixture]
public class ScriptedPlayerTests
{
    /// <summary>
    /// The actor logs the character in without a connection and ends up in the world, as a player
    /// of the game context - not as an offline player.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ActorEntersTheWorldAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();

        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);

        Assert.That(actor.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        Assert.That(actor.AccountLoginName, Is.EqualTo("test1"));
        Assert.That(actor.Name, Is.EqualTo("Actor1"));
        Assert.That(actor.CurrentMap, Is.Not.Null);
        Assert.That(actor, Is.Not.InstanceOf<MUnique.OpenMU.GameLogic.Offline.OfflinePlayer>(), "bots must treat an actor like a human player");
        Assert.That((await gameContext.GetPlayersAsync().ConfigureAwait(false)), Contains.Item(actor));
        Assert.That(actor.Intelligence, Is.Not.Null);
    }

    /// <summary>
    /// Entering the world is recorded, so a scenario can wait for it.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task EnteringTheWorldIsRecordedAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();

        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);

        var spawned = actor.EventLog.Since(0).FirstOrDefault(e => e.Type == "spawned");
        Assert.That(spawned, Is.Not.Null);
        Assert.That(spawned!.Fields.First(f => f.Name == "character").Value, Is.EqualTo("Actor1"));
    }

    /// <summary>
    /// The recording container answers the views the actor records, and <c>null</c> for everything
    /// else - the way the offline player's container does.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ViewContainerAnswersRecordedViewsOnlyAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);

        Assert.That(actor.ViewPlugIns.GetPlugIn<IObjectGotKilledPlugIn>(), Is.Not.Null);
        Assert.That(actor.ViewPlugIns.GetPlugIn<MUnique.OpenMU.GameLogic.Views.Character.IUpdateStatsPlugIn>(), Is.Not.Null);
        Assert.That(actor.ViewPlugIns.GetPlugIn<IChatViewPlugIn>(), Is.Not.Null);
        Assert.That(actor.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>(), Is.Not.Null);
        Assert.That(actor.ViewPlugIns.GetPlugIn<IMapChangePlugIn>(), Is.Not.Null);

        // Deliberately not implemented: hits are attributed by the hit recorder plugin instead.
        Assert.That(actor.ViewPlugIns.GetPlugIn<IShowHitPlugIn>(), Is.Null);
    }

    /// <summary>
    /// A kill in the actor's view is recorded with both names.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task KillIsRecordedWithKillerAndVictimAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        await using var victim = await ActorTestHelper.CreateActorAsync(gameContext, "test2", "Actor2").ConfigureAwait(false);
        var lastSeq = actor.EventLog.LastSequence;

        await actor.ViewPlugIns.GetPlugIn<IObjectGotKilledPlugIn>()!.ObjectGotKilledAsync(victim, actor).ConfigureAwait(false);

        var killed = actor.EventLog.Since(lastSeq).Single(e => e.Type == "killed");
        Assert.That(killed.Fields.First(f => f.Name == "victim").Value, Is.EqualTo("Actor2"));
        Assert.That(killed.Fields.First(f => f.Name == "killer").Value, Is.EqualTo("Actor1"));
        Assert.That(killed.Fields.First(f => f.Name == "killer_kind").Value, Is.EqualTo(ActorObjects.ActorKind));
    }

    /// <summary>
    /// Stat changes reach the stream with the attribute's designation and the new value.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task StatChangesAreRecordedAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        var view = actor.ViewPlugIns.GetPlugIn<MUnique.OpenMU.GameLogic.Views.Character.IUpdateStatsPlugIn>()!;
        var lastSeq = actor.EventLog.LastSequence;

        await view.UpdateStatsAsync(MUnique.OpenMU.GameLogic.Attributes.Stats.CurrentHealth, 42).ConfigureAwait(false);
        await view.UpdateStatsAsync(MUnique.OpenMU.GameLogic.Attributes.Stats.CurrentShield, 7).ConfigureAwait(false);
        await view.UpdateStatsAsync(MUnique.OpenMU.GameLogic.Attributes.Stats.CurrentMana, 13).ConfigureAwait(false);
        await view.UpdateStatsAsync(MUnique.OpenMU.GameLogic.Attributes.Stats.CurrentAbility, 3).ConfigureAwait(false);

        // Bookkeeping attributes the engine recalculates constantly are not recorded, or they would
        // wrap the ring within minutes (see design decision 4).
        await view.UpdateStatsAsync(MUnique.OpenMU.GameLogic.Attributes.Stats.ShieldRecoveryMultiplier, 0.002f).ConfigureAwait(false);

        var stats = actor.EventLog.Since(lastSeq).Where(e => e.Type == "stat").ToList();
        Assert.That(stats.Count, Is.EqualTo(4));
        Assert.That(stats[0].Fields.First(f => f.Name == "attribute").Value, Is.EqualTo("Current Health"));
        Assert.That(stats[0].Fields.First(f => f.Name == "value").Value, Is.EqualTo(42f));
        Assert.That(stats.Select(s => s.Fields.First(f => f.Name == "attribute").Value), Is.EqualTo(new object[]
        {
            "Current Health", "Current Shield", "Current Mana", "Current Ability",
        }));
    }
}
