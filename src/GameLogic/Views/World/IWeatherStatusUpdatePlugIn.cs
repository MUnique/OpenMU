// <copyright file="IWeatherStatusUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about the weather of the current map.
/// </summary>
public interface IWeatherStatusUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the weather of the current map.
    /// </summary>
    /// <param name="weather">The weather, usually a value between 0 and 2 (inclusive).</param>
    /// <param name="variation">The variation, usually a value between 0 and 9 (inclusive).</param>
    ValueTask ShowWeatherAsync(byte weather, byte variation);
}