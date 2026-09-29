// <copyright file="ImperialGuardianFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The feature plug-in of the imperial guardian event. Its configuration describes the run of the event,
/// e.g. the times, the experience reward and the scaling of the monsters, so that it can be adapted in the admin panel.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ImperialGuardianFeaturePlugIn_Name), Description = nameof(PlugInResources.ImperialGuardianFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("7E3B9C15-2D84-4F60-A1B7-5C9E0D6F2A38")]
public class ImperialGuardianFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<ImperialGuardianEventDefinition>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public ImperialGuardianEventDefinition? Configuration { get; set; }

    /// <summary>
    /// Gets the definition of the imperial guardian event of the game context.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The definition of the event.</returns>
    public static ImperialGuardianEventDefinition GetEventDefinition(IGameContext gameContext)
    {
        return gameContext.FeaturePlugIns.GetPlugIn<ImperialGuardianFeaturePlugIn>()?.Configuration ?? new ImperialGuardianEventDefinition();
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new ImperialGuardianEventDefinition();
    }
}
