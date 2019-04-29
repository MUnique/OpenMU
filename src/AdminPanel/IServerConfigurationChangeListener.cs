// <copyright file="IServerConfigurationChangeListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    /// <summary>
    /// A interface for an object which listens to changes in the server configuration.
    /// </summary>
    public interface IServerConfigurationChangeListener
    {
        /// <summary>
        /// Is called when a connection server was added.
        /// </summary>
        /// <param name="currentConfiguration">The current configuration.</param>
        void ConnectionServerAdded(DataModel.Configuration.ConnectServerDefinition currentConfiguration);

        /// <summary>
        /// Is called when a connection server configuration changed.
        /// </summary>
        /// <param name="currentConfiguration">The current configuration.</param>
        void ConnectionServerChanged(DataModel.Configuration.ConnectServerDefinition currentConfiguration);
    }
}