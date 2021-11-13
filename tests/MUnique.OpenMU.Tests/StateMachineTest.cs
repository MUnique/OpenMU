// <copyright file="StateMachineTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;

/// <summary>
/// Tests for the state machine.
/// </summary>
[TestFixture]
public class StateMachineTest
{
    private StateMachine _stateMachine = null!;

    private State _initialState = null!;

    private State _isolatedState = null!;

    private State _nextState = null!;

    private State _finishedState = null!;

    /// <summary>
    /// Sets up the test data.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._initialState = new State(new Guid("ADBEC1FA-7DB8-4A80-B054-2297B20AF32B"))
        {
            Name = "Initial State",
            PossibleTransitions = new List<State>(),
        };
        this._nextState = new State(new Guid("9954D837-D5FC-4204-AD96-6BD9F19353EA"))
        {
            Name = "Next State",
            PossibleTransitions = new List<State>(),
        };
        this._initialState.PossibleTransitions.Add(this._nextState);
        this._nextState.PossibleTransitions.Add(this._initialState);
        this._finishedState = new State(new Guid("F3658D9E-581B-451A-9C35-92A6B13B8C64"))
        {
            Name = "Finished",
        };
        this._nextState.PossibleTransitions.Add(this._finishedState);

        this._isolatedState = new State(new Guid("4D45D4B0-1CA5-4222-91CC-B05DC5D87D56"))
        {
            Name = "Isolated State",
        };

        this._stateMachine = new StateMachine(this._initialState);
    }

    /// <summary>
    /// Tests if the transition to the next allowed state is successful.
    /// </summary>
    [Test]
    public void TransitionToNextState()
    {
        var success = this._stateMachine.TryAdvanceTo(this._nextState);
        Assert.That(success, Is.True);
        Assert.That(this._stateMachine.CurrentState, Is.EqualTo(this._nextState));
        Assert.That(this._stateMachine.Finished, Is.False);
    }

    /// <summary>
    /// Tests if the transition to an isolated state fails.
    /// </summary>
    [Test]
    public void TransitionToIsolatedState()
    {
        var success = this._stateMachine.TryAdvanceTo(this._isolatedState);
        Assert.That(success, Is.False);
        Assert.That(this._stateMachine.CurrentState, Is.EqualTo(this._initialState));
    }

    /// <summary>
    /// Tests if the transition to the finished state succeeds and if the state machine takes notice of it.
    /// </summary>
    [Test]
    public void TransitionToFinishedState()
    {
        this._stateMachine.TryAdvanceTo(this._nextState);
        var success = this._stateMachine.TryAdvanceTo(this._finishedState);
        Assert.That(success, Is.True);
        Assert.That(this._stateMachine.CurrentState, Is.EqualTo(this._finishedState));
        Assert.That(this._stateMachine.Finished, Is.True);
    }

    /// <summary>
    /// Tests if the state change event does get raised with the next state in the event arguments.
    /// </summary>
    [Test]
    public void ChangesEventStateObject()
    {
        State? stateInEvent = null;
        this._stateMachine.StateChanges += (_, args) =>
        {
            stateInEvent = args.NextState;
        };
        this._stateMachine.TryAdvanceTo(this._nextState);
        Assert.That(stateInEvent, Is.EqualTo(this._nextState));
    }

    /// <summary>
    /// Tests the cancellation of state changes.
    /// </summary>
    [Test]
    public void ChangesEventCancels()
    {
        this._stateMachine.StateChanges += (_, args) =>
        {
            args.Cancel = true;
        };
        var success = this._stateMachine.TryAdvanceTo(this._nextState);
        Assert.That(success, Is.False);
        Assert.That(this._stateMachine.CurrentState, Is.EqualTo(this._initialState));
    }

    /// <summary>
    /// Tests if the state change event does get raised.
    /// </summary>
    [Test]
    public void ChangedEvent()
    {
        var stateChangeEventCalled = false;
        this._stateMachine.StateChanged += (_, _) => stateChangeEventCalled = true;
        this._stateMachine.TryAdvanceTo(this._nextState);
        Assert.That(stateChangeEventCalled, Is.True);
    }
}