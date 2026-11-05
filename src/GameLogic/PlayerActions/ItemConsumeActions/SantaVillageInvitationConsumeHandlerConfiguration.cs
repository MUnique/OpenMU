// <copyright file="SantaVillageInvitationConsumeHandlerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Configuration for the <see cref="SantaVillageInvitationConsumeHandlerPlugIn"/>.
/// </summary>
public class SantaVillageInvitationConsumeHandlerConfiguration
{
    /// <summary>
    /// Gets or sets the number of the map to which the invitation warps the player.
    /// The default is the santa village, about which the game client asks the player before it uses the invitation.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.SantaVillageInvitationConsumeHandlerConfiguration_MapNumber_Name))]
    public short MapNumber { get; set; } = 62;
}
