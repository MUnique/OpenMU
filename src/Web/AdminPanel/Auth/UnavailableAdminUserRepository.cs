// <copyright file="UnavailableAdminUserRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// A fallback <see cref="IAdminUserRepository"/> which is used when the hosting application didn't
/// register a real one, for example when the admin panel is started without a persistence provider.
/// </summary>
/// <remarks>
/// It behaves like an empty storage, so the panel starts in its initial setup mode instead of
/// failing to resolve its services. Only the configured bootstrap user can log in then.
/// </remarks>
public class UnavailableAdminUserRepository : IAdminUserRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnavailableAdminUserRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public UnavailableAdminUserRepository(ILogger<UnavailableAdminUserRepository> logger)
    {
        logger.LogWarning(
            "No storage for admin panel users is registered, so no user can be created or stored. "
            + "Call {MethodName} in the hosting application to enable it.",
            "AddAdminUserRepository");
    }

    /// <inheritdoc />
    public ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(false);

    /// <inheritdoc />
    public ValueTask<int> GetCountAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(0);

    /// <inheritdoc />
    public ValueTask<IList<AdminUser>> GetAllAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult<IList<AdminUser>>(new List<AdminUser>());

    /// <inheritdoc />
    public ValueTask<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ValueTask.FromResult<AdminUser?>(null);

    /// <inheritdoc />
    public ValueTask<AdminUser?> GetByNormalizedLoginNameAsync(string normalizedLoginName, CancellationToken cancellationToken = default)
        => ValueTask.FromResult<AdminUser?>(null);

    /// <inheritdoc />
    public ValueTask AddAsync(AdminUser user, CancellationToken cancellationToken = default) => throw this.CreateException();

    /// <inheritdoc />
    public ValueTask UpdateAsync(AdminUser user, CancellationToken cancellationToken = default) => throw this.CreateException();

    /// <inheritdoc />
    public ValueTask DeleteAsync(AdminUser user, CancellationToken cancellationToken = default) => throw this.CreateException();

    private InvalidOperationException CreateException()
        => new("No storage for admin panel users is registered.");
}
