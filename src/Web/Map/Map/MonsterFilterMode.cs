// <copyright file="MonsterFilterMode.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Map;

/// <summary>
/// Defines how the monster filter of the live map compares the name of a monster with its text.
/// The values match the <c>MonsterFilterMode</c> enum of the map scripts.
/// </summary>
public enum MonsterFilterMode
{
    /// <summary>
    /// The name contains the text.
    /// </summary>
    Contains = 0,

    /// <summary>
    /// The name starts with the text.
    /// </summary>
    StartsWith = 1,

    /// <summary>
    /// The name ends with the text.
    /// </summary>
    EndsWith = 2,
}
