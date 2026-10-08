// <copyright file="DiscordProvisioningResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// The result of the <see cref="DiscordServerProvisioner"/>.
/// </summary>
public sealed class DiscordProvisioningResult
{
    /// <summary>
    /// Gets the names of the created roles, categories and channels.
    /// </summary>
    public List<string> Created { get; } = new();

    /// <summary>
    /// Gets the names of the roles, categories and channels which already existed.
    /// </summary>
    public List<string> Adopted { get; } = new();

    /// <summary>
    /// Gets the names of the roles, categories and channels which couldn't be created or set up, with the reason.
    /// </summary>
    public List<(string Name, string Reason)> Failed { get; } = new();

    /// <summary>
    /// Gets the identifiers of the channels by their key.
    /// </summary>
    public Dictionary<string, ulong> ChannelIds { get; } = new(StringComparer.OrdinalIgnoreCase);
}
