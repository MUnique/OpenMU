// <copyright file="WebsiteCharacterSummary.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// A character of the account of a player who logged in at the website.
/// </summary>
/// <param name="Name">The name of the character.</param>
/// <param name="ClassName">The name of the character class.</param>
/// <param name="Level">The level.</param>
/// <param name="MasterLevel">The master level.</param>
/// <param name="Resets">The number of resets.</param>
/// <param name="Money">The zen in the inventory of the character.</param>
/// <param name="MapNumber">The number of the map on which the character is; <c>null</c>, if it's unknown.</param>
/// <param name="MapName">The name of the map on which the character is; <c>null</c>, if it's unknown.</param>
/// <param name="PositionX">The x coordinate of the character on its map.</param>
/// <param name="PositionY">The y coordinate of the character on its map.</param>
public sealed record WebsiteCharacterSummary(string Name, string ClassName, int Level, int MasterLevel, int Resets, int Money, int? MapNumber, string? MapName, int PositionX, int PositionY);
