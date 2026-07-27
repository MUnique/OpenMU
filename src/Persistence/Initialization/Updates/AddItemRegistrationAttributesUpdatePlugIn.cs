// <copyright file="AddItemRegistrationAttributesUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the missing RegisteredRenas and TotalRegisteredRenas attributes.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("182FC652-3277-4CDB-8BA8-DE70311E67C9")]
public class AddItemRegistrationAttributesUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Item Registration Attributes";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds data for the item registration attributes.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddItemRegistrationAttributes;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 07, 24, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Attributes.All(a => a.Id != Stats.RegisteredRenas.Id))
        {
            var registeredRenas = context.CreateNew<AttributeDefinition>(Stats.RegisteredRenas.Id, Stats.RegisteredRenas.Designation, Stats.RegisteredRenas.Description);
            gameConfiguration.Attributes.Add(registeredRenas);
        }

        if (gameConfiguration.Attributes.All(a => a.Id != Stats.TotalRegisteredRenas.Id))
        {
            var totalRegisteredRenas = context.CreateNew<AttributeDefinition>(Stats.TotalRegisteredRenas.Id, Stats.TotalRegisteredRenas.Designation, Stats.TotalRegisteredRenas.Description);
            gameConfiguration.Attributes.Add(totalRegisteredRenas);
        }
    }
}
