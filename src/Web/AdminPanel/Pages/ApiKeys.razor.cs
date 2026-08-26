// <copyright file="ApiKeys.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// The page which manages the API keys of the public API.
/// </summary>
public partial class ApiKeys
{
    private IList<ApiKey> _apiKeys = new List<ApiKey>();
    private bool _isLoading = true;

    /// <summary>
    /// The key which has just been created. It's the only moment at which it's available, because
    /// only its hash is stored.
    /// </summary>
    private string? _createdKey;

    [Inject]
    private ApiKeyManagementService ApiKeyManagementService { get; set; } = null!;

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

    private async Task OnRoleChangedAsync(ApiKey apiKey, string? role)
    {
        if (string.IsNullOrEmpty(role) || role == apiKey.Roles)
        {
            return;
        }

        await this.ApiKeyManagementService.SetRoleAsync(apiKey, role).ConfigureAwait(true);
        await this.ReloadAsync().ConfigureAwait(true);
    }
}
