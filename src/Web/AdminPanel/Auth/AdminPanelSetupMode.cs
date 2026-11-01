// <copyright file="AdminPanelSetupMode.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Knows whether the admin panel is in its initial setup mode, and disables the API keys which
/// were created in it.
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
    private readonly IApiKeyRepository _apiKeys;
    private readonly BootstrapAdminUserProvider _bootstrapUserProvider;
    private readonly ILogger<AdminPanelSetupMode> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPanelSetupMode"/> class.
    /// </summary>
    /// <param name="users">The repository of the stored users.</param>
    /// <param name="apiKeys">The repository of the stored API keys.</param>
    /// <param name="bootstrapUserProvider">The provider of the bootstrap user.</param>
    /// <param name="logger">The logger.</param>
    public AdminPanelSetupMode(IAdminUserRepository users, IApiKeyRepository apiKeys, BootstrapAdminUserProvider bootstrapUserProvider, ILogger<AdminPanelSetupMode> logger)
    {
        this._users = users;
        this._apiKeys = apiKeys;
        this._bootstrapUserProvider = bootstrapUserProvider;
        this._logger = logger;
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

    /// <summary>
    /// Gets the time at which the oldest stored user has been created. API keys which are older
    /// were created while the admin panel had no user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The creation time of the oldest stored user; <c>null</c>, if there is none.</returns>
    /// <remarks>
    /// When users have been deleted, the oldest remaining one is younger than the first user was,
    /// so keys of that time count as created before the first user as well.
    /// </remarks>
    public async ValueTask<DateTime?> GetFirstUserCreatedAtAsync(CancellationToken cancellationToken = default)
    {
        var users = await this._users.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return users.Count == 0 ? null : users.Min(user => user.CreatedAt);
    }

    /// <summary>
    /// Disables every enabled API key. It's called when the first user ends the setup mode: every
    /// key which exists then was created while anybody could create one.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of keys which have been disabled.</returns>
    public async ValueTask<int> DisableApiKeysAsync(CancellationToken cancellationToken = default)
    {
        var disabled = new List<ApiKey>();
        foreach (var apiKey in await this._apiKeys.GetAllAsync(cancellationToken).ConfigureAwait(false))
        {
            if (apiKey.IsDisabled)
            {
                continue;
            }

            apiKey.IsDisabled = true;
            await this._apiKeys.UpdateAsync(apiKey, cancellationToken).ConfigureAwait(false);
            disabled.Add(apiKey);
        }

        if (disabled.Count > 0)
        {
            this._logger.LogWarning(
                "The first user of the admin panel ends its setup mode. {Count} API key(s) which were created while the admin panel had no user have been disabled: {Keys}",
                disabled.Count,
                string.Join(", ", disabled.Select(apiKey => $"{apiKey.Name} ({apiKey.KeyPrefix}…)")));
        }

        return disabled.Count;
    }
}
