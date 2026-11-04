// <copyright file="CastleSiegeMagicEffectNumber.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The client-visible magic effects which identify a player's Castle Siege side.
/// </summary>
public enum CastleSiegeMagicEffectNumber : short
{
    /// <summary>
    /// The defending side.
    /// </summary>
    Defense = 14,

    /// <summary>
    /// The first attacking side.
    /// </summary>
    Attack1 = 15,

    /// <summary>
    /// The second attacking side.
    /// </summary>
    Attack2 = 16,

    /// <summary>
    /// The third attacking side.
    /// </summary>
    Attack3 = 17,
}
