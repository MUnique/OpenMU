// <copyright file="GuildChatBinding.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A binding of the chat of a guild or alliance to a channel of an external service, e.g. Discord,
/// so that the messages are mirrored in both directions.
/// </summary>
/// <remarks>
/// It's not part of the <see cref="Guild"/> aggregate, because it's managed by the external service.
/// That's why it refers to its guild by identifier.
/// </remarks>
[AggregateRoot]
public class GuildChatBinding
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the guild. For an alliance, it's the master guild of the alliance.
    /// </summary>
    public Guid GuildId { get; set; }

    /// <summary>
    /// Gets or sets the scope, which tells if the chat of the guild or of its alliance is bound.
    /// </summary>
    public GuildChatScope Scope { get; set; }

    /// <summary>
    /// Gets or sets the name of the external service, e.g. <c>discord</c>.
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the server of the external service, e.g. of the Discord server.
    /// </summary>
    public string ExternalServerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the channel of the external service.
    /// </summary>
    public string ExternalChannelId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the channel is hosted by the server owner,
    /// i.e. it's in the Discord server of the game server and was created for the guild.
    /// Otherwise, it's in a Discord server of the guild.
    /// </summary>
    public bool IsHosted { get; set; }

    /// <summary>
    /// Gets or sets the name of the character which bound the chat.
    /// </summary>
    public string? BoundBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp at which the chat was bound.
    /// </summary>
    public DateTime BoundAt { get; set; }
}
