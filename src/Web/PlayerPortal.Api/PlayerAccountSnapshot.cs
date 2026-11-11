// <copyright file="PlayerAccountSnapshot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

/// <summary>A persisted, owner-authorized player account snapshot.</summary>
public sealed record PlayerAccountSnapshot(string LoginName, IReadOnlyList<PlayerCharacterSnapshot> Characters);
