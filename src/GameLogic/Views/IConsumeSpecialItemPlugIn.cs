// <copyright file="IDrinkAlcoholPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs about the alcohol consumption of the own character. The character appears to be red (drunken).
/// </summary>
public interface IConsumeSpecialItemPlugIn : IViewPlugIn
{
    /// <summary>
    /// Consumes the special item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="effectTimeInSeconds">The effect time in seconds.</param>
    /// <returns></returns>
    ValueTask ConsumeSpecialItemAsync(Item item, ushort effectTimeInSeconds);
}