// <copyright file="ItemStatTermState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Whether a part of <see cref="ItemStatTerms"/> is written by its profile.
/// </summary>
/// <remarks>
/// The values are stored in the database, so new ones are only appended.
/// </remarks>
public enum ItemStatTermState
{
    /// <summary>
    /// The profile doesn't write the part, so the part of the profile named "default" applies.
    /// </summary>
    NotWritten,

    /// <summary>
    /// The part doesn't apply: the stat has no level bonus table, or excellent or ancient items get nothing more.
    /// </summary>
    None,

    /// <summary>
    /// The profile writes the part with its values.
    /// </summary>
    Values,
}
