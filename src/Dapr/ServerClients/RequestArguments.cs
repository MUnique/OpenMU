// <copyright file="RequestArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Common arguments for a request of a player to another player.
/// </summary>
/// <param name="Requester">The requester character name.</param>
/// <param name="Receiver">The receiver character name.</param>
public record RequestArguments(string Requester, string Receiver);