// <copyright file="DoppelgangerFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The feature plug-in of the doppelganger event. Its configuration describes the run of the event,
/// e.g. the monsters, their paths and multipliers, so that it can be adapted in the admin panel.
/// </summary>
/// <remarks>
/// The configuration is <see langword="null"/> until it's either seeded by the data initialization
/// or filled in by an administrator, because the monsters can only be referenced when the game
/// configuration is known. The <see cref="DoppelgangerContext"/> falls back to
/// <see cref="DoppelgangerEventDefinition.CreateDefault"/> in that case.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.DoppelgangerFeaturePlugIn_Name), Description = nameof(PlugInResources.DoppelgangerFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A4C7E2D9-3B5F-4E81-9C06-7D1F2B8E5A43")]
public class DoppelgangerFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<DoppelgangerEventDefinition>
{
    /// <inheritdoc />
    public DoppelgangerEventDefinition? Configuration { get; set; }
}
