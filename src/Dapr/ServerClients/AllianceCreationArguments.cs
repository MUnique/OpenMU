// <copyright file="AllianceCreationArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for creating an alliance.
/// </summary>
/// <param name="MasterGuildId">The master guild identifier.</param>
/// <param name="TargetGuildId">The target guild identifier.</param>
public record AllianceCreationArguments(uint MasterGuildId, uint TargetGuildId);
