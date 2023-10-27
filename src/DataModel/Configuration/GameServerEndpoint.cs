// <copyright file="GameServerEndpoint.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines an endpoint of a game server.
/// </summary>
[Cloneable]
public partial class GameServerEndpoint : ServerEndpoint
{
    /// <summary>
    /// Gets or sets the alternative published network port. It's reported to the connect server instead of the <see cref="ServerEndpoint.NetworkPort"/>, if not <c>0</c>.
    /// This allows to run a network analyzer program as a proxy, which listens to this <see cref="AlternativePublishedPort"/> and forwards to <see cref="ServerEndpoint.NetworkPort"/>
    /// without changing the connection address of the game client.
    /// This may also be useful if you use port forwarding over NAT.
    /// </summary>
    public int AlternativePublishedPort { get; set; }
}