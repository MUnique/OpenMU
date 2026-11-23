// <copyright file="IRoundTripViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// The view which answers the client's round trip requests, with which it measures its ping.
/// </summary>
public interface IRoundTripViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Answers a round trip request of the client.
    /// </summary>
    /// <param name="token">The token of the request.</param>
    ValueTask AnswerRoundTripAsync(uint token);
}
