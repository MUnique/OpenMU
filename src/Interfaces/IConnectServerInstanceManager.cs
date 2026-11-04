// <copyright file="IConnectServerInstanceManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface for an instance which manages connect servers.
/// </summary>
public interface IConnectServerInstanceManager
{
    /// <summary>
    /// Initializes a connect server with the specified definition.
    /// </summary>
    /// <param name="connectServerDefinitionId">The connect server definition identifier.</param>
    ValueTask InitializeConnectServerAsync(Guid connectServerDefinitionId);

    /// <summary>
    /// Removes the connect server instance of the specified definition.
    /// </summary>
    /// <param name="connectServerDefinitionId">The connect server definition identifier.</param>
    ValueTask RemoveConnectServerAsync(Guid connectServerDefinitionId);
}