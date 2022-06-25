// <copyright file="IUserService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Interface for a service to manage admin panel users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets the users.
    /// </summary>
    ICollection<string> Users { get; }

    /// <summary>
    /// Gets a value indicating whether managing users is available.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Deletes a user with the specified name.
    /// </summary>
    /// <param name="user">The username.</param>
    Task DeleteUserAsync(string user);

    /// <summary>
    /// Creates a new Account in a modal dialog.
    /// </summary>
    Task CreateNewInModalDialogAsync();

    /// <summary>
    /// Changes the password of a user in a modal dialog.
    /// </summary>
    /// <param name="user">The username.</param>
    Task ChangePasswordInModalDialogAsync(string user);
}