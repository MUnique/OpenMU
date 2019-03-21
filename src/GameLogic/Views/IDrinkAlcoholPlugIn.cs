// <copyright file="IDrinkAlcoholPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    /// <summary>
    /// Interface of a view whose implementation informs about the alcohol consumption of the own character. The character appears to be red (drunken).
    /// </summary>
    public interface IDrinkAlcoholPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the effects of drinking alcohol.
        /// </summary>
        void DrinkAlcohol();
    }
}