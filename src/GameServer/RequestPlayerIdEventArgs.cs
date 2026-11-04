// <copyright file="RequestPlayerIdEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using System.ComponentModel;

/// <summary>
/// Event arguments for the <see cref="IGameServerListener.PlayerConnected"/> event.
/// </summary>
public class RequestPlayerIdEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPlayerIdEventArgs"/> class.
    /// </summary>
    public RequestPlayerIdEventArgs()
        : base(true)
    {
    }

    /// <summary>
    /// Gets or sets the player identifier.
    /// </summary>
    public ushort PlayerId { get; set; }
}