// <copyright file="UnavailableApiKeyRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// A fallback <see cref="IApiKeyRepository"/> which is used when the hosting application didn't
/// register a real one.
/// </summary>
/// <remarks>
/// It behaves like an empty storage, so only the configured keys work. Unlike the users, this
/// doesn't warn on its own: the admin user repository already does, and both are registered together.
/// </remarks>
public class UnavailableApiKeyRepository : IApiKeyRepository
{
    private const string NotAvailableMessage = "The API key storage is not available.";

    /// <inheritdoc />
    public ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(false);

    /// <inheritdoc />
    public ValueTask<IList<ApiKey>> GetAllAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult<IList<ApiKey>>(new List<ApiKey>());

    /// <inheritdoc />
    public ValueTask<ApiKey?> GetEnabledByHashAsync(string keyHash, CancellationToken cancellationToken = default)
        => ValueTask.FromResult<ApiKey?>(null);

    /// <inheritdoc />
    public ValueTask AddAsync(ApiKey apiKey, CancellationToken cancellationToken = default) => throw new InvalidOperationException(NotAvailableMessage);

    /// <inheritdoc />
    public ValueTask UpdateAsync(ApiKey apiKey, CancellationToken cancellationToken = default) => throw new InvalidOperationException(NotAvailableMessage);

    /// <inheritdoc />
    /// <inheritdoc />
    public ValueTask DeleteAsync(ApiKey apiKey, CancellationToken cancellationToken = default) => throw new InvalidOperationException(NotAvailableMessage);
}
