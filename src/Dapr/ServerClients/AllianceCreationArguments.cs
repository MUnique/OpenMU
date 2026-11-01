// <copyright file="AllianceCreationArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for creating an alliance.
/// </summary>
public record AllianceCreationArguments(uint MasterGuildId, uint TargetGuildId);
