// <copyright file="ISupportDataChangedNotification.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanelBlazor.Services
{
    using System;

    /// <summary>
    /// Interface for classes which support to notify about changes in their data.
    /// </summary>
    public interface ISupportDataChangedNotification
    {
        event EventHandler DataChanged;
    }
}