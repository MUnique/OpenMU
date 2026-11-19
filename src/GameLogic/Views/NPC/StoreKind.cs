// <copyright file="StoreKind.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// The kind of a store of the shown dialog.
/// </summary>
public enum StoreKind
{
    /// <summary>
    /// The merchant store dialog.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// The chaos machine dialog.
    /// </summary>
    ChaosMachine = 3,

    /// <summary>
    /// A failed resurrection (of Dark Horse or Dark Raven) storage dialog.
    /// </summary>
    ResurrectionFailed = 5,
}
