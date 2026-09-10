// <copyright file="Guild.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Web.Shared.Models;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Page which shows the details of a single guild, including its members.
/// </summary>
public partial class Guild : ComponentBase
{
    private GuildListItem? _guild;

    private IReadOnlyList<GuildMemberViewItem> _members = [];

    private bool _isLoading = true;

    /// <summary>
    /// Gets or sets the persistent identifier of the guild.
    /// </summary>
    [Parameter]
    public Guid GuildId { get; set; }

    /// <summary>
    /// Gets or sets the guild service.
    /// </summary>
    [Inject]
    public IGuildService GuildService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the loading overlay service.
    /// </summary>
    [Inject]
    public LoadingOverlayService LoadingService { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync().ConfigureAwait(true);
        using var loading = this.LoadingService.ShowLoadingIndicator();
        this._isLoading = true;
        this._guild = null;
        this._members = [];
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);

        this._guild = await this.GuildService.GetGuildAsync(this.GuildId).ConfigureAwait(true);
        if (this._guild is not null)
        {
            this._members = await this.GuildService.GetGuildMembersAsync(this.GuildId).ConfigureAwait(true);
        }

        this._isLoading = false;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
    }
}
