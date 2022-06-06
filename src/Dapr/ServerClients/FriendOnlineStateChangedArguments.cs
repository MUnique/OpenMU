// <copyright file="FriendOnlineStateChangedArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// The arguments for a friend online state change.
/// </summary>
public record FriendOnlineStateChangedArguments(string Player, string Friend, int ServerId);