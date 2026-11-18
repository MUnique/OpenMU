// <copyright file="FriendResponseArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// The arguments for a friend response.
/// </summary>
/// <param name="CharacterName">The character name.</param>
/// <param name="FriendName">The friend name.</param>
/// <param name="Accepted">A value indicating whether the request was accepted.</param>
public record FriendResponseArguments(string CharacterName, string FriendName, bool Accepted);