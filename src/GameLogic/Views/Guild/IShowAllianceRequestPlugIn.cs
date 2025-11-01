// <copyright file="IShowAllianceRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Interface of a view whose implementation shows an alliance request to the guild master.
/// </summary>
public interface IShowAllianceRequestPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the alliance request from the specified guild.
    /// </summary>
    /// <param name="requesterGuildName">Name of the requester guild.</param>
    ValueTask ShowRequestAsync(string requesterGuildName);
}
