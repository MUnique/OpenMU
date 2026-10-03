// <copyright file="ConfigureNpcTalkPlugInsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Text.Json.Serialization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update configures the NPCs of the plugins which handle talking to an NPC (<see cref="NpcTalkPlugInBase"/>).
/// Before, these plugins knew the NPC numbers in their code.
/// </summary>
public abstract class ConfigureNpcTalkPlugInsPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Configure NPCs of talk plugins";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update configures the NPCs of the plugins which handle talking to an NPC, e.g. the reset NPC or the Blood Castle Archangel.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 3, 14, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var plugInTypes = typeof(NpcTalkPlugInBase).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(NpcTalkPlugInBase)) && !type.IsAbstract);
        foreach (var plugInType in plugInTypes)
        {
            if (gameConfiguration.PlugInConfigurations.FirstOrDefault(c => c.TypeId == plugInType.GUID) is not { } plugInConfiguration)
            {
                plugInConfiguration = context.CreateNew<PlugInConfiguration>();
                plugInConfiguration.SetGuid(plugInType.GUID);
                plugInConfiguration.TypeId = plugInType.GUID;
                plugInConfiguration.IsActive = true;
                gameConfiguration.PlugInConfigurations.Add(plugInConfiguration);
            }

            var plugIn = (NpcTalkPlugInBase)Activator.CreateInstance(plugInType)!;
            plugInConfiguration.SetConfiguration(plugIn.CreateDefaultConfig(gameConfiguration), new IdReferenceHandler());
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// A reference handler which serializes objects of the configuration by their id.
    /// </summary>
    private sealed class IdReferenceHandler : ReferenceHandler
    {
        /// <inheritdoc />
        public override ReferenceResolver CreateResolver() => new IdReferenceResolver();
    }

    /// <summary>
    /// A reference resolver which serializes objects of the configuration by their id.
    /// </summary>
    private sealed class IdReferenceResolver : ReferenceResolver
    {
        /// <inheritdoc />
        public override void AddReference(string referenceId, object value)
        {
            // Only used for serialization.
        }

        /// <inheritdoc />
        public override string GetReference(object value, out bool alreadyExists)
        {
            if (value is IIdentifiable identifiable)
            {
                alreadyExists = true;
                return identifiable.Id.ToString();
            }

            alreadyExists = false;
            return string.Empty;
        }

        /// <inheritdoc />
        public override object ResolveReference(string referenceId) => throw new NotSupportedException($"This resolver only serializes references, so it can't resolve the reference '{referenceId}'.");
    }
}
