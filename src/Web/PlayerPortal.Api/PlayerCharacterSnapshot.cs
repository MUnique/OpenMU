// <copyright file="PlayerCharacterSnapshot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

/// <summary>A persisted character snapshot without private inventory, credentials, or account IDs.</summary>
public sealed record PlayerCharacterSnapshot(string Name, string? ClassName, byte? ClassNumber, int Level, int MasterLevel);
