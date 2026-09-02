// <copyright file="ApiKeys.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// The page which manages the API keys of the public API.
/// </summary>
public partial class ApiKeys : IAsyncDisposable
{
    private const string ClipboardScriptPath = "./_content/MUnique.OpenMU.Web.AdminPanel/js/clipboard.js";

    private IList<ApiKey> _apiKeys = new List<ApiKey>();
    private bool _isLoading = true;
    private IJSObjectReference? _clipboardModule;

    /// <summary>
    /// The key which has just been created. It's the only moment at which it's available, because
    /// only its hash is stored.
    /// </summary>
    private string? _createdKey;

    [Inject]
    private ApiKeyManagementService ApiKeyManagementService { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (this._clipboardModule is { } module)
        {
            this._clipboardModule = null;
            try
            {
                await module.DisposeAsync().ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
                // The circuit is already gone - nothing to clean up on the client anymore.
            }
        }

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        await this.ReloadAsync().ConfigureAwait(true);
    }

    private async Task ReloadAsync()
    {
        this._isLoading = true;
        try
        {
            this._apiKeys = await this.ApiKeyManagementService.GetKeysAsync().ConfigureAwait(true);
        }
        finally
        {
            this._isLoading = false;
        }
    }

    private async Task OnCreateNewAsync()
    {
        var createdKey = await this.ApiKeyManagementService.CreateNewInModalDialogAsync().ConfigureAwait(true);
        if (createdKey is null)
        {
            return;
        }

        this._createdKey = createdKey;
        await this.ReloadAsync().ConfigureAwait(true);
    }

    private async Task OnSetDisabledAsync(ApiKey apiKey, bool isDisabled)
    {
        await this.ApiKeyManagementService.SetDisabledAsync(apiKey, isDisabled).ConfigureAwait(true);
        await this.ReloadAsync().ConfigureAwait(true);
    }

    private async Task OnDeleteAsync(ApiKey apiKey)
    {
        if (await this.ApiKeyManagementService.DeleteAsync(apiKey).ConfigureAwait(true))
        {
            await this.ReloadAsync().ConfigureAwait(true);
        }
    }

    private async Task CopyCreatedKeyAsync()
    {
        if (this._createdKey is not { Length: > 0 } createdKey)
        {
            return;
        }

        var module = await this.GetClipboardModuleAsync().ConfigureAwait(true);
        var isCopied = await module.InvokeAsync<bool>("copyText", createdKey).ConfigureAwait(true);
        if (isCopied)
        {
            this.ToastService.ShowSuccess(Resources.CopiedToClipboard);
        }
        else
        {
            // Copying is refused without a secure context, so the user selects it instead.
            this.ToastService.ShowError(Resources.CopyToClipboardFailed);
        }
    }

    private async ValueTask<IJSObjectReference> GetClipboardModuleAsync()
    {
        return this._clipboardModule ??= await this.JsRuntime
            .InvokeAsync<IJSObjectReference>("import", ClipboardScriptPath)
            .ConfigureAwait(true);
    }
}
