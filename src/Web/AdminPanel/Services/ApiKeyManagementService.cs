// <copyright file="ApiKeyManagementService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared;
using MUnique.OpenMU.Web.Shared.Components.Form.Modal;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// Manages the API keys with which external applications use the public API.
/// </summary>
public class ApiKeyManagementService
{
    private readonly IApiKeyRepository _repository;
    private readonly IModalService _modalService;
    private readonly IToastService _toastService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyManagementService"/> class.
    /// </summary>
    /// <param name="repository">The repository of the stored keys.</param>
    /// <param name="modalService">The modal service.</param>
    /// <param name="toastService">The toast service.</param>
    public ApiKeyManagementService(IApiKeyRepository repository, IModalService modalService, IToastService toastService)
    {
        this._repository = repository;
        this._modalService = modalService;
        this._toastService = toastService;
    }

    /// <summary>
    /// Gets all stored keys.
    /// </summary>
    /// <returns>All stored keys.</returns>
    public async Task<IList<ApiKey>> GetKeysAsync()
    {
        return await this._repository.GetAllAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new key, asking for its name and role in a modal dialog.
    /// </summary>
    /// <returns>
    /// The generated key in plain text, so it can be shown to the user exactly once;
    /// <c>null</c>, if no key has been created.
    /// </returns>
    public async Task<string?> CreateNewInModalDialogAsync()
    {
        var input = new ApiKeyCreationParameters();
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<ApiKeyCreationParameters>.Item), input);
        var modal = this._modalService.Show<ModalCreateNew<ApiKeyCreationParameters>>(Resources.CreateApiKey, parameters, new ModalOptions { DisableBackgroundCancel = true });
        var result = await modal.Result.ConfigureAwait(false);
        if (result.Cancelled)
        {
            return null;
        }

        var generatedKey = ApiKeyGenerator.GenerateKey();
        var apiKey = new ApiKey
        {
            Id = Guid.NewGuid(),
            Name = input.Name,
            KeyHash = ApiKeyGenerator.Hash(generatedKey),
            KeyPrefix = ApiKeyGenerator.GetVisiblePrefix(generatedKey),
            Roles = input.Role.ToString(),
        };

        try
        {
            await this._repository.AddAsync(apiKey).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._toastService.ShowError(ex.Message);
            return null;
        }

        this._toastService.ShowSuccess(Resources.ApiKeyCreated);
        return generatedKey;
    }

    /// <summary>
    /// Enables or disables the specified key.
    /// </summary>
    /// <param name="apiKey">The key.</param>
    /// <param name="isDisabled">If set to <c>true</c>, the key is rejected from now on.</param>
    public async Task SetDisabledAsync(ApiKey apiKey, bool isDisabled)
    {
        apiKey.IsDisabled = isDisabled;
        try
        {
            await this._repository.UpdateAsync(apiKey).ConfigureAwait(false);
            this._toastService.ShowSuccess(isDisabled ? Resources.ApiKeyDisabled : Resources.ApiKeyEnabled);
        }
        catch (Exception ex)
        {
            this._toastService.ShowError(ex.Message);
        }
    }

    /// <summary>
    /// Deletes the specified key, after asking for a confirmation.
    /// </summary>
    /// <param name="apiKey">The key.</param>
    /// <returns><c>true</c>, if the key has been deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteAsync(ApiKey apiKey)
    {
        var confirmed = await this._modalService
            .ShowQuestionAsync(Resources.DeleteApiKey, string.Format(Resources.DeleteApiKeyQuestion, apiKey.Name))
            .ConfigureAwait(false);
        if (!confirmed)
        {
            return false;
        }

        try
        {
            await this._repository.DeleteAsync(apiKey).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._toastService.ShowError(ex.Message);
            return false;
        }

        this._toastService.ShowSuccess(Resources.ApiKeyDeleted);
        return true;
    }

    /// <summary>
    /// The parameters to create a new API key.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used by data binding.")]
    private class ApiKeyCreationParameters
    {
        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.ApiKeyName))]
        [MaxLength(100)]
        [MinLength(3)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.Role))]
        [Required]
        public AdminRole Role { get; set; } = AdminRole.Viewer;
    }
}
