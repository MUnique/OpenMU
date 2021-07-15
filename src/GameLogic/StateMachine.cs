// <copyright file="StateMachine.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.ComponentModel;
    using System.Threading;

    /// <summary>
    /// A state machine.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// The lock object for state transitions.
        /// </summary>
        private readonly object lockObject = new ();

        /// <summary>
        /// A cancel event args object, which is getting reused.
        /// </summary>
        private readonly StateChangeEventArgs cachedCancelEventArgs = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachine"/> class.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        public StateMachine(State initialState)
        {
            this.CurrentState = initialState;
        }

        /// <summary>
        /// The state change cancel event handler.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void StateChangeCancelEventHandler(object? sender, StateChangeEventArgs e);

        /// <summary>
        /// Event that fires just before the state changes.
        /// </summary>
        public event StateChangeCancelEventHandler? StateChanges;

        /// <summary>
        /// Event that fires after the state have changed.
        /// </summary>
        public event EventHandler? StateChanged;

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public State? CurrentState { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the state machine is in a finished state, that means that no further state changes are possible.
        /// </summary>
        public bool Finished => this.CurrentState?.PossibleTransitions is null || this.CurrentState.PossibleTransitions.Count == 0;

        /// <summary>
        /// Tries to advance the state to <paramref name="nextState"/>.
        /// </summary>
        /// <param name="nextState">The state to advance to.</param>
        /// <returns>The success.</returns>
        public bool TryAdvanceTo(State nextState)
        {
            if (this.CurrentState?.PossibleTransitions is null)
            {
                return false;
            }

            lock (this.lockObject)
            {
                if (this.CurrentState.PossibleTransitions.Contains(nextState) && this.OnStateChanging(nextState))
                {
                    this.CurrentState = nextState;
                    this.OnStateChanged();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to start a "transaction" to advance the state to <paramref name="nextState"/>.
        /// </summary>
        /// <param name="nextState">The state to advance to.</param>
        /// <returns>The state change context. On disposal of this object, the state change is getting completed.</returns>
        public StateChangeContext TryBeginAdvanceTo(State nextState)
        {
            var context = new StateChangeContext(this.lockObject, () =>
            {
                this.CurrentState = nextState;
                this.OnStateChanged();
            })
            {
                Allowed = (this.CurrentState?.PossibleTransitions?.Contains(nextState) ?? false) && this.OnStateChanging(nextState),
            };

            return context;
        }

        /// <summary>
        /// Calls the StateChanged-Event.
        /// </summary>
        private void OnStateChanged()
        {
            this.StateChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Calls the StateChanges-Event.
        /// </summary>
        /// <param name="nextState">The next state.</param>
        /// <returns><c>True</c>, if all event handlers did not set <see cref="CancelEventArgs"/> to <c>true</c>; Otherwise, <c>false</c>.</returns>
        private bool OnStateChanging(State nextState)
        {
            if (this.StateChanges != null)
            {
                this.cachedCancelEventArgs.Cancel = false;
                this.cachedCancelEventArgs.NextState = nextState;
                this.StateChanges(this, this.cachedCancelEventArgs);
                return !this.cachedCancelEventArgs.Cancel;
            }

            return true;
        }

        /// <summary>
        /// The state change context for more complex state changes.
        /// On disposal of this object, the state change is getting completed.
        /// </summary>
        public sealed class StateChangeContext : IDisposable
        {
            /// <summary>
            /// The lock object of the state machine.
            /// </summary>
            private readonly object lockObject;

            /// <summary>
            /// The action which gets executed when the state change is completed.
            /// </summary>
            private readonly Action finishAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="StateChangeContext"/> class.
            /// </summary>
            /// <param name="lockObject">The lock object of the state machine.</param>
            /// <param name="finishAction">The action which should get executed when the state change is completed.</param>
            public StateChangeContext(object lockObject, Action finishAction)
            {
                Monitor.Enter(lockObject);
                this.lockObject = lockObject;
                this.finishAction = finishAction;
            }

            /// <summary>
            /// Gets a value indicating whether a state change is allowed.
            /// </summary>
            public bool Allowed { get; internal set; }

            /// <inheritdoc/>
            public void Dispose()
            {
                try
                {
                    if (this.Allowed)
                    {
                        this.finishAction();
                    }
                }
                finally
                {
                    Monitor.Exit(this.lockObject);
                }
            }
        }

        /// <summary>
        /// The state change event args, including the next state.
        /// </summary>
        public class StateChangeEventArgs : CancelEventArgs
        {
            /// <summary>
            /// Gets or sets the next state.
            /// </summary>
            public State? NextState { get; set; }
        }
    }
}
