// <copyright file="PlayerLoggedInArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for <see cref="IGameServer.PlayerAlreadyLoggedInAsync"/>
/// </summary>
public record PlayerLoggedInArguments(byte ServerId, string LoginName);