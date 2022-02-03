// <copyright file="SetOnlineStateArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

public record SetOnlineStateArguments(Guid CharacterId, string CharacterName, int ServerId);