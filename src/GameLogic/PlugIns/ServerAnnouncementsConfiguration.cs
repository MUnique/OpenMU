// <copyright file="ServerAnnouncementsConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Configuration for the <see cref="ServerAnnouncementsPlugIn"/>.
/// </summary>
public class ServerAnnouncementsConfiguration
{
    /// <summary>
    /// Gets or sets the interval between one announcement and the next.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ServerAnnouncementsConfiguration_Interval_Name), Description = nameof(PlugInResources.ServerAnnouncementsConfiguration_Interval_Description))]
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Gets or sets the announcements, which are shown one after another in their order.
    /// After the last one, it starts over with the first one.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ServerAnnouncementsConfiguration_Messages_Name), Description = nameof(PlugInResources.ServerAnnouncementsConfiguration_Messages_Description))]
    public ICollection<Announcement> Messages { get; set; } = new List<Announcement>();

    /// <summary>
    /// One announcement of the rotation.
    /// </summary>
    public class Announcement
    {
        /// <summary>
        /// Gets or sets the text of the announcement.
        /// </summary>
        [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ServerAnnouncementsConfiguration_Text_Name))]
        public LocalizedString Text { get; set; }

        /// <summary>
        /// Gets or sets how the announcement is shown.
        /// </summary>
        [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ServerAnnouncementsConfiguration_Type_Name), Description = nameof(PlugInResources.ServerAnnouncementsConfiguration_Type_Description))]
        public MessageType Type { get; set; } = MessageType.GoldenCenter;

        /// <inheritdoc />
        public override string ToString() => this.Text.ToString() ?? string.Empty;
    }
}
