// <copyright file="IShowGuildWarResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Interface of a view whose implementation informs about a guild war result after the war has ended.
/// </summary>
public interface IShowGuildWarResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the guild war result.
    /// </summary>
    /// <param name="hostileGuildName">Name of the hostile guild.</param>
    /// <param name="result">The result.</param>
    ValueTask ShowResultAsync(string hostileGuildName, GuildWarResult result);
}