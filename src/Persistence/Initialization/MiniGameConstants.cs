// <copyright file="MiniGameConstants.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

/// <summary>
/// The constants which the mini game initializers and the corresponding updates share.
/// </summary>
public static class MiniGameConstants
{
    /// <summary>
    /// The entrance duration which every event entrance announces, so that players always get
    /// the same amount of time to walk to the event gate.
    /// </summary>
    public static readonly TimeSpan EnterDuration = TimeSpan.FromMinutes(5);
}
