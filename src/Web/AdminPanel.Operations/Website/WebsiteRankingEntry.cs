// <copyright file="WebsiteRankingEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// An entry of the character ranking of a website.
/// </summary>
/// <param name="Rank">The rank, starting at 1.</param>
/// <param name="Name">The name of the character.</param>
/// <param name="ClassName">The name of the character class.</param>
/// <param name="Level">The level.</param>
/// <param name="MasterLevel">The master level.</param>
/// <param name="Resets">The number of resets.</param>
/// <param name="GuildName">The name of the guild of the character; <c>null</c>, if it isn't a member of a guild.</param>
public sealed record WebsiteRankingEntry(int Rank, string Name, string ClassName, int Level, int MasterLevel, int Resets, string? GuildName);
