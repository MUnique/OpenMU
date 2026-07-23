// <copyright file="IGoldenArcherRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

using System.Threading.Tasks;

/// <summary>
/// Interface of a view whose event chip registration result is shown.
/// </summary>
public interface IGoldenArcherRegistrationResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the event chip registration result.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask RegistrationResultAsync();
}
