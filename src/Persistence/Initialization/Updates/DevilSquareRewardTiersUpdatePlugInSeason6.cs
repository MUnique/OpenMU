// <copyright file="DevilSquareRewardTiersUpdatePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update configures the rewards of the Devil Square of season 6 by tiers.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DevilSquareRewardTiersUpdatePlugIn_Name), Description = nameof(PlugInResources.DevilSquareRewardTiersUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("53811655-DA54-4042-ADB6-7EDB63F5B403")]
public class DevilSquareRewardTiersUpdatePlugInSeason6 : DevilSquareRewardTiersUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
