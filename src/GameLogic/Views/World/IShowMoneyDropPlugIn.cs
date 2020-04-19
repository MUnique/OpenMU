// <copyright file="IShowMoneyDropPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World
{
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Interface of a view whose implementation informs about dropped zen.
    /// </summary>
    public interface IShowMoneyDropPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Show money on the ground.
        /// </summary>
        /// <param name="itemId">the id of item(money) in map.</param>
        /// <param name="quantity">the quantity to drop.</param>
        /// <param name="point">the ground position.</param>
        void ShowMoney(ushort itemId, uint quantity, Point point);
    }
}