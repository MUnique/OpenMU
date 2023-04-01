// <copyright file="IChangeNotificationService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.ComponentModel;

/// <summary>
/// Interface for a service which notifies about changes of data within the admin panel.
/// </summary>
public interface IChangeNotificationService
{
    /// <summary>
    /// Occurs when a property of an object changed.
    /// </summary>
    event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies all subscribers of <see cref="PropertyChanged"/> about the change.
    /// </summary>
    /// <param name="sender">The sender of the event, usually the changed object.</param>
    /// <param name="propertyName">Name of the property.</param>
    void NotifyChange(object? sender, string? propertyName);
}