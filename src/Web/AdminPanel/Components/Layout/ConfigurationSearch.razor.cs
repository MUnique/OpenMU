// <copyright file="ConfigurationSearch.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Layout;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Header search for configuration properties.
/// </summary>
public partial class ConfigurationSearch : IDisposable
{
    private const int MinimumSearchLength = 2;
    private const int MaximumResults = 15;

    private readonly Debouncer _searchDebouncer = new(200);
    private readonly List<ConfigurationSearchEntry> _searchResults = new();

    private bool _isLoading;
    private string _searchText = string.Empty;
    private IReadOnlyList<ConfigurationSearchEntry> _searchEntries = Array.Empty<ConfigurationSearchEntry>();

    [Inject]
    private ConfigurationSearchIndexCache SearchIndexCache { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private NavigationHistory NavigationHistory { get; set; } = null!;

    [Inject]
    private SetupService SetupService { get; set; } = null!;

    /// <inheritdoc />
    public void Dispose()
    {
        this.SetupService.DatabaseInitialized -= this.OnDatabaseInitializedAsync;
        this._searchDebouncer.Dispose();
    }

    /// <inheritdoc />
    protected override Task OnInitializedAsync()
    {
        this.SetupService.DatabaseInitialized += this.OnDatabaseInitializedAsync;
        if (this.SearchIndexCache.IsLoaded)
        {
            this._searchEntries = this.SearchIndexCache.Entries;
        }

        return base.OnInitializedAsync();
    }

    private static int CalculateScore(ConfigurationSearchEntry entry, string normalizedQuery, IReadOnlyList<string> queryParts)
    {
        if (queryParts.Count == 0 || !queryParts.All(part => entry.NormalizedHaystack.Contains(part, StringComparison.Ordinal)))
        {
            return int.MaxValue;
        }

        var score = 100;
        if (entry.NormalizedCaption.StartsWith(normalizedQuery, StringComparison.Ordinal))
        {
            score -= 60;
        }
        else if (entry.NormalizedCaption.Contains(normalizedQuery, StringComparison.Ordinal))
        {
            score -= 45;
        }
        else
        {
            // Caption does not contain the query, no score adjustment needed.
        }

        if (entry.NormalizedHaystack.StartsWith(normalizedQuery, StringComparison.Ordinal))
        {
            score -= 20;
        }

        score += entry.Path.Length / 64;
        return score;
    }

    private static string Normalize(string value)
    {
        return string.Join(
                ' ',
                value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToUpperInvariant();
    }

    private async Task OnSearchFocusAsync(FocusEventArgs _)
    {
        await this.EnsureIndexLoadedAsync().ConfigureAwait(true);
        this.UpdateSearchResults();
    }

    private Task OnSearchInputAsync()
    {

        _ = this._searchDebouncer.DebounceAsync(async token =>
        {
            if (this._searchEntries.Count == 0)
            {
                await this.EnsureIndexLoadedAsync().ConfigureAwait(false);
            }

            if (!token.IsCancellationRequested)
            {
                await this.InvokeAsync(() =>
                {
                    this.UpdateSearchResults();
                    this.StateHasChanged();
                }).ConfigureAwait(false);
            }
        });

        return Task.CompletedTask;
    }

    private async Task OnSearchBlurAsync(FocusEventArgs _)
    {
        await Task.Delay(100).ConfigureAwait(true);
        this._searchResults.Clear();
    }

    private Task OnSearchKeyDownAsync(KeyboardEventArgs args)
    {
        if (string.Equals(args.Key, "Escape", StringComparison.Ordinal))
        {
            this._searchText = string.Empty;
            this._searchResults.Clear();
        }
        else if (string.Equals(args.Key, "Enter", StringComparison.Ordinal)
                 && this._searchResults.FirstOrDefault() is { } firstResult)
        {
            this.NavigateToResult(firstResult);
        }
        else
        {
            // Other keys are not handled.
        }

        return Task.CompletedTask;
    }

    private async ValueTask OnDatabaseInitializedAsync()
    {
        this.SearchIndexCache.Invalidate();
        this._searchEntries = Array.Empty<ConfigurationSearchEntry>();
        this._searchResults.Clear();
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }

    private void UpdateSearchResults()
    {
        this._searchResults.Clear();
        if (this._searchEntries.Count == 0)
        {
            return;
        }

        var normalizedQuery = Normalize(this._searchText);
        if (normalizedQuery.Length < MinimumSearchLength)
        {
            return;
        }

        var queryParts = normalizedQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var results = this._searchEntries
            .Select(entry => (Entry: entry, Score: CalculateScore(entry, normalizedQuery, queryParts)))
            .Where(result => result.Score < int.MaxValue)
            .OrderBy(result => result.Score)
            .ThenBy(result => result.Entry.Path, StringComparer.Ordinal)
            .Take(MaximumResults)
            .Select(result => result.Entry);

        this._searchResults.AddRange(results);
    }

    private void NavigateToResult(ConfigurationSearchEntry entry)
    {
        this._searchText = string.Empty;
        this._searchResults.Clear();
        this.NavigationHistory.Clear();
        this.NavigationManager.NavigateTo(entry.Url);
    }

    private async Task EnsureIndexLoadedAsync()
    {
        if (this._searchEntries.Count > 0 || this._isLoading)
        {
            return;
        }

        this._isLoading = true;
        try
        {
            await this.SearchIndexCache.EnsureLoadedAsync().ConfigureAwait(true);
            this._searchEntries = this.SearchIndexCache.Entries;
        }
        finally
        {
            this._isLoading = false;
        }
    }
}
