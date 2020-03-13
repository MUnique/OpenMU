// <copyright file="IManageableServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// General interface for a server which provides some information and functions to manage it from outside.
    /// </summary>
    public interface IManageableServer : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the identifier of the server.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the identifier of the configuration of the server.
        /// </summary>
        Guid ConfigurationId { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        ServerType Type { get; }

        /// <summary>
        /// Gets the current state of the server.
        /// </summary>
        ServerState ServerState { get; }

        /// <summary>
        /// Gets the maximum number of connections the server can handle.
        /// </summary>
        int MaximumConnections { get; }

        /// <summary>
        /// Gets the current connection count.
        /// </summary>
        int CurrentConnections { get; }

        /// <summary>
        /// Starts the server.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the server.
        /// </summary>
        void Shutdown();
    }
}