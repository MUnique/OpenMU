// <copyright file="SetIntroducedGameVersionsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// This update sets in which version of the original game the maps, character classes, monsters,
/// items and mini games were introduced, see <see cref="IntroducedGameVersions"/>.
/// The database migration adds them as <see cref="GameVersion.Unknown"/>.
/// </summary>
public abstract class SetIntroducedGameVersionsPlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.SetIntroducedGameVersionsPlugInBase_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.SetIntroducedGameVersionsPlugInBase_Description;

    /// <inheritdoc />
    /// <remarks>
    /// It's mandatory, because it only sets versions which are still unknown and therefore
    /// can't overwrite anything the server administrator configured.
    /// </remarks>
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 2, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        IntroducedGameVersions.Apply(gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
