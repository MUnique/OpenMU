// <copyright file="GuildInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// Information about a guild.
/// </summary>
/// <param name="Name">The name.</param>
/// <param name="MasterName">The name of the guild master.</param>
/// <param name="MemberCount">The number of members.</param>
/// <param name="OnlineMemberNames">The names of the members which are online.</param>
public sealed record GuildInfo(string Name, string? MasterName, int MemberCount, IReadOnlyList<string> OnlineMemberNames);
