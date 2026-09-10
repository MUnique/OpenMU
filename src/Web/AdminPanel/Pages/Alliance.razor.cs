// <copyright file="Alliance.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Web.Shared.Models;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Page which shows the guilds of an alliance.
/// </summary>
public partial class Alliance : ComponentBase
{
    private AllianceDetails? _alliance;

    private bool _isLoading = true;

    /// <summary>
    /// Gets or sets the persistent identifier of any guild in the alliance.
    /// </summary>
    [Parameter]
    public Guid GuildId { get; set; }

    /// <summary>
    /// Gets or sets the guild service.
    /// </summary>
    [Inject]
    public IGuildService GuildService { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync().ConfigureAwait(true);
        this._isLoading = true;
        this._alliance = null;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);

        this._alliance = await this.GuildService.GetAllianceAsync(this.GuildId).ConfigureAwait(true);

        this._isLoading = false;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
    }
}
