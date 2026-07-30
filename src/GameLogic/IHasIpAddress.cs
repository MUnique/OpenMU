// <copyright file="IHasIpAddress.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Interface for objects that expose a remote IP address.
/// </summary>
public interface IHasIpAddress
{
    /// <summary>
    /// Gets the IP address of the remote connection.
    /// </summary>
    string? IpAddress { get; }
}
