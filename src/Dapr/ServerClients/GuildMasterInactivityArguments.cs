// <copyright file="GuildMasterInactivityArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for the handling of inactive guild masters.
/// </summary>
public record GuildMasterInactivityArguments(TimeSpan InactivityLimit, int MaximumGuilds);
