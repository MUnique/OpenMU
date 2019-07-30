// <copyright file="ClientAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network.PlugIns;

    /// <summary>
    /// Attribute to mark an implemented <see cref="IViewPlugIn"/> with a specific client version.
    /// </summary>
    public class ClientAttribute : Attribute, IComparable<ClientAttribute>, IComparable, IEquatable<ClientAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAttribute"/> class.
        /// </summary>
        /// <param name="season">The minimum season of compatible clients.</param>
        /// <param name="episode">The minimum episode of compatible clients.</param>
        /// <param name="language">The language of compatible clients.</param>
        protected ClientAttribute(byte season, byte episode, ClientLanguage language)
        {
            this.Client = new ClientVersion(season, episode, language);
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public ClientVersion Client { get; }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(ClientAttribute left, ClientAttribute right) => Compare(left, right) > 0;

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >=(ClientAttribute left, ClientAttribute right) => Compare(left, right) >= 0;

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(ClientAttribute left, ClientAttribute right) => Compare(left, right) < 0;

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <=(ClientAttribute left, ClientAttribute right) => Compare(left, right) <= 0;

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ClientAttribute left, ClientAttribute right) => !(left == right);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ClientAttribute left, ClientAttribute right) => Compare(left, right) == 0;

        /// <inheritdoc />
        public int CompareTo(ClientAttribute other)
        {
            return this.Client.CompareTo(other?.Client ?? default);
        }

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as ClientAttribute);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        /// <inheritdoc />
        public bool Equals(ClientAttribute other)
        {
            return this.CompareTo(other) == 0;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => this.Client.GetHashCode();

        private static int Compare(ClientAttribute left, ClientAttribute right) => left.CompareTo(right);
    }
}