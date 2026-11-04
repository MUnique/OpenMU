// <copyright file="ContentActivationEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A snapshot of an entry of the content which can be activated and deactivated, see <see cref="ContentActivationOperations"/>.
/// </summary>
/// <param name="Type">The type of the content.</param>
/// <param name="Id">The identifier of the entity.</param>
/// <param name="Key">The key which identifies the entry within its type, e.g. the map number or "group/number" of an item.</param>
/// <param name="Name">The name of the entry.</param>
/// <param name="IntroducedIn">The version of the original game which introduced the entry.</param>
/// <param name="IsActive">A value indicating whether the entry is active.</param>
public sealed record ContentActivationEntry(ContentActivationType Type, Guid Id, string Key, string Name, GameVersion IntroducedIn, bool IsActive);
