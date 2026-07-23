// <copyright file=""AddGoldenArcherConfigurationPlugIn.cs"" company=""MUnique"">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the data for the golden archer configuration.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid(""182FC652-3277-4CDB-8BA8-DE70311E67C9"")]
public class AddGoldenArcherConfigurationPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = ""Add golden archer configuration"";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = ""This update adds data for the golden archer configuration"";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddGoldenArcherConfiguration;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 07, 23, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.GoldenArcherConfiguration is not null)
        {
            return;
        }

        var config = context.CreateNew<GoldenArcherConfiguration>();
        config.RequiredRenas = 1;
        config.RewardZen = 5000000;
        config.ItemDropChance = 100.0;

        gameConfiguration.GoldenArcherConfiguration = config;
    }
}
