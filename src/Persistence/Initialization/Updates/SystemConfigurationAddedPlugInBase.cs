// <copyright file="SystemConfigurationAddedPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Network;

/// <summary>
/// This updates adds the new <see cref="SystemConfiguration"/> with default settings.
/// </summary>
public abstract class SystemConfigurationAddedPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add System Configuration";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the new system configuration.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2023, 03, 24, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if ((await context.GetAsync<SystemConfiguration>().ConfigureAwait(false)).FirstOrDefault() is { })
        {
            // Already exists -> we can skip that.
            return;
        }

        var systemConfiguration = context.CreateNew<SystemConfiguration>();
        systemConfiguration.SetGuid(0);
        systemConfiguration.AutoStart = true;
        systemConfiguration.AutoUpdateSchema = true;
        systemConfiguration.ReadConsoleInput = false;

        var (type, param) = IpAddressResolverFactory.DetermineBestFittingResolver(Environment.GetCommandLineArgs());
        systemConfiguration.IpResolver = type;
        systemConfiguration.IpResolverParameter = param;
    }
}