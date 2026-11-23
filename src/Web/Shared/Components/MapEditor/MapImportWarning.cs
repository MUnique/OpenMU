// <copyright file="MapImportWarning.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

/// <summary>
/// Describes a record which was skipped during the validation of an import file.
/// </summary>
/// <param name="Kind">The reason why the record was skipped.</param>
/// <param name="MapNumber">The number of the affected map.</param>
/// <param name="MonsterNumber">The monster number of the affected spawn, if applicable.</param>
/// <param name="ExpectedKind">The kind declared by the file, if applicable.</param>
/// <param name="ActualKind">The kind of the target monster definition, if applicable.</param>
/// <param name="GateKey">The file-local key of the unresolved target gate, if applicable.</param>
public sealed record MapImportWarning(
    MapImportWarningKind Kind,
    short MapNumber,
    int? MonsterNumber = null,
    MapObjectKind? ExpectedKind = null,
    MapObjectKind? ActualKind = null,
    int? GateKey = null);
