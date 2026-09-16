// <copyright file="ActorEndpointOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup.TestActors;

using System.Net;

/// <summary>
/// The options of the actor control endpoint.
/// </summary>
/// <param name="EndPoint">The address and port to listen on.</param>
public sealed record ActorEndpointOptions(IPEndPoint EndPoint)
{
    /// <summary>
    /// Parses the values of the two environment variables: the port, which enables the endpoint,
    /// and the optional address to bind. Without an address the IPv4 loopback address is bound;
    /// an operator who explicitly wants another one names it - e.g. <c>0.0.0.0</c> inside a
    /// container whose port publishing does the containment.
    /// </summary>
    /// <param name="port">The port variable's value.</param>
    /// <param name="address">The address variable's value, or <c>null</c> when it is unset.</param>
    /// <returns>The options, or <c>null</c> when the port is unset or either value is invalid.</returns>
    public static ActorEndpointOptions? TryParse(string? port, string? address)
    {
        if (string.IsNullOrWhiteSpace(port)
            || !int.TryParse(port.Trim(), out var portNumber)
            || portNumber is <= 0 or > 65535)
        {
            return null;
        }

        var bindAddress = IPAddress.Loopback;
        if (!string.IsNullOrWhiteSpace(address) && !IPAddress.TryParse(address.Trim(), out bindAddress))
        {
            return null;
        }

        return new ActorEndpointOptions(new IPEndPoint(bindAddress, portNumber));
    }
}
