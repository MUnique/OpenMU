// <copyright file="StateMachineTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.GameLogic;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the state machine.
    /// </summary>
    [TestFixture]
    public class StateMachineTest
    {
        private StateMachine stateMachine;

        private State initialState;

        private State isolatedState;

        private State nextState;

        private State finishedState;

        /// <summary>
        /// Sets up the test data.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.initialState = new State(new Guid("ADBEC1FA-7DB8-4A80-B054-2297B20AF32B"))
            {
                Name = "Initial State",
                PossibleTransitions = new List<State>(),
            };
            this.nextState = new State(new Guid("9954D837-D5FC-4204-AD96-6BD9F19353EA"))
            {
                Name = "Next State",
                PossibleTransitions = new List<State>(),
            };
            this.initialState.PossibleTransitions.Add(this.nextState);
            this.nextState.PossibleTransitions.Add(this.initialState);
            this.finishedState = new State(new Guid("F3658D9E-581B-451A-9C35-92A6B13B8C64"))
            {
                Name = "Finished",
            };
            this.nextState.PossibleTransitions.Add(this.finishedState);

            this.isolatedState = new State(new Guid("4D45D4B0-1CA5-4222-91CC-B05DC5D87D56"))
            {
                Name = "Isolated State",
            };

            this.stateMachine = new StateMachine(this.initialState);
        }

        /// <summary>
        /// Tests if the transition to the next allowed state is successful.
        /// </summary>
        [Test]
        public void TransitionToNextState()
        {
            var success = this.stateMachine.TryAdvanceTo(this.nextState);
            Assert.That(success, Is.True);
            Assert.That(this.stateMachine.CurrentState, Is.EqualTo(this.nextState));
            Assert.That(this.stateMachine.Finished, Is.False);
        }

        /// <summary>
        /// Tests if the transition to an isolated state fails.
        /// </summary>
        [Test]
        public void TransitionToIsolatedState()
        {
            var success = this.stateMachine.TryAdvanceTo(this.isolatedState);
            Assert.That(success, Is.False);
            Assert.That(this.stateMachine.CurrentState, Is.EqualTo(this.initialState));
        }

        /// <summary>
        /// Tests if the transition to the finished state succeeds and if the state machine takes notice of it.
        /// </summary>
        [Test]
        public void TransitionToFinishedState()
        {
            this.stateMachine.TryAdvanceTo(this.nextState);
            var success = this.stateMachine.TryAdvanceTo(this.finishedState);
            Assert.That(success, Is.True);
            Assert.That(this.stateMachine.CurrentState, Is.EqualTo(this.finishedState));
            Assert.That(this.stateMachine.Finished, Is.True);
        }

        /// <summary>
        /// Tests if the state change event does get raised with the next state in the event arguments.
        /// </summary>
        [Test]
        public void ChangesEventStateObject()
        {
            State stateInEvent = null;
            this.stateMachine.StateChanges += (sender, args) =>
                {
                    stateInEvent = args.NextState;
                };
            this.stateMachine.TryAdvanceTo(this.nextState);
            Assert.That(stateInEvent, Is.EqualTo(this.nextState));
        }

        /// <summary>
        /// Tests the cancellation of state changes.
        /// </summary>
        [Test]
        public void ChangesEventCancels()
        {
            this.stateMachine.StateChanges += (sender, args) =>
                {
                    args.Cancel = true;
                };
            var success = this.stateMachine.TryAdvanceTo(this.nextState);
            Assert.That(success, Is.False);
            Assert.That(this.stateMachine.CurrentState, Is.EqualTo(this.initialState));
        }

        /// <summary>
        /// Tests if the state change event does get raised.
        /// </summary>
        [Test]
        public void ChangedEvent()
        {
            var stateChangeEventCalled = false;
            this.stateMachine.StateChanged += (sender, args) => stateChangeEventCalled = true;
            this.stateMachine.TryAdvanceTo(this.nextState);
            Assert.That(stateChangeEventCalled, Is.True);
        }
    }
}
