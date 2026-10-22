// <copyright file="ICastleSiegeOwnerLogoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which shows the guild logo of the castle owner, e.g. on the castle banners.
/// </summary>
public interface ICastleSiegeOwnerLogoPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the guild logo of the castle owner.
    /// </summary>
    /// <param name="logo">The guild logo, or an empty memory when the castle has no owner.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    ValueTask ShowOwnerLogoAsync(ReadOnlyMemory<byte> logo);
}
