// <copyright file="AddItemStatProfilesUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Adds the item stat profiles, which hold the item formulas as data, and gives each item the profile of its kind.
/// </summary>
/// <remarks>
/// It only adds data: profiles which exist (by name) and items which have a profile stay as they are,
/// and the item formulas compute the same values with the added profiles as without them.
/// The drop level bonuses and the new item fields get their values from the database migration.
/// </remarks>
public abstract class AddItemStatProfilesUpdatePlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => PlugInResources.AddItemStatProfilesUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddItemStatProfilesUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 23, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        ItemStatProfiles.Apply(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
