// <copyright file="WeeklyQuestServiceCollectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.WeeklyQuests;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MUnique.OpenMU.Persistence.WeeklyQuests;

/// <summary>
/// Extensions to register the persistence of the weekly quests.
/// </summary>
public static class WeeklyQuestServiceCollectionExtensions
{
    /// <summary>
    /// Adds the database backed <see cref="IWeeklyQuestProgressRepository"/> to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same instance, to allow chaining of further calls.</returns>
    public static IServiceCollection AddWeeklyQuestProgressRepository(this IServiceCollection services)
    {
        services.TryAddSingleton<IWeeklyQuestProgressRepository, WeeklyQuestProgressRepository>();
        return services;
    }
}
