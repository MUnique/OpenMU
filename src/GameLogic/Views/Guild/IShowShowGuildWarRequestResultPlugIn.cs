// <copyright file="IShowShowGuildWarRequestResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Interface of a view whose implementation informs about a guild war request result (response) of the requested guild master.
/// </summary>
public interface IShowShowGuildWarRequestResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowResultAsync(GuildWarRequestResult result);
}