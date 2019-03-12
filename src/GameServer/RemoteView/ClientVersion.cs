// <copyright file="ClientVersion.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;

    /// <summary>
    /// A definition about a client version.
    /// </summary>
    public struct ClientVersion : IComparable<ClientVersion>, IComparable
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

        /// <inheritdoc />
        public int CompareTo(ClientVersion other)
        {
            if (other.Language != ClientLanguage.Invariant && this.Language != other.Language)
            {
                return int.MinValue;
            }

            var result = this.CombinedVersion - other.CombinedVersion;
            result *= 10;
            if (other.Language == ClientLanguage.Invariant)
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
    }
}
