// <copyright file="DiscordRoleInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// An existing role of a Discord server.
/// </summary>
/// <param name="Id">The identifier.</param>
/// <param name="Name">The name.</param>
public sealed record DiscordRoleInfo(ulong Id, string Name);
