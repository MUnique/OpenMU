// <copyright file="ConfigureCastleSiegeRegistrationUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Configures Sign of Lord registration for an existing Season 6 Castle Siege configuration.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("D91757B1-0C3D-4336-8DEC-20438EDA7F09")]
public class ConfigureCastleSiegeRegistrationUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Configure Castle Siege registration";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update configures the item used for Castle Siege Sign of Lord registration.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ConfigureCastleSiegeRegistration;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 08, 06, 14, 30, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var configuration = gameConfiguration.CastleSiegeConfiguration
            ?? throw new InvalidOperationException("The Castle Siege configuration does not exist.");
        new CastleSiegeInitializer(context, gameConfiguration).InitializeRegistration(configuration);
        return ValueTask.CompletedTask;
    }
}
