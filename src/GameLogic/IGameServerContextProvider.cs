// <copyright file="IGameServerContextProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Interface for an implementation which provides an <see cref="IGameServerContext"/>.
/// </summary>
public interface IGameServerContextProvider
{
    /// <summary>
    /// Gets the game server context.
    /// </summary>
    IGameServerContext Context { get; }
}