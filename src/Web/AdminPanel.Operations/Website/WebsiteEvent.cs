// <copyright file="WebsiteEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// A scheduled event, like a mini game, an invasion or the happy hour.
/// </summary>
/// <param name="Id">
/// The identifier of the plugin which runs the event. It doesn't change, so a website should recognize
/// an event by it, e.g. to show its own name or picture for it.
/// </param>
/// <param name="Name">The english name of the event.</param>
/// <param name="NextStartUtc">The next start time, in UTC.</param>
public sealed record WebsiteEvent(Guid Id, string Name, DateTime NextStartUtc);
