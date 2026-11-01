// <copyright file="CharacterInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// Information about a character.
/// </summary>
/// <param name="Name">The name.</param>
/// <param name="ClassName">The name of the class, as value of a localized string.</param>
/// <param name="Level">The level.</param>
/// <param name="MasterLevel">The master level.</param>
/// <param name="Resets">The resets.</param>
/// <param name="OnlineServerName">The name of the game server on which the character is online; <see langword="null"/>, if offline.</param>
public sealed record CharacterInfo(string Name, string ClassName, int Level, int MasterLevel, int Resets, string? OnlineServerName);
