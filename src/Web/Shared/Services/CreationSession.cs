// <copyright file="CreationSession.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using MUnique.OpenMU.Persistence;

/// <summary>
/// Describes a single "create new object" interaction which is rendered in the
/// persistent creation panel instead of a modal dialog.
/// </summary>
public sealed class CreationSession
{
    /// <summary>
    /// Gets the title which is shown at the top of the panel.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the object which is being created.
    /// </summary>
    public required object Item { get; init; }

    /// <summary>
    /// Gets the type of the object which is being created.
    /// </summary>
    public required Type ItemType { get; init; }

    /// <summary>
    /// Gets the persistence context which is used to create and save the object.
    /// </summary>
    public required IContext Context { get; init; }

    /// <summary>
    /// Gets the owner of the collection the item is created for, if any.
    /// It's used to provide context for the creation (e.g. filtering skills by character class).
    /// </summary>
    public object? Owner { get; init; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Context"/> is owned by this session
    /// and should be disposed when the session ends. This is the case for top-level objects
    /// whose context is independent from any page.
    /// </summary>
    public bool OwnsContext { get; init; }

    /// <summary>
    /// Gets the action which persists the created object. It's invoked when the user submits the form.
    /// </summary>
    public Func<Task> SaveAsync { get; init; } = () => Task.CompletedTask;

    /// <summary>
    /// Gets the action which discards the created object. It's invoked when the session is cancelled or replaced.
    /// </summary>
    public Func<Task> CancelAsync { get; init; } = () => Task.CompletedTask;
}
