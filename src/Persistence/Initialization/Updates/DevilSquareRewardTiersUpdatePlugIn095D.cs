// <copyright file="DevilSquareRewardTiersUpdatePlugIn095D.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update configures the rewards of the Devil Square of version 0.95d by tiers.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DevilSquareRewardTiersUpdatePlugIn_Name), Description = nameof(PlugInResources.DevilSquareRewardTiersUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E5B3C9B6-0600-45D1-B77E-0CE158C145E5")]
public class DevilSquareRewardTiersUpdatePlugIn095D : DevilSquareRewardTiersUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
