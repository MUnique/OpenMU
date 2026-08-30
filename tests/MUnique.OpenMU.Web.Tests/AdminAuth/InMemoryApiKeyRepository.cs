// <copyright file="InMemoryApiKeyRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// An in-memory <see cref="IApiKeyRepository"/> for the tests.
/// </summary>
public class InMemoryApiKeyRepository : IApiKeyRepository
{
    private readonly List<ApiKey> _apiKeys = new();

    /// <inheritdoc />
    public ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(true);

    /// <inheritdoc />
    public ValueTask<IList<ApiKey>> GetAllAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult<IList<ApiKey>>(this._apiKeys.OrderBy(k => k.Name).ToList());

    /// <inheritdoc />
    public ValueTask<ApiKey?> GetEnabledByHashAsync(string keyHash, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(this._apiKeys.FirstOrDefault(k => k.KeyHash == keyHash && !k.IsDisabled));

    /// <inheritdoc />
    public ValueTask AddAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
    {
        this._apiKeys.Add(apiKey);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask UpdateAsync(ApiKey apiKey, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

    /// <inheritdoc />
    public ValueTask DeleteAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
    {
        this._apiKeys.Remove(apiKey);
        return ValueTask.CompletedTask;
    }
}
