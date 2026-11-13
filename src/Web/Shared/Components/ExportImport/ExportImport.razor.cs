// <copyright file="ExportImport.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.ExportImport;

using System.IO;
using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Two buttons which export data as a JSON file and import a JSON file, e.g. the monster spawns of a map.
/// </summary>
/// <remarks>
/// The component handles the file selection and the download of the file; what the file contains
/// is up to the parent, so that the same component can be used for different kinds of data.
/// </remarks>
public partial class ExportImport : IAsyncDisposable
{
    private static readonly string JsModulePath =
        $"./_content/{typeof(ExportImport).Assembly.GetName().Name}/Components/ExportImport/{nameof(ExportImport)}.razor.js";

    private readonly string _fileInputId = $"import-file-input-{Guid.NewGuid():N}";

    private IJSObjectReference? _jsModule;
    private bool _isExporting;
    private bool _isImporting;

    /// <summary>
    /// Gets or sets the caption and tooltip of the export button.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string ExportText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the caption and tooltip of the import button.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string ImportText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the exported file.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string ExportFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the function which builds the content of the exported file.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public Func<string> ExportContentProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the callback which handles the content of the imported file.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public EventCallback<string> OnImport { get; set; }

    /// <summary>
    /// Gets or sets the java script runtime.
    /// </summary>
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// Gets or sets the loading overlay service.
    /// </summary>
    [Inject]
    private LoadingOverlayService LoadingOverlay { get; set; } = null!;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (this._jsModule is null)
        {
            return;
        }

        try
        {
            await this._jsModule.DisposeAsync().ConfigureAwait(false);
        }
        catch (JSDisconnectedException)
        {
            // The circuit is already gone, so the JavaScript module is gone with it.
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this._jsModule = await this.JsRuntime
                .InvokeAsync<IJSObjectReference>("import", JsModulePath)
                .ConfigureAwait(true);
        }
    }

    private async Task ExportAsync()
    {
        if (this._jsModule is not { } jsModule)
        {
            return;
        }

        this._isExporting = true;
        using var loading = this.LoadingOverlay.ShowLoadingIndicator();
        try
        {
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);

            var content = this.ExportContentProvider();
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
            await jsModule.InvokeVoidAsync("downloadFile", this.ExportFileName, base64).ConfigureAwait(true);
        }
        finally
        {
            this._isExporting = false;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        }
    }

    private async Task TriggerFileImportAsync()
    {
        if (this._jsModule is not null)
        {
            await this._jsModule.InvokeVoidAsync("triggerFileInput", this._fileInputId).ConfigureAwait(true);
        }
    }

    private async Task ImportFileSelectedAsync(IFileListEntry[] files)
    {
        if (files.FirstOrDefault() is not { } file)
        {
            return;
        }

        this._isImporting = true;
        using var loading = this.LoadingOverlay.ShowLoadingIndicator();
        try
        {
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);

            using var reader = new StreamReader(file.Data);
            var content = await reader.ReadToEndAsync().ConfigureAwait(true);
            await this.OnImport.InvokeAsync(content).ConfigureAwait(true);
        }
        finally
        {
            await this.ResetFileInputAsync().ConfigureAwait(true);
            this._isImporting = false;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        }
    }

    private async Task ResetFileInputAsync()
    {
        if (this._jsModule is not { } jsModule)
        {
            return;
        }

        try
        {
            await jsModule.InvokeVoidAsync("resetFileInput", this._fileInputId).ConfigureAwait(true);
        }
        catch (JSDisconnectedException)
        {
            // The circuit is already gone, so there is no file input anymore.
        }
    }
}
