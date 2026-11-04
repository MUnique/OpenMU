// <copyright file="SocketSubOptionType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The <see cref="ItemOption.SubOptionType"/>s for socket options.
/// </summary>
public enum SocketSubOptionType
{
    /// <summary>
    /// The fire seed option type.
    /// </summary>
    Fire = 0,

    /// <summary>
    /// The water seed option type.
    /// </summary>
    Water = 1,

    /// <summary>
    /// The ice seed option type.
    /// </summary>
    Ice = 2,

    /// <summary>
    /// The wind seed option type.
    /// </summary>
    Wind = 3,

    /// <summary>
    /// The lightning seed option type.
    /// </summary>
    Lightning = 4,

    /// <summary>
    /// The earth seed option type.
    /// </summary>
    Earth = 5,
}