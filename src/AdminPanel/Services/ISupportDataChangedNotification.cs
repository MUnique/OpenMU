// <copyright file="ISupportDataChangedNotification.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System;

    /// <summary>
    /// Interface for classes which support to notify about changes in their data.
    /// </summary>
    public interface ISupportDataChangedNotification
    {
        /// <summary>
        /// Occurs when data has been changed and needs to be updated on the user interface.
        /// </summary>
        event EventHandler DataChanged;
    }
}