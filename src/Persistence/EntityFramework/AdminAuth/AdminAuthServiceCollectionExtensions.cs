// <copyright file="AdminAuthServiceCollectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.AdminAuth;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Extensions to register the persistence of the admin panel users.
/// </summary>
public static class AdminAuthServiceCollectionExtensions
{
    /// <summary>
    /// Adds the database backed <see cref="IAdminUserRepository"/> and <see cref="IApiKeyRepository"/>
    /// to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same instance, to allow chaining of further calls.</returns>
    public static IServiceCollection AddAdminUserRepository(this IServiceCollection services)
    {
        services.TryAddSingleton<IAdminUserRepository, AdminUserRepository>();
        services.TryAddSingleton<IApiKeyRepository, ApiKeyRepository>();
        return services;
    }
}
