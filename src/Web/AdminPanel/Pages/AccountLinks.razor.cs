// <copyright file="AccountLinks.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// The page which shows and removes the links of an account to users of external services, e.g. Discord.
/// </summary>
public partial class AccountLinks : ComponentBase
{
    private IReadOnlyList<AccountExternalLink>? _links;
    private Guid _accountId;
    private GameConfiguration? _gameConfiguration;
    private bool _isLoading = true;
    private bool _isRemoving;

    /// <summary>
    /// Gets or sets the login name of the account.
    /// </summary>
    [Parameter]
    public string LoginName { get; set; } = string.Empty;

    [Inject]
    private IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    [Inject]
    private IDataSource<GameConfiguration> GameConfigurationSource { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private ILogger<AccountLinks> Logger { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await this.LoadAsync().ConfigureAwait(true);
        await base.OnParametersSetAsync().ConfigureAwait(true);
    }

    private IPlayerContext CreateContext() => this.PersistenceContextProvider.CreateNewPlayerContext(this._gameConfiguration!);

    private async Task LoadAsync()
    {
        this._isLoading = true;
        try
        {
            this._gameConfiguration ??= await this.GameConfigurationSource.GetOwnerAsync(Guid.Empty).ConfigureAwait(true);
            using var context = this.CreateContext();
            if (await context.GetAccountByLoginNameAsync(this.LoginName).ConfigureAwait(true) is { } account)
            {
                this._accountId = account.GetId();
                this._links = await context.GetAccountExternalLinksAsync(this._accountId).ConfigureAwait(true);
            }
            else
            {
                this._links = null;
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading the links of account {LoginName}.", this.LoginName);
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
        finally
        {
            this._isLoading = false;
        }
    }

    private async Task OnRemoveAsync(string provider)
    {
        this._isRemoving = true;
        try
        {
            await new AccountLinkService(this.CreateContext).UnlinkAccountAsync(this._accountId, provider).ConfigureAwait(true);
            this.Logger.LogInformation("The {Provider} link of account {LoginName} was removed.", provider, this.LoginName);
            this.ToastService.ShowSuccess(Resources.ExternalLinkRemoved);
            await this.LoadAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error removing the {Provider} link of account {LoginName}.", provider, this.LoginName);
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
        finally
        {
            this._isRemoving = false;
        }
    }
}
