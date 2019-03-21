﻿// <copyright file="IStatIncreaseResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    using MUnique.OpenMU.AttributeSystem;

    /// <summary>
    /// Interface of a view whose implementation informs about an increased stat point.
    /// </summary>
    public interface IStatIncreaseResultPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the stat increase result.
        /// </summary>
        /// <param name="statType">Type of the stat.</param>
        /// <param name="success">if set to <c>true</c> the increment was successful.</param>
        void StatIncreaseResult(AttributeDefinition statType, bool success);
    }
}