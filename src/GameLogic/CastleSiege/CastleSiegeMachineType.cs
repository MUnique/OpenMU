// <copyright file="CastleSiegeMachineType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// The available Castle Siege warfare-machine types.
/// </summary>
public enum CastleSiegeMachineType : byte
{
    /// <summary>
    /// A machine operated by an attacking side.
    /// </summary>
    Attack = 1,

    /// <summary>
    /// A machine operated by the defending side.
    /// </summary>
    Defense = 2,
}
