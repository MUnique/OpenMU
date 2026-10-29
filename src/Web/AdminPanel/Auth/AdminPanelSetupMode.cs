// <copyright file="AdminPanelSetupMode.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Knows whether the admin panel is in its initial setup mode.
/// </summary>
/// <remarks>
/// While the admin panel has no user and no bootstrap user, anybody who reaches it gets in. So
/// anybody could have created an API key then, and the key would keep working after the first
/// user exists. Unlike <see cref="AdminUserAvailabilityService"/>, which answers the authorization
/// of every request from a cached value, this asks the storage each time; it's meant for single
/// actions like creating a key or a user.
/// </remarks>
public class AdminPanelSetupMode
{
    private readonly IAdminUserRepository _users;
    private readonly BootstrapAdminUserProvider _bootstrapUserProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPanelSetupMode"/> class.
    /// </summary>
    /// <param name="users">The repository of the stored users.</param>
    /// <param name="bootstrapUserProvider">The provider of the bootstrap user.</param>
    public AdminPanelSetupMode(IAdminUserRepository users, BootstrapAdminUserProvider bootstrapUserProvider)
    {
        this._users = users;
        this._bootstrapUserProvider = bootstrapUserProvider;
    }

    /// <summary>
    /// Determines whether the admin panel has no user right now, asking the storage.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <c>true</c>, if there is neither a bootstrap user nor a stored user, or if the storage isn't
    /// available, so that it can't tell; otherwise, <c>false</c>.
    /// </returns>
    public async ValueTask<bool> IsActiveAsync(CancellationToken cancellationToken = default)
    {
        if (this._bootstrapUserProvider.User is not null)
        {
            return false;
        }

        if (!await this._users.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
        {
            return true;
        }

        return await this._users.GetCountAsync(cancellationToken).ConfigureAwait(false) == 0;
    }
}
