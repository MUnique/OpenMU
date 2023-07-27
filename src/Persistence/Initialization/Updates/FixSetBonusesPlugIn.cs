// <copyright file="FixSetBonusesPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This updates adds the new <see cref="SystemConfiguration"/> with default settings.
/// </summary>
public abstract class FixSetBonusesPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Item Set Bonuses";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes the set bonuses for additional defense of +10~15 sets and the defense rate.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2023, 04, 22, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var itemSetGroups = await context.GetAsync<ItemSetGroup>().ConfigureAwait(false);
        var unassignedSets = itemSetGroups.Where(s => !gameConfiguration.ItemSetGroups.Contains(s));
        foreach (var set in unassignedSets)
        {
            gameConfiguration.ItemSetGroups.Add(set);
        }
    }

    /// <summary>
    /// The <see cref="FixSetBonusesPlugIn"/> for season 6.
    /// </summary>
    [PlugIn(PlugInName, PlugInDescription)]
    [Guid("F858E471-B76D-4AAF-8886-8DEB45BC1AB8")]
    public class Season6 : FixSetBonusesPlugIn
    {
        /// <inheritdoc />
        public override UpdateVersion Version => UpdateVersion.FixSetBonusesSeason6;

        /// <inheritdoc />
        public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
    }

    /// <summary>
    /// The <see cref="FixSetBonusesPlugIn"/> for version 0.95d.
    /// </summary>
    [PlugIn(PlugInName, PlugInDescription)]
    [Guid("3D0201C3-D956-4BDD-9D57-3F6FD921EDF7")]
    public class V095d : FixSetBonusesPlugIn
    {
        /// <inheritdoc />
        public override UpdateVersion Version => UpdateVersion.FixSetBonuses095d;

        /// <inheritdoc />
        public override string DataInitializationKey => Version095d.DataInitialization.Id;
    }

    /// <summary>
    /// The <see cref="FixSetBonusesPlugIn"/> for version 0.75.
    /// </summary>
    [PlugIn(PlugInName, PlugInDescription)]
    [Guid("2E009ADF-1580-4E03-BA59-C9C51DC109BA")]
    public class V075 : FixSetBonusesPlugIn
    {
        /// <inheritdoc />
        public override UpdateVersion Version => UpdateVersion.FixSetBonuses075;

        /// <inheritdoc />
        public override string DataInitializationKey => Version075.DataInitialization.Id;
    }
}