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
    private readonly AdminPanelSetupMode _setupMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyManagementService"/> class.
    /// </summary>
    /// <param name="repository">The repository of the stored keys.</param>
    /// <param name="modalService">The modal service.</param>
    /// <param name="toastService">The toast service.</param>
    /// <param name="setupMode">The setup mode, which knows whether the admin panel has a user.</param>
    public ApiKeyManagementService(IApiKeyRepository repository, IModalService modalService, IToastService toastService, AdminPanelSetupMode setupMode)
    {
        this._repository = repository;
        this._modalService = modalService;
        this._toastService = toastService;
        this._setupMode = setupMode;
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
    /// <remarks>
    /// No key is created while the admin panel has no user. In that initial setup mode, anybody
    /// who reaches the admin panel could create one, and the key would keep working after the
    /// first user exists. It's checked again right before the key is stored, because the dialog
    /// can stay open for a long time.
    /// </remarks>
    public async Task<string?> CreateNewInModalDialogAsync()
    {
        if (await this.RefuseInSetupModeAsync().ConfigureAwait(false))
        {
            return null;
        }

        var input = new ApiKeyCreationParameters();
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<ApiKeyCreationParameters>.Item), input);
        var modal = this._modalService.Show<ModalCreateNew<ApiKeyCreationParameters>>(Resources.CreateApiKey, parameters, new ModalOptions { DisableBackgroundCancel = true });
        var result = await modal.Result.ConfigureAwait(false);
        if (result.Cancelled)
        {
            return null;
        }

        var allowedToolGroups = input.AllowedToolGroups.Trim();
        if (input.Role == ApiKeyRole.WebManager)
        {
            // A website key can't do anything by its role, so its functions are selected right away.
            if (await this.SelectWebsitePermissionsAsync(string.Empty).ConfigureAwait(false) is not { } permissions)
            {
                return null;
            }

            allowedToolGroups = permissions;
        }
        else
        {
            if (!AdminToolGroups.IsValid(allowedToolGroups))
            {
                this._toastService.ShowError(ApiKeyToolGroupResources.Caption);
                return null;
            }
        }

        var generatedKey = ApiKeyGenerator.GenerateKey();
        var apiKey = new ApiKey
        {
            Id = Guid.NewGuid(),
            Name = input.Name,
            KeyHash = ApiKeyGenerator.Hash(generatedKey),
            KeyPrefix = ApiKeyGenerator.GetVisiblePrefix(generatedKey),
            Roles = input.Role.ToString(),
            AllowedToolGroups = allowedToolGroups,
        };

        try
        {
            if (await this.RefuseInSetupModeAsync().ConfigureAwait(false))
            {
                return null;
            }

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

    /// <summary>Edits an existing key's optional MCP allowlist without changing its secret or role.</summary>
    /// <param name="apiKey">The stored key.</param>
    /// <returns>Whether the restriction was saved.</returns>
    /// <remarks>
    /// The functions of a <see cref="AdminRoles.WebManager"/> key are selected with check boxes instead,
    /// because they are a fixed set and an empty list means that the key can't do anything.
    /// </remarks>
    public async ValueTask<bool> EditToolGroupsAsync(ApiKey apiKey)
    {
        if (string.Equals(apiKey.Roles, AdminRoles.WebManager, StringComparison.Ordinal))
        {
            return await this.SelectWebsitePermissionsAsync(apiKey.AllowedToolGroups).ConfigureAwait(false) is { } permissions
                && await this.SaveToolGroupsAsync(apiKey, permissions).ConfigureAwait(false);
        }

        var input = new ApiKeyToolGroupParameters { AllowedToolGroups = apiKey.AllowedToolGroups };
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<ApiKeyToolGroupParameters>.Item), input);
        var modal = this._modalService.Show<ModalCreateNew<ApiKeyToolGroupParameters>>(Resources.ApiKeys, parameters, new ModalOptions { DisableBackgroundCancel = true });
        var result = await modal.Result.ConfigureAwait(false);
        if (result.Cancelled)
        {
            return false;
        }

        if (!AdminToolGroups.IsValid(input.AllowedToolGroups))
        {
            this._toastService.ShowError(ApiKeyToolGroupResources.Caption);
            return false;
        }

        return await this.SaveToolGroupsAsync(apiKey, input.AllowedToolGroups.Trim()).ConfigureAwait(false);
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

    private async ValueTask<bool> SaveToolGroupsAsync(ApiKey apiKey, string allowedToolGroups)
    {
        var previous = apiKey.AllowedToolGroups;
        apiKey.AllowedToolGroups = allowedToolGroups;
        try
        {
            await this._repository.UpdateAsync(apiKey).ConfigureAwait(false);
            return true;
        }
        catch (Exception exception)
        {
            apiKey.AllowedToolGroups = previous;
            this._toastService.ShowError(exception.Message);
            return false;
        }
    }

    /// <summary>
    /// Asks for the functions of a website key with check boxes.
    /// </summary>
    /// <param name="allowedToolGroups">The currently allowed functions.</param>
    /// <returns>The selected functions as allowlist; <c>null</c>, if the dialog has been cancelled.</returns>
    private async ValueTask<string?> SelectWebsitePermissionsAsync(string allowedToolGroups)
    {
        var input = WebsitePermissionParameters.FromAllowlist(allowedToolGroups);
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<WebsitePermissionParameters>.Item), input);
        var modal = this._modalService.Show<ModalCreateNew<WebsitePermissionParameters>>(Resources.WebsitePermissions, parameters, new ModalOptions { DisableBackgroundCancel = true });
        var result = await modal.Result.ConfigureAwait(false);
        return result.Cancelled ? null : input.ToAllowlist();
    }

    /// <summary>
    /// Refuses to create a key while the admin panel has no user, and tells why.
    /// </summary>
    /// <returns><c>true</c>, if the admin panel has no user and no key may be created; otherwise, <c>false</c>.</returns>
    private async ValueTask<bool> RefuseInSetupModeAsync()
    {
        if (!await this._setupMode.IsActiveAsync().ConfigureAwait(false))
        {
            return false;
        }

        this._toastService.ShowError(Resources.ApiKeyNeedsAUser);
        return true;
    }

    /// <summary>
    /// The check boxes of the functions of a website key, see <see cref="WebsitePermissions"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used by data binding.")]
    private sealed class WebsitePermissionParameters
    {
        /// <summary>
        /// The functions with their check boxes, in the order in which they are stored.
        /// </summary>
        private static readonly (string Permission, Func<WebsitePermissionParameters, bool> IsSelected, Action<WebsitePermissionParameters> Select)[] Map =
        [
            (WebsitePermissions.Status, p => p.Status, p => p.Status = true),
            (WebsitePermissions.Register, p => p.Register, p => p.Register = true),
            (WebsitePermissions.Ranking, p => p.Ranking, p => p.Ranking = true),
            (WebsitePermissions.MiniGameRanking, p => p.MiniGameRanking, p => p.MiniGameRanking = true),
            (WebsitePermissions.Events, p => p.Events, p => p.Events = true),
            (WebsitePermissions.Character, p => p.Character, p => p.Character = true),
            (WebsitePermissions.Login, p => p.Login, p => p.Login = true),
            (WebsitePermissions.ChangePassword, p => p.ChangePassword, p => p.ChangePassword = true),
            (WebsitePermissions.CashShopGrant, p => p.CashShopGrant, p => p.CashShopGrant = true),
        ];

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.WebsitePermissionStatus))]
        public bool Status { get; set; }

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.WebsitePermissionRegister))]
        public bool Register { get; set; }

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.WebsitePermissionRanking))]
        public bool Ranking { get; set; }

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.WebsitePermissionMiniGameRanking))]
        public bool MiniGameRanking { get; set; }

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.WebsitePermissionEvents))]
        public bool Events { get; set; }

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.WebsitePermissionCharacter))]
        public bool Character { get; set; }

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.WebsitePermissionLogin))]
        public bool Login { get; set; }

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.WebsitePermissionChangePassword))]
        public bool ChangePassword { get; set; }

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.WebsitePermissionCashShopGrant))]
        public bool CashShopGrant { get; set; }

        /// <summary>
        /// Creates the check boxes for the functions of an allowlist.
        /// </summary>
        /// <param name="allowlist">The allowlist of the key.</param>
        /// <returns>The check boxes.</returns>
        public static WebsitePermissionParameters FromAllowlist(string allowlist)
        {
            var result = new WebsitePermissionParameters();
            foreach (var (permission, _, select) in Map)
            {
                if (AdminToolGroups.AllowsExplicitly(allowlist, permission))
                {
                    select(result);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the allowlist of the selected functions.
        /// </summary>
        /// <returns>The comma separated names of the selected functions.</returns>
        public string ToAllowlist() => string.Join(',', Map.Where(entry => entry.IsSelected(this)).Select(entry => entry.Permission));
    }

    private sealed class ApiKeyToolGroupParameters
    {
        [Display(ResourceType = typeof(ApiKeyToolGroupResources), Name = nameof(ApiKeyToolGroupResources.Caption))]
        [MaxLength(1000)]
        public string AllowedToolGroups { get; set; } = string.Empty;
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

        [Display(ResourceType = typeof(ApiKeyToolGroupResources), Name = nameof(ApiKeyToolGroupResources.Caption))]
        [MaxLength(1000)]
        public string AllowedToolGroups { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.Role))]
        [Required]
        public ApiKeyRole Role { get; set; } = ApiKeyRole.Viewer;
    }
}
