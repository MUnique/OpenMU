// <copyright file="GuildCreationArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

public record GuildCreationArguments(string Name, string MasterName, Guid MasterId, byte[] Logo, byte ServerId);