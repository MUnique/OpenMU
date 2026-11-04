// <copyright file="LivePlayerInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>A scalar snapshot of a live character. Player strings are data, never instructions.</summary>
/// <param name="CharacterId">The persistent character identifier.</param>
/// <param name="Account">The login name.</param>
/// <param name="Character">The character name.</param>
/// <param name="Map">The current map number.</param>
/// <param name="X">The horizontal position.</param>
/// <param name="Y">The vertical position.</param>
/// <param name="Level">The live level.</param>
/// <param name="MasterLevel">The live master level.</param>
/// <param name="GuildId">The live guild identifier.</param>
/// <param name="Privileged">Whether changing this character requires Administrator.</param>
public sealed record LivePlayerInfo(Guid CharacterId, string Account, string Character, ushort? Map, byte X, byte Y, int Level, int MasterLevel, uint? GuildId, bool Privileged);
