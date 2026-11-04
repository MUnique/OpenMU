// <copyright file="FixIcarusSafezonePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Sets the Icarus safezone to Devias.
/// </summary>
public abstract class FixIcarusSafezonePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    protected const string PlugInName = "Fix Icarus Safezone";

    /// <summary>
    /// The plug in description.
    /// </summary>
    protected const string PlugInDescription = "Sets the Icarus safezone to Devias.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 12, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var icarus = gameConfiguration.Maps.FirstOrDefault(map => map.Number == Version095d.Maps.Icarus.Number);
        var devias = gameConfiguration.Maps.FirstOrDefault(map => map.Number == Version075.Maps.Devias.Number);
        if (icarus is not null && devias is not null)
        {
            icarus.SafezoneMap = devias;
        }

        return ValueTask.CompletedTask;
    }
}
