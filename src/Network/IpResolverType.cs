namespace MUnique.OpenMU.Network;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Defines how the own IP should be resolved and reported to the game client.
/// </summary>
public enum IpResolverType
{
    /// <summary>
    /// The IP is detected automatically by considering the environment.
    /// </summary>
    [Display(ResourceType = typeof(Resources), Name = nameof(Resources.IpResolverType_Auto_Name), Description = nameof(Resources.IpResolverType_Auto_Description))]
    Auto,

    /// <summary>
    /// The IP is detected by asking an external service. As a result, we resolve
    /// to the public IP.
    /// </summary>
    [Display(ResourceType = typeof(Resources), Name = nameof(Resources.IpResolverType_Public_Name), Description = nameof(Resources.IpResolverType_Public_Description))]
    Public,

    /// <summary>
    /// The IP is detected by asking the DNS about the IP of the own host name.
    /// </summary>
    [Display(ResourceType = typeof(Resources), Name = "IpResolverType_Local_Name", Description = "IpResolverType_Local_Description")]
    Local,

    /// <summary>
    /// The IP is resolved as '127.127.127.127'.
    /// </summary>
    [Display(ResourceType = typeof(Resources), Name = "IpResolverType_Loopback_Name", Description = "IpResolverType_Loopback_Description")]
    Loopback,

    /// <summary>
    /// The IP or a host name can be set manually.
    /// </summary>
    [Display(ResourceType = typeof(Resources), Name = "IpResolverType_Custom_Name", Description = "IpResolverType_Custom_Description")]
    Custom,
}