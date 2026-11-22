// <copyright file="MapImportPreviewDialog.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.Globalization;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Properties;

/// <summary>
/// Shows the validated content of an import file and lets the user choose which kinds to import.
/// </summary>
public partial class MapImportPreviewDialog
{
    private HashSet<MapObjectKind> _selectedKinds = new();
    private IReadOnlyDictionary<MapObjectKind, int> _counts = new Dictionary<MapObjectKind, int>();

    /// <summary>
    /// Gets or sets the preview which is shown.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public MapImportPreview Preview { get; set; } = null!;

    /// <summary>
    /// Gets or sets the modal instance used to close the dialog.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public ModalInstance Modal { get; set; } = null!;

    private int ImportCount => this.Preview.CountImported(this._selectedKinds);

    private bool CanImport => this.ImportCount > 0;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this._counts = MapObjectKindExtensions.All.ToDictionary(kind => kind, kind => this.Preview.GetCount(kind));
        if (this._selectedKinds.Count == 0)
        {
            this._selectedKinds = new HashSet<MapObjectKind>(this.Preview.DeclaredKinds);
        }
    }

    private static string GetWarningText(MapImportWarning warning)
    {
        return warning.Kind switch
        {
            MapImportWarningKind.UnknownMapNumber => string.Format(
                CultureInfo.CurrentCulture,
                Resources.ImportWarningUnknownMap,
                warning.MapNumber),
            MapImportWarningKind.UnknownMonsterNumber => string.Format(
                CultureInfo.CurrentCulture,
                Resources.ImportWarningUnknownMonster,
                warning.MonsterNumber,
                warning.MapNumber),
            MapImportWarningKind.KindMismatch => string.Format(
                CultureInfo.CurrentCulture,
                Resources.ImportWarningKindMismatch,
                GetKindCaption(warning.ExpectedKind),
                warning.MapNumber,
                GetKindCaption(warning.ActualKind)),
            MapImportWarningKind.KindNotDeclared => string.Format(
                CultureInfo.CurrentCulture,
                Resources.ImportWarningKindNotDeclared,
                GetKindCaption(warning.ActualKind),
                warning.MapNumber),
            MapImportWarningKind.UnresolvedGateTarget => string.Format(
                CultureInfo.CurrentCulture,
                Resources.ImportWarningUnresolvedGate,
                warning.MapNumber,
                warning.GateKey),
            _ => string.Empty,
        };
    }

    private static string GetKindCaption(MapObjectKind? kind) =>
        kind is { } value ? value.GetCaption() : string.Empty;

    private string GetColumnClass(MapObjectKind kind) =>
        this._selectedKinds.Contains(kind) ? string.Empty : "text-muted opacity-50";

    private void OnSelectedKindsChanged(IReadOnlySet<MapObjectKind> kinds)
    {
        this._selectedKinds = new HashSet<MapObjectKind>(kinds);
    }

    private Task ConfirmAsync()
    {
        return this.Modal.CloseAsync(ModalResult.Ok<IReadOnlySet<MapObjectKind>>(this._selectedKinds));
    }
}
