// <copyright file="ManageableServerCommandArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a command to a <see cref="IManageableServer"/>.
/// </summary>
/// <param name="ServerId">The identifier of the server. The command is published to all processes
/// which host manageable servers, and the processes which don't host this server ignore it.</param>
/// <param name="Command">The command, e.g. <see cref="IManageableServer.StartAsync"/> or <see cref="IManageableServer.ShutdownAsync"/>.</param>
public record ManageableServerCommandArguments(int ServerId, string Command);
