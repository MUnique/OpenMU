// <copyright file="ClientVersion.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns
{
    using System;

    /// <summary>
    /// A definition about a client version.
    /// </summary>
    public struct ClientVersion : IComparable<ClientVersion>, IComparable, IEquatable<ClientVersion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientVersion"/> struct.
        /// </summary>
        /// <param name="season">The season.</param>
        /// <param name="episode">The episode.</param>
        /// <param name="language">The language.</param>
        public ClientVersion(byte season, byte episode, ClientLanguage language)
        {
            this.Season = season;
            this.Episode = episode;
            this.Language = language;
        }

        /// <summary>
        /// Gets the season of the compatible client.
        /// </summary>
        public byte Season { get; }

        /// <summary>
        /// Gets the episode of the compatible client.
        /// </summary>
        public byte Episode { get; }

        /// <summary>
        /// Gets the language of the compatible client.
        /// </summary>
        public ClientLanguage Language { get; }

        private int CombinedVersion => (this.Season << 8) + this.Episode;

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(ClientVersion left, ClientVersion right) => Compare(left, right) > 0;

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >=(ClientVersion left, ClientVersion right) => Compare(left, right) > 0;

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(ClientVersion left, ClientVersion right) => Compare(left, right) < 0;

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <=(ClientVersion left, ClientVersion right) => Compare(left, right) < 0;

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ClientVersion left, ClientVersion right) => !(left == right);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ClientVersion left, ClientVersion right) => Compare(left, right) == 0;

        /// <inheritdoc />
        public int CompareTo(ClientVersion other)
        {
            if (other.Language != ClientLanguage.Invariant && this.Language != other.Language)
            {
                return int.MinValue;
            }

            var result = this.CombinedVersion - other.CombinedVersion;
            result *= 10;
            if (this.Language != ClientLanguage.Invariant && other.Language == ClientLanguage.Invariant)
            {
                result++;
            }

            return result;
        }

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            if (obj is ClientVersion other)
            {
                return this.CompareTo(other);
            }

            return int.MinValue;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (this.Season > 0)
            {
                return $"Season {this.Season} Episode {this.Episode} {this.Language}";
            }

            return $"{this.Season}.{this.Episode} {this.Language}";
        }

        /// <inheritdoc/>
        public bool Equals(ClientVersion other)
        {
            return this.CompareTo(other) == 0;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ClientVersion other && this.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Season.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Episode.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.Language;
                return hashCode;
            }
        }

        private static int Compare(ClientVersion left, ClientVersion right) => left.CompareTo(right);
    }
}
