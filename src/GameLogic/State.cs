// <copyright file="State.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A state of a state machine.
    /// </summary>
    public class State : IEquatable<State>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public State(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the unique id of a state.
        /// </summary>
        public Guid Id
        {
            get;
        }

        /// <summary>
        /// Gets or sets the name of the state.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the possible transitions to which this state can advance.
        /// </summary>
        public ICollection<State> PossibleTransitions
        {
            get;
            set;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(State lhs, State rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(State lhs, State rhs)
        {
            return !(lhs == rhs);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => this.Equals(obj as State);

        /// <inheritdoc />
        public bool Equals(State other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Returns the name of the state.
        /// </summary>
        /// <returns>The name of the state.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
