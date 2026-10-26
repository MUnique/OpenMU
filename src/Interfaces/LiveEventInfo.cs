// <copyright file="LiveEventInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>A periodic mini game's live schedule on one server.</summary>
/// <param name="Type">The invariant mini game type name.</param>
/// <param name="Name">The configured event name.</param>
/// <param name="Enabled">Whether the event is enabled on this server.</param>
/// <param name="Active">Whether an event is running.</param>
/// <param name="StartsIn">The duration until the next start, when available.</param>
/// <param name="Participants">The current number of active participants.</param>
public sealed record LiveEventInfo(string Type, string Name, bool Enabled, bool Active, TimeSpan? StartsIn, int Participants = 0);
