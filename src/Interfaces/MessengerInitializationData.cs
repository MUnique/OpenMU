// <copyright file="MessengerInitializationData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Collections.Immutable;

/// <summary>
/// The data of a messenger initialization for <see cref="IFriendSystemSubscriber.InitializeMessengerAsync"/>.
/// </summary>
/// <param name="PlayerName">The name of the player.</param>
/// <param name="Friends">The list of friends.</param>
/// <param name="OpenFriendRequests">The list of open friend requests.</param>
public record MessengerInitializationData(string PlayerName, IImmutableList<string> Friends, IImmutableList<string> OpenFriendRequests);
