// <copyright file="CastleSiegeTaxType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// Defines a Castle Siege tax type.
/// </summary>
public enum CastleSiegeTaxType : byte
{
    /// <summary>
    /// No tax type.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The percentage added to Chaos Machine crafting costs.
    /// </summary>
    ChaosMachine = 1,

    /// <summary>
    /// The percentage added to NPC store prices.
    /// </summary>
    Store = 2,

    /// <summary>
    /// The flat Land of Trials entrance fee.
    /// </summary>
    HuntZone = 3,
}
