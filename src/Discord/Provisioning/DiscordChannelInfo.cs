// <copyright file="DiscordChannelInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// An existing channel or category of a Discord server.
/// </summary>
/// <param name="Id">The identifier.</param>
/// <param name="Name">The name.</param>
/// <param name="IsCategory">A value indicating whether it's a category.</param>
/// <param name="CategoryId">The identifier of the category of the channel, if it has one.</param>
public sealed record DiscordChannelInfo(ulong Id, string Name, bool IsCategory, ulong? CategoryId);
