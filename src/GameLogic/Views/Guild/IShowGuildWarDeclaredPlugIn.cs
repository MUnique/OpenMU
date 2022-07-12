// <copyright file="IShowGuildWarDeclaredPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Interface of a view whose implementation informs about a declared guild war.
/// </summary>
public interface IShowGuildWarDeclaredPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows, that the guild war was declared.
    /// </summary>
    ValueTask ShowDeclaredAsync();
}