// <copyright file="SpecialServerId.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Defines some server ids which are used for some specific states.
/// </summary>
public enum SpecialServerId : byte
{
    /// <summary>
    /// Gets the server id which represents being offline.
    /// </summary>
    Offline = 0xFF,

    /// <summary>
    /// Gets the server id which represents being invisible (=offline to other players).
    /// </summary>
    Invisible = 0xFE,
}
