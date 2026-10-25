// <copyright file="ManageableServerGroup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// A group of <see cref="IManageableServer"/>s of this process, whose number is known at runtime only,
/// e.g. one connect server for each connect server definition.
/// </summary>
/// <param name="Servers">The servers.</param>
public sealed record ManageableServerGroup(IEnumerable<IManageableServer> Servers);
