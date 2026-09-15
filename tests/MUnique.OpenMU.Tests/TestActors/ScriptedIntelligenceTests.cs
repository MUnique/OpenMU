// <copyright file="ScriptedIntelligenceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.TestActors;

using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.TestActors;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the command execution: every command is either carried out or refused with a code, and a
/// long command can be interrupted without losing the actor.
/// </summary>
[TestFixture]
public class ScriptedIntelligenceTests
{
    /// <summary>
    /// A skill the character never learned is refused.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task UnknownSkillIsRefusedAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);

        var result = await actor.Intelligence!.ExecuteAsync(new SkillCommand(9999, "Actor2")).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.UnknownSkill));
    }

    /// <summary>
    /// A target which is nowhere near the actor is refused before anything happens.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task AttackOnAnUnknownTargetIsRefusedAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);

        var result = await actor.Intelligence!.ExecuteAsync(new AttackCommand("NotHere", 1, 0)).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.NotInView));
    }

    /// <summary>
    /// A target in view but out of the character's melee range is refused, with the distance in the
    /// message.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task AttackOutOfRangeIsRefusedAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        await using var target = await ActorTestHelper.CreateActorAsync(gameContext, "test2", "Actor2").ConfigureAwait(false);
        await actor.MoveAsync(new Point(100, 100)).ConfigureAwait(false);
        await target.MoveAsync(new Point(105, 100)).ConfigureAwait(false);

        var result = await actor.Intelligence!.ExecuteAsync(new AttackCommand("Actor2", 1, 0)).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.OutOfRange));
        Assert.That(result.Error, Does.Contain("5"));
    }

    /// <summary>
    /// Attacking from within a safe zone is refused - the engine would silently do nothing.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task AttackFromTheSafezoneIsRefusedAsync()
    {
        var safezone = new Point(50, 50);
        var gameContext = ActorTestHelper.CreateGameContext(safezoneTiles: [safezone, new Point(51, 50)]);
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        await using var target = await ActorTestHelper.CreateActorAsync(gameContext, "test2", "Actor2").ConfigureAwait(false);
        await actor.MoveAsync(safezone).ConfigureAwait(false);
        await target.MoveAsync(new Point(51, 50)).ConfigureAwait(false);

        var result = await actor.Intelligence!.ExecuteAsync(new AttackCommand("Actor2", 1, 0)).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.SafeZone));
    }

    /// <summary>
    /// A walk reaches its target and reports the planned path length.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task WalkReachesTheTargetAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        await actor.MoveAsync(new Point(100, 100)).ConfigureAwait(false);

        var result = await actor.Intelligence!.ExecuteAsync(new WalkCommand(104, 104)).ConfigureAwait(false);

        Assert.That(result.Ok, Is.True, result.Error);
        Assert.That(result.Fields.First(f => f.Name == "steps").Value, Is.EqualTo(4));
        Assert.That(actor.Position, Is.EqualTo(new Point(104, 104)));
    }

    /// <summary>
    /// Walking to a tile the path finder cannot reach is refused instead of silently doing nothing.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task WalkWithoutAPathIsRefusedAsync()
    {
        var blocked = new Point(80, 80);
        var gameContext = ActorTestHelper.CreateGameContext(blockedTiles: [blocked]);
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        await actor.MoveAsync(new Point(78, 80)).ConfigureAwait(false);

        var result = await actor.Intelligence!.ExecuteAsync(new WalkCommand(blocked.X, blocked.Y)).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.NoPath));
    }

    /// <summary>
    /// A <c>halt</c> during a repeated attack ends the command with <c>interrupted</c> and the hits
    /// performed so far, records an <c>interrupted</c> event, and leaves the actor in the world.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task HaltInterruptsARepeatedAttackAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        await using var target = await ActorTestHelper.CreateActorAsync(gameContext, "test2", "Actor2").ConfigureAwait(false);
        await actor.MoveAsync(new Point(100, 100)).ConfigureAwait(false);
        await target.MoveAsync(new Point(100, 101)).ConfigureAwait(false);
        var lastSeq = actor.EventLog.LastSequence;

        var attack = actor.Intelligence!.ExecuteAsync(new AttackCommand("Actor2", 20, 1000));
        await Task.Delay(300).ConfigureAwait(false);
        actor.Intelligence.Halt();
        var result = await attack.ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.Interrupted));
        var hits = (IReadOnlyList<object?>)result.Fields.First(f => f.Name == "hits").Value!;
        Assert.That(hits.Count, Is.InRange(1, 3));
        Assert.That(actor.EventLog.Since(lastSeq).Any(e => e.Type == "interrupted"), Is.True);
        Assert.That(actor.PlayerState.CurrentState, Is.EqualTo(MUnique.OpenMU.GameLogic.PlayerState.EnteredWorld));
        Assert.That((await gameContext.GetPlayersAsync().ConfigureAwait(false)), Contains.Item(actor));
    }

    /// <summary>
    /// A <c>halt</c> during a multi-step walk stops the actor where it is, answers the caller with
    /// <c>interrupted</c> and the progress, and keeps the actor in the world.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task HaltInterruptsAWalkAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        await actor.MoveAsync(new Point(100, 100)).ConfigureAwait(false);
        var lastSeq = actor.EventLog.LastSequence;

        // 12 tiles is what one path finder request covers, like a client's click.
        var walk = actor.Intelligence!.ExecuteAsync(new WalkCommand(112, 112));
        await Task.Delay(300).ConfigureAwait(false);
        actor.Intelligence.Halt();
        var result = await walk.ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.Interrupted));
        Assert.That(result.Fields.First(f => f.Name == "steps").Value, Is.GreaterThan(0));
        Assert.That(actor.IsWalking, Is.False, "the halt stopped the walk");
        Assert.That(actor.EventLog.Since(lastSeq).Any(e => e.Type == "interrupted"), Is.True);
        Assert.That((await gameContext.GetPlayersAsync().ConfigureAwait(false)), Contains.Item(actor));
    }

    /// <summary>
    /// A second command interrupts the one in flight the same way a <c>halt</c> does, and then runs.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ASecondCommandInterruptsTheFirstAsync()
    {
        var gameContext = ActorTestHelper.CreateGameContext();
        await using var actor = await ActorTestHelper.CreateActorAsync(gameContext, "test1", "Actor1").ConfigureAwait(false);
        await using var target = await ActorTestHelper.CreateActorAsync(gameContext, "test2", "Actor2").ConfigureAwait(false);
        await actor.MoveAsync(new Point(100, 100)).ConfigureAwait(false);
        await target.MoveAsync(new Point(100, 101)).ConfigureAwait(false);

        var firstAttack = actor.Intelligence!.ExecuteAsync(new AttackCommand("Actor2", 20, 1000));
        await Task.Delay(300).ConfigureAwait(false);
        var second = actor.Intelligence.ExecuteAsync(new SayCommand("interrupting"));

        var firstResult = await firstAttack.ConfigureAwait(false);
        var secondResult = await second.ConfigureAwait(false);

        Assert.That(firstResult.Code, Is.EqualTo(ActorErrorCodes.Interrupted));
        Assert.That(secondResult.Ok, Is.True, secondResult.Error);
    }
}
