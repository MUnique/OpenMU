// <copyright file="FriendResponseArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

public record FriendResponseArguments(string CharacterName, string FriendName, bool Accepted);