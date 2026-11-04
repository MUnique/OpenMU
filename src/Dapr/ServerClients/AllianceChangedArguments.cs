// <copyright file="AllianceChangedArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for the notification that a guild joined or left an alliance.
/// </summary>
public record AllianceChangedArguments(uint MasterGuildId, uint MemberGuildId);
