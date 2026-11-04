// <copyright file="GameServerAdministrationRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Text.Json.Nodes;
using MUnique.OpenMU.Interfaces;

/// <summary>Bounded arguments for the internal live administration request/response channel.</summary>
public sealed class GameServerAdministrationRequest
{
    /// <summary>Gets or sets a unique delivery identifier reused by transport retries.</summary>
    public Guid RequestId { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the expected server identifier.</summary>
    public int ServerId { get; set; }

    /// <summary>Gets or sets the exact character name.</summary>
    public string CharacterName { get; set; } = string.Empty;

    /// <summary>Gets or sets the expected character identifier.</summary>
    public Guid CharacterId { get; set; }

    /// <summary>Gets or sets the target map or optional map filter.</summary>
    public ushort? MapNumber { get; set; }

    /// <summary>Gets or sets the optional live guild filter.</summary>
    public uint? GuildId { get; set; }

    /// <summary>Gets or sets the horizontal coordinate.</summary>
    public byte X { get; set; }

    /// <summary>Gets or sets the vertical coordinate.</summary>
    public byte Y { get; set; }

    /// <summary>Gets or sets the level.</summary>
    public int Level { get; set; }

    /// <summary>Gets or sets a value indicating whether to set master level.</summary>
    public bool Master { get; set; }

    /// <summary>Gets or sets the structured item.</summary>
    public ItemSpecification? Item { get; set; }

    /// <summary>Gets or sets the invariant event type.</summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether an active event may be restarted.</summary>
    public bool Restart { get; set; }

    /// <summary>Gets or sets the event state approved in the preview.</summary>
    public LiveEventInfo? ExpectedEvent { get; set; }

    /// <summary>Gets or sets the number of matches to skip.</summary>
    public int Offset { get; set; }

    /// <summary>Gets or sets the maximum page length.</summary>
    public int Limit { get; set; } = 50;

    /// <summary>Gets or sets the contributed plugin identifier.</summary>
    public Guid PlugInId { get; set; }

    /// <summary>Gets or sets the invariant contributed operation name.</summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>Gets or sets the bounded JSON plugin input.</summary>
    public string Arguments { get; set; } = "{}";

    /// <summary>Gets or sets the approved preview.</summary>
    public JsonObject? ExpectedPreview { get; set; }
}
