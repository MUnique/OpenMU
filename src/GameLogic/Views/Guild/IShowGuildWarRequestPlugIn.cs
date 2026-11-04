// <copyright file="IShowGuildWarRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

using MUnique.OpenMU.GameLogic.GuildWar;

/// <summary>
/// Interface of a view whose implementation informs about a guild war request of another guild master.
/// </summary>
public interface IShowGuildWarRequestPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the guild war request.
    /// </summary>
    /// <param name="requestingGuildName">Name of the requesting guild.</param>
    /// <param name="warType">Type of the war.</param>
    ValueTask ShowRequestAsync(string requestingGuildName, GuildWarType warType);
}