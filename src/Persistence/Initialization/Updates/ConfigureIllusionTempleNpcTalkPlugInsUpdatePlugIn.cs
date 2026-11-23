// <copyright file="ConfigureIllusionTempleNpcTalkPlugInsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using System.Text.Json;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Configures the NPCs of the Illusion Temple talk plug-ins: the stone statue (380) and the
/// item storages of the allied (383) and the illusion forces (384).
/// </summary>
/// <remarks>
/// <see cref="IllusionTempleDataUpdatePlugIn"/> doesn't set the NPC of these plug-ins, which leaves them with
/// an empty configuration in an existing database, so talking to these NPCs does nothing.
/// A new database already configures them. This update only sets NPCs which are not set yet,
/// so it doesn't overwrite a configuration which was changed by the server operator.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.ConfigureIllusionTempleNpcTalkPlugInsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.ConfigureIllusionTempleNpcTalkPlugInsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("5D2E7A41-9C3B-4F68-A1D5-0E8B73C4F296")]
public class ConfigureIllusionTempleNpcTalkPlugInsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Configure Illusion Temple NPCs";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "The Illusion Temple update left the NPC of the stone statue and of the two item storages unset in the configuration of their talk plug-ins, so talking to them did nothing. This update sets the NPCs which are still unset.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 22, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(IllusionTempleDataUpdatePlugIn).GUID, typeof(ConfigureNpcTalkPlugInsPlugInSeason6).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        ConfigureIfNpcIsNotSet(context, gameConfiguration, new IllusionTempleStoneStatueTalkPlugIn());
        ConfigureIfNpcIsNotSet(context, gameConfiguration, new IllusionTempleAllianceStorageTalkPlugIn());
        ConfigureIfNpcIsNotSet(context, gameConfiguration, new IllusionTempleIllusionStorageTalkPlugIn());
        return ValueTask.CompletedTask;
    }

    private static void ConfigureIfNpcIsNotSet(IContext context, GameConfiguration gameConfiguration, NpcTalkPlugInBase plugIn)
    {
        var typeId = plugIn.GetType().GUID;
        if (gameConfiguration.PlugInConfigurations.FirstOrDefault(c => c.TypeId == typeId) is not { } plugInConfiguration)
        {
            plugInConfiguration = context.CreateNew<PlugInConfiguration>();
            plugInConfiguration.SetGuid(typeId);
            plugInConfiguration.TypeId = typeId;
            plugInConfiguration.IsActive = true;
            gameConfiguration.PlugInConfigurations.Add(plugInConfiguration);
        }
        else if (IsNpcSet(plugInConfiguration))
        {
            return;
        }

        plugInConfiguration.SetConfiguration(plugIn.CreateDefaultConfig(gameConfiguration), new ConfigureNpcTalkPlugInsPlugInBase.IdReferenceHandler());
    }

    private static bool IsNpcSet(PlugInConfiguration plugInConfiguration)
    {
        if (string.IsNullOrWhiteSpace(plugInConfiguration.CustomConfiguration))
        {
            return false;
        }

        using var document = JsonDocument.Parse(plugInConfiguration.CustomConfiguration);
        return document.RootElement.ValueKind == JsonValueKind.Object
               && document.RootElement.TryGetProperty(nameof(NpcTalkPlugInConfiguration.Npc), out var npc)
               && npc.ValueKind != JsonValueKind.Null;
    }
}
