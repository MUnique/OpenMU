// <copyright file="MapImportWorkflow.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Properties;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Runs the interactive import flow: validate, show the preview, apply the selected kinds.
/// </summary>
public sealed class MapImportWorkflow
{
    private readonly MapExportImportService _service;
    private readonly IModalService _modalService;
    private readonly IToastService _toastService;
    private readonly LoadingOverlayService _loadingOverlay;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapImportWorkflow"/> class.
    /// </summary>
    /// <param name="service">The export/import service.</param>
    /// <param name="modalService">The modal service.</param>
    /// <param name="toastService">The toast service.</param>
    /// <param name="loadingOverlay">The loading overlay.</param>
    public MapImportWorkflow(
        MapExportImportService service,
        IModalService modalService,
        IToastService toastService,
        LoadingOverlayService loadingOverlay)
    {
        this._service = service;
        this._modalService = modalService;
        this._toastService = toastService;
        this._loadingOverlay = loadingOverlay;
    }

    /// <summary>
    /// Runs the import flow.
    /// </summary>
    /// <param name="json">The JSON content of the selected file.</param>
    /// <param name="targetMaps">The maps which are available as import target.</param>
    /// <param name="includeGates">Whether gates may be imported. They are only supported for all maps at once.</param>
    /// <param name="context">The persistence context.</param>
    /// <param name="allMaps">All maps in their regular order, to tell apart maps which share a number.</param>
    /// <returns><see langword="true"/> if changes were applied; otherwise, <see langword="false"/>.</returns>
    public async ValueTask<bool> RunAsync(
        string json,
        IReadOnlyCollection<GameMapDefinition> targetMaps,
        bool includeGates,
        IContext context,
        IReadOnlyCollection<GameMapDefinition>? allMaps = null)
    {
        MapImportPreview preview;
        using (this._loadingOverlay.ShowLoadingIndicator())
        {
            // Gives the overlay the chance to render before the (synchronous) validation starts.
            await Task.Yield();
            preview = await this._service
                .PreviewAsync(json, targetMaps, includeGates, context, allMaps)
                .ConfigureAwait(true);
        }

        if (preview.Error is { } error)
        {
            this._toastService.ShowError(GetErrorMessage(error));
            return false;
        }

        if (!preview.HasChanges)
        {
            this._toastService.ShowInfo(Resources.NothingToImport);
            return false;
        }

        var parameters = new ModalParameters();
        parameters.Add(nameof(MapImportPreviewDialog.Preview), preview);
        var modal = this._modalService.Show<MapImportPreviewDialog>(
            Resources.ImportPreview,
            parameters,
            new ModalOptions { Size = ModalSize.Large });

        var result = await modal.Result.ConfigureAwait(true);
        if (result.Cancelled || result.Data is not IReadOnlySet<MapObjectKind> selectedKinds)
        {
            return false;
        }

        using (this._loadingOverlay.ShowLoadingIndicator())
        {
            // Gives the overlay the chance to render before thousands of objects are replaced.
            await Task.Yield();
            await this._service.ApplyAsync(preview, selectedKinds, context).ConfigureAwait(true);
        }

        this._toastService.ShowSuccess(string.Format(
            CultureInfo.CurrentCulture,
            Resources.ImportSucceeded,
            preview.CountImported(selectedKinds)));

        if (preview.Warnings.Count > 0)
        {
            this._toastService.ShowWarning(string.Format(
                CultureInfo.CurrentCulture,
                Resources.ImportCompletedWithWarnings,
                preview.Warnings.Count));
        }

        return true;
    }

    private static string GetErrorMessage(MapImportError error)
    {
        return error switch
        {
            MapImportError.InvalidJson => Resources.ImportInvalidJson,
            MapImportError.UnsupportedFormatVersion => Resources.ImportUnsupportedFormatVersion,
            MapImportError.NoMaps => Resources.ImportNoMaps,
            MapImportError.NoKinds => Resources.ImportNoKinds,
            MapImportError.MapNotFound => Resources.ImportMapNotFound,
            _ => Resources.ImportInvalidJson,
        };
    }
}
