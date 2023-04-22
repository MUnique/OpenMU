// <copyright file="LoopbackIpResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Net;

/// <summary>
/// Resolves the ip address as 127.127.127.127.
/// Usually only used at development time.
/// </summary>
public class LoopbackIpResolver : CustomIpResolver
{
    /// <summary>
    /// Gets the local address.
    /// </summary>
    internal static IPAddress LoopbackAddress { get; } = new IPAddress(0x7F7F7F7F);

    /// <summary>
    /// Initializes a new instance of the <see cref="LoopbackIpResolver"/> class.
    /// </summary>
    public LoopbackIpResolver()
        : base(LoopbackAddress)
    {
    }
}