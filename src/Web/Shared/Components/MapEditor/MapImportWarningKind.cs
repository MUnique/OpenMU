// <copyright file="MapImportWarningKind.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

/// <summary>
/// Defines the reason why an imported record was skipped.
/// </summary>
public enum MapImportWarningKind
{
    /// <summary>
    /// The map of the file does not exist in the target database.
    /// </summary>
    UnknownMapNumber,

    /// <summary>
    /// The monster definition of a spawn does not exist in the target database.
    /// </summary>
    UnknownMonsterNumber,

    /// <summary>
    /// The declared kind of a spawn does not match the kind of its target monster definition.
    /// </summary>
    KindMismatch,

    /// <summary>
    /// The kind of a spawn is not declared by the file.
    /// </summary>
    KindNotDeclared,

    /// <summary>
    /// The target of an enter gate is not part of the file.
    /// </summary>
    UnresolvedGateTarget,
}
