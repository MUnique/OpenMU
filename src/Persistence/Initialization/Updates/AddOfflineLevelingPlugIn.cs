// <copyright file="AddOfflineLevelingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.OfflineLeveling;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Registers the <see cref="OfflineLevelingChatCommandPlugIn"/> and
/// <see cref="OfflineLevelingStopOnLoginPlugIn"/> in the database so that they are
/// active on existing installations that were initialized before these plugins existed.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("C2A5E8F3-4D1B-4E9A-7F6C-3B0D2A9E4C17")]
public class AddOfflineLevelingPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Offline Leveling";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Registers the offline leveling chat command and stop-on-login plugin configurations.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddOfflineLeveling;

    /// <inheritdoc />
    public override string DataInitializationKey => DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 3, 11, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddPlugInIfMissing<OfflineLevelingChatCommandPlugIn>(context, gameConfiguration);
        this.AddPlugInIfMissing<OfflineLevelingStopOnLoginPlugIn>(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }

    private void AddPlugInIfMissing<TPlugIn>(IContext context, GameConfiguration gameConfiguration)
    {
        var typeId = typeof(TPlugIn).GUID;

        if (gameConfiguration.PlugInConfigurations.Any(c => c.TypeId == typeId))
        {
            return;
        }

        var config = context.CreateNew<PlugInConfiguration>();
        config.SetGuid(typeId);
        config.TypeId = typeId;
        config.IsActive = true;
        gameConfiguration.PlugInConfigurations.Add(config);
    }
}
