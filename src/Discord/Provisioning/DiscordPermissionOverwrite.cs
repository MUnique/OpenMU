// <copyright file="DiscordPermissionOverwrite.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// An overwrite of the permissions of a channel, for a role or a user.
/// The access which is neither allowed nor denied stays as it is.
/// </summary>
/// <param name="TargetId">The identifier of the role or user.</param>
/// <param name="IsRole">A value indicating whether the target is a role; otherwise, it's a user.</param>
/// <param name="Allow">The allowed access.</param>
/// <param name="Deny">The denied access.</param>
public sealed record DiscordPermissionOverwrite(ulong TargetId, bool IsRole, DiscordChannelAccess Allow, DiscordChannelAccess Deny);
