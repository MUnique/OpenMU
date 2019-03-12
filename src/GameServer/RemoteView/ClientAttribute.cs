// <copyright file="ClientAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Attribute to mark an implemented <see cref="IViewPlugIn"/> with a specific client version.
    /// </summary>
    public class ClientAttribute : Attribute, IComparable<ClientAttribute>, IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAttribute"/> class.
        /// </summary>
        /// <param name="season">The minimum season of compatible clients.</param>
        /// <param name="episode">The minimum episode of compatible clients.</param>
        /// <param name="language">The language of compatible clients.</param>
        public ClientAttribute(byte season, byte episode, ClientLanguage language)
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
    }
}