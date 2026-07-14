// <copyright file="CreationPanelService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.Web.Shared.Properties;

/// <summary>
/// A circuit-scoped service which holds the currently active <see cref="CreationSession"/>.
/// The creation panel (rendered in the layout) subscribes to it and renders the form, so that
/// the form survives page navigation and can be collapsed while the user looks up other data.
/// Only one creation can be active at a time; starting another one asks the user to discard the current one.
/// </summary>
public sealed class CreationPanelService : IDisposable
{
    private readonly IModalService _modalService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreationPanelService"/> class.
    /// </summary>
    /// <param name="modalService">The modal service, used for the discard confirmation.</param>
    public CreationPanelService(IModalService modalService)
    {
        this._modalService = modalService;
    }

    /// <summary>
    /// Occurs when the active session or the collapse state changed, so the panel can re-render.
    /// </summary>
    public event Action? StateChanged;

    /// <summary>
    /// Occurs after an object of the given type has been created and saved, so views can refresh.
    /// </summary>
    public event Func<Type, Task>? ItemCreated;

    /// <summary>
    /// Gets the currently active creation session, or <see langword="null"/> if none is active.
    /// </summary>
    public CreationSession? Current { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the panel is currently collapsed.
    /// </summary>
    public bool IsCollapsed { get; private set; }

    /// <summary>
    /// Starts a new creation session. If one is already active, the user is asked to discard it first.
    /// </summary>
    /// <param name="session">The session to start.</param>
    /// <returns><see langword="true"/> if the session was started; <see langword="false"/> if the user declined.</returns>
    public async Task<bool> BeginAsync(CreationSession session)
    {
        if (this.Current is { } existing)
        {
            var caption = existing.ItemType.GetTypeCaption();
            var confirmed = await this._modalService
                .ShowQuestionAsync(Resources.DiscardEntryTitle, string.Format(Resources.DiscardEntryQuestion, caption))
                .ConfigureAwait(false);
            if (!confirmed)
            {
                return false;
            }

            await this.RunCleanupAsync(existing).ConfigureAwait(false);
        }

        this.Current = session;
        this.IsCollapsed = false;
        this.NotifyStateChanged();
        return true;
    }

    /// <summary>
    /// Submits the active session: it persists the object and ends the session.
    /// If saving throws, the session stays active so the user can correct the input and retry.
    /// </summary>
    public async Task CompleteAsync()
    {
        if (this.Current is not { } session)
        {
            return;
        }

        await session.SaveAsync().ConfigureAwait(false);

        this.Current = null;
        this.NotifyStateChanged();

        if (session.OwnsContext)
        {
            this.SafeDispose(session);
        }

        if (this.ItemCreated is { } handler)
        {
            foreach (Func<Type, Task> subscriber in handler.GetInvocationList())
            {
                await subscriber(session.ItemType).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Cancels the active session and discards the created object.
    /// </summary>
    public async Task CancelAsync()
    {
        if (this.Current is not { } session)
        {
            return;
        }

        this.Current = null;
        this.NotifyStateChanged();
        await this.RunCleanupAsync(session).ConfigureAwait(false);
    }

    /// <summary>
    /// Toggles the collapsed state of the panel.
    /// </summary>
    public void ToggleCollapse()
    {
        this.IsCollapsed = !this.IsCollapsed;
        this.NotifyStateChanged();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Dispose a still-pending, self-owned context so it doesn't leak when the circuit ends.
        if (this.Current is { OwnsContext: true } session)
        {
            this.SafeDispose(session);
        }

        this.Current = null;
    }

    private async Task RunCleanupAsync(CreationSession session)
    {
        try
        {
            await session.CancelAsync().ConfigureAwait(false);
        }
        catch
        {
            // The context may already be disposed (e.g. the page navigated away). We can ignore that.
        }
        finally
        {
            if (session.OwnsContext)
            {
                this.SafeDispose(session);
            }
        }
    }

    private void SafeDispose(CreationSession session)
    {
        try
        {
            session.Context.Dispose();
        }
        catch
        {
            // Nothing we can do about it, and we should not throw during cleanup.
        }
    }

    private void NotifyStateChanged() => this.StateChanged?.Invoke();
}
