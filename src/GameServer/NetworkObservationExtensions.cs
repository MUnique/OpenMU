// <copyright file="NetworkObservationExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Network.Analyzer.Archive;
using MUnique.OpenMU.Persistence;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// Extensions to register the archive for the traffic of observed accounts.
/// </summary>
public static class NetworkObservationExtensions
{
    /// <summary>
    /// Adds the archive for the traffic of observed accounts, configured by the system
    /// configuration of the database.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// It belongs to the game server, not to the admin panel: an observed account is archived
    /// as soon as it plays, no matter whether an admin panel is running somewhere.
    /// </remarks>
    public static IServiceCollection AddNetworkObservation(this IServiceCollection services)
    {
        return services
            .AddSingleton(CreateOptions)
            .AddSingleton<IPacketArchive, PacketArchive>();
    }

    /// <summary>
    /// Creates the options of the network observation from the given system configuration.
    /// </summary>
    /// <param name="configuration">The system configuration, if it exists.</param>
    /// <returns>The options of the network observation.</returns>
    public static NetworkObservationOptions CreateOptions(SystemConfiguration? configuration)
    {
        var options = new NetworkObservationOptions();
        if (configuration is null)
        {
            return options;
        }

        // A value which was never configured is left at its default, so that an existing
        // database doesn't end up with an unlimited archive.
        if (!string.IsNullOrWhiteSpace(configuration.NetworkObservationArchivePath))
        {
            options.ArchivePath = configuration.NetworkObservationArchivePath;
        }

        if (configuration.NetworkObservationMaxSessionSizeMb > 0)
        {
            options.MaximumSessionSizeMb = configuration.NetworkObservationMaxSessionSizeMb;
        }

        if (configuration.NetworkObservationMaxTotalSizeMb > 0)
        {
            options.MaximumTotalSizeMb = configuration.NetworkObservationMaxTotalSizeMb;
        }

        if (configuration.NetworkObservationRetentionDays > 0)
        {
            options.RetentionDays = configuration.NetworkObservationRetentionDays;
        }

        return options;
    }

    private static NetworkObservationOptions CreateOptions(IServiceProvider serviceProvider)
    {
        try
        {
            if (serviceProvider.GetService<IPersistenceContextProvider>() is { } contextProvider)
            {
                using var context = contextProvider.CreateNewTypedContext(typeof(SystemConfiguration), false);
                var configuration = context.GetAsync<SystemConfiguration>().AsTask().WaitAndUnwrapException().FirstOrDefault();
                return CreateOptions(configuration);
            }
        }
        catch (Exception ex)
        {
            serviceProvider.GetService<ILoggerFactory>()?
                .CreateLogger(typeof(NetworkObservationExtensions))
                .LogWarning(ex, "Could not read the configuration of the network observation. The defaults are used.");
        }

        return new NetworkObservationOptions();
    }
}
