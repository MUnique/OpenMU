// <copyright file="IShowAllianceResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

using MUnique.OpenMU.GameLogic.PlayerActions.Guild;

/// <summary>
/// Interface of a view whose implementation shows the result of an alliance operation.
/// </summary>
public interface IShowAllianceResponsePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of an alliance operation.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="targetGuildName">Name of the target guild.</param>
    ValueTask ShowResponseAsync(AllianceResponse response, string targetGuildName);
}
