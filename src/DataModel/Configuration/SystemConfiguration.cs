// <copyright file="SystemConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Properties;
using MUnique.OpenMU.Network;

/// <summary>
/// System-wide configuration values.
/// </summary>
[AggregateRoot]
[Cloneable]
[Display(ResourceType = typeof(Resources), Name = nameof(Resources.SystemConfiguration_Name), Description = nameof(Resources.SystemConfiguration_Description))]
public partial class SystemConfiguration
{
    /// <summary>
    /// Gets or sets the type of the ip resolver.
    /// </summary>
    [Display(
         Order = 1,
         Name = nameof(Resources.SystemConfiguration_IpResolver_Name),
         Description = nameof(Resources.SystemConfiguration_IpResolver_Description),
         GroupName = nameof(Resources.SystemConfiguration_IpResolver_Name),
         ResourceType = typeof(Resources))]
    public IpResolverType IpResolver { get; set; }

    /// <summary>
    /// Gets or sets the ip resolver parameter, when <see cref="IpResolverType.Custom"/>
    /// is used.
    /// </summary>
    [Display(
        Order = 2,
        Name = nameof(Resources.SystemConfiguration_IpResolverParameter_Name),
        Description = nameof(Resources.SystemConfiguration_IpResolverParameter_Description),
        GroupName = nameof(Resources.SystemConfiguration_IpResolver_Name),
        ResourceType = typeof(Resources))]
    public string? IpResolverParameter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically start
    /// the listeners of the servers when the server process starts.
    /// Only applicable to the All-In-One Startup. The distributed processes
    /// always start their listeners automatically.
    /// </summary>
    [Display(
        Order = 3,
        Name = nameof(Resources.SystemConfiguration_AutoStart_Name),
        Description = nameof(Resources.SystemConfiguration_AutoStart_Description),
        ResourceType = typeof(Resources))]
    public bool AutoStart { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically update the
    /// database schema when the server process starts.
    /// Only applicable to the All-In-One Startup. In a distributed deployment,
    /// the user must start the update manually over the admin panel
    /// </summary>
    [Display(
        Order = 4,
        Name = nameof(Resources.SystemConfiguration_AutoUpdateSchema_Name),
        Description = nameof(Resources.SystemConfiguration_AutoUpdateSchema_Description),
        ResourceType = typeof(Resources))]
    public bool AutoUpdateSchema { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user can enter commands in
    /// the console of the All-In-One Startup process.
    /// </summary>
    [Display(
        Order = 5,
        Name = nameof(Resources.SystemConfiguration_ReadConsoleInput_Name),
        Description = nameof(Resources.SystemConfiguration_ReadConsoleInput_Description),
        ResourceType = typeof(Resources))]
    public bool ReadConsoleInput { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return "System Configuration";
    }
}