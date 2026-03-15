// <copyright file="NavigationHistory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using System;
using Microsoft.AspNetCore.Components;

/// <summary>
/// A service which keeps track of the navigation history to support breadcrumb navigation.
/// </summary>
public sealed class NavigationHistory
{
    private readonly Stack<HistoryEntry> _nextPages = new();
    private readonly Stack<HistoryEntry> _previousPages = new();
    private bool _isNavigating;
    private HistoryEntry? _current;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationHistory"/> class.
    /// </summary>
    /// <param name="navigationManager">The navigation history.</param>
    public NavigationHistory(NavigationManager navigationManager)
    {
        this.NavigationManager = navigationManager;
    }

    /// <summary>
    /// Gets the navigation manager.
    /// </summary>
    public NavigationManager NavigationManager { get; }

    /// <summary>
    /// Event which is fired whenever the history changed.
    /// </summary>
    public event EventHandler? HistoryChanged;

    /// <summary>
    /// Gets the previous page entries.
    /// </summary>
    public IEnumerable<HistoryEntry> Previous => this._previousPages.Reverse();

    /// <summary>
    /// Gets the current page entry.
    /// </summary>
    public HistoryEntry? Current => this._current;

    /// <summary>
    /// Gets the next page entries.
    /// </summary>
    public IEnumerable<HistoryEntry> Next => this._nextPages;

    /// <summary>
    /// Gets a value indicating whether navigating backward is possible.
    /// </summary>
    public bool CanGoBackward => this._previousPages.Count > 0;

    /// <summary>
    /// Gets a value indicating whether navigating forward is possible.
    /// </summary>
    public bool CanGoForward => this._nextPages.Count > 0;

    /// <summary>
    /// Clears the history, except for 'Home'.
    /// </summary>
    public void Clear()
    {
        this._nextPages.Clear();
        this._previousPages.Clear();
        this._current = null;
        this._current = new HistoryEntry(this.NavigationManager.BaseUri, "Home");
        this.HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Adds the current page to history with the specified caption.
    /// </summary>
    /// <param name="caption">The caption of the current page.</param>
    public void AddCurrentPageToHistory(string caption)
    {
        var currentUri = this.NavigationManager.Uri;
        if (this._isNavigating)
        {
            return;
        }

        if (this._current?.Uri == currentUri)
        {
            return;
        }

        if (this._nextPages.TryPeek(out var next) && next.Uri == currentUri)
        {
            this._nextPages.Pop();
        }
        else
        {
            this._nextPages.Clear();
        }

        if (this._current is not null)
        {
            if (this._previousPages.Any(p => p.Uri == currentUri))
            {
                this._nextPages.Push(this._current);
                while (this._previousPages.TryPop(out var p))
                {
                    if (p.Uri == currentUri)
                    {
                        break;
                    }

                    this._nextPages.Push(p);
                }
            }
            else
            {
                this._previousPages.Push(this._current);
            }
        }

        this._current = new HistoryEntry(currentUri, caption);
        this.HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Goes a step backward.
    /// </summary>
    public void GoBackward()
    {
        var previous = this._previousPages.Pop();
        if (this._current is not null)
        {
            this._nextPages.Push(this._current);
        }

        this._current = previous;

        this._isNavigating = true;
        try
        {
            this.NavigationManager.NavigateTo(previous.Uri);
        }
        finally
        {
            this._isNavigating = false;
        }
    }

    /// <summary>
    /// Goes a step forward.
    /// </summary>
    public void GoForward()
    {
        var next = this._nextPages.Pop();
        if (this._current is not null)
        {
            this._previousPages.Push(this._current);
        }

        this._current = next;

        this._isNavigating = true;
        try
        {
            this.NavigationManager.NavigateTo(next.Uri);
        }
        finally
        {
            this._isNavigating = false;
        }
    }

    /// <summary>
    /// Jumps to a specific entry of the history.
    /// </summary>
    /// <param name="entry">The history entry.</param>
    public void JumpTo(HistoryEntry entry)
    {
        if (this._current == entry)
        {
            return;
        }

        if (this._nextPages.Contains(entry))
        {
            while (this._nextPages.TryPop(out var nextEntry))
            {
                if (this._current is not null)
                {
                    this._previousPages.Push(this._current);
                }

                this._current = nextEntry;
                if (this._current == entry)
                {
                    break;
                }
            }
        }
        else if (this._previousPages.Contains(entry))
        {
            while (this._previousPages.TryPop(out var previousEntry))
            {
                if (this._current is not null)
                {
                    this._nextPages.Push(this._current);
                }

                this._current = previousEntry;
                if (this._current == entry)
                {
                    break;
                }
            }
        }
        else
        {
            // should never happen :)
            return;
        }

        this.HistoryChanged?.Invoke(this, EventArgs.Empty);
        if (this._current is not null)
        {
            this.NavigationManager.NavigateTo(this._current.Uri);
        }
    }

    /// <summary>
    /// An entry for a navigation history.
    /// </summary>
    public record HistoryEntry(string Uri, string Title);
}