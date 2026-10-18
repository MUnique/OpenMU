// <copyright file="ModalService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Web.Shared.Components.Modal;

/// <summary>
/// Default implementation of <see cref="IModalService"/>.
/// </summary>
public sealed class ModalService : IModalService, IDisposable
{
    private readonly List<ModalState> _states = new();

    /// <summary>
    /// Occurs when the modal state changes (shown, closed).
    /// </summary>
    public event Action? StateChanged;

    /// <summary>
    /// Gets the current (topmost) modal state, or <see langword="null"/> if none is active.
    /// </summary>
    internal ModalState? Current => this._states.Count > 0 ? this._states[^1] : null;

    /// <summary>
    /// Gets a snapshot of the active modal states, from bottom to top.
    /// </summary>
    /// <remarks>
    /// Returns a copy so renderers never observe the live list while it is being modified.
    /// </remarks>
    internal IReadOnlyList<ModalState> States => this._states.ToArray();

    /// <inheritdoc />
    public IModalReference Show<TComponent>(string title, ModalParameters? parameters = null, ModalOptions? options = null)
        where TComponent : class, IComponent
    {
        return this.Show(typeof(TComponent), title, parameters, options);
    }

    /// <inheritdoc />
    public IModalReference Show(Type componentType, string title, ModalParameters? parameters = null, ModalOptions? options = null)
    {
        var reference = new ModalReference();
        var instance = new ModalInstance(reference, this.Remove);
        var state = new ModalState(componentType, title, parameters, options, instance, reference);
        this._states.Add(state);
        this.StateChanged?.Invoke();
        return reference;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var state in this._states)
        {
            state.Reference.TrySetResult(ModalResult.Cancel());
        }

        this._states.Clear();
    }

    /// <summary>
    /// Dismisses the currently active (topmost) modal.
    /// </summary>
    internal void Dismiss()
    {
        if (this.Current is { } state)
        {
            this.Dismiss(state.Instance);
        }
    }

    /// <summary>
    /// Dismisses the modal with the specified instance.
    /// </summary>
    /// <param name="instance">The modal instance to dismiss.</param>
    internal void Dismiss(ModalInstance instance)
    {
        var state = this._states.FirstOrDefault(s => ReferenceEquals(s.Instance, instance));
        if (state is not null)
        {
            state.Reference.TrySetResult(ModalResult.Cancel());
            this.Remove(instance);
        }
    }

    /// <summary>
    /// Removes the modal with the specified instance from the stack.
    /// </summary>
    /// <remarks>
    /// The caller sets the <see cref="ModalReference"/> result before calling this,
    /// either via <see cref="ModalInstance.CloseAsync"/> (which passes its own result)
    /// or via <see cref="Dismiss(ModalInstance)"/> (which records a cancellation).
    /// </remarks>
    /// <param name="instance">The modal instance to remove.</param>
    internal void Remove(ModalInstance instance)
    {
        var index = this._states.FindIndex(s => ReferenceEquals(s.Instance, instance));
        if (index >= 0)
        {
            this._states.RemoveAt(index);
            this.StateChanged?.Invoke();
        }
    }

    /// <summary>
    /// Holds the full state of an active modal.
    /// </summary>
    internal sealed class ModalState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModalState"/> class.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <param name="title">The modal title.</param>
        /// <param name="parameters">The modal parameters.</param>
        /// <param name="options">The modal options.</param>
        /// <param name="instance">The modal instance.</param>
        /// <param name="reference">The modal reference.</param>
        internal ModalState(
            Type componentType,
            string title,
            ModalParameters? parameters,
            ModalOptions? options,
            ModalInstance instance,
            ModalReference reference)
        {
            this.ComponentType = componentType;
            this.Title = title;
            this.Parameters = parameters;
            this.Options = options;
            this.Instance = instance;
            this.Reference = reference;
        }

        /// <summary>
        /// Gets the component type to render.
        /// </summary>
        public Type ComponentType { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public ModalParameters? Parameters { get; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public ModalOptions? Options { get; }

        /// <summary>
        /// Gets the <see cref="ModalInstance"/> cascaded to the modal content.
        /// </summary>
        public ModalInstance Instance { get; }

        /// <summary>
        /// Gets the <see cref="ModalReference"/> that tracks the result.
        /// </summary>
        public ModalReference Reference { get; }
    }
}
