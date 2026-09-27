// <copyright file="ModalService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Web.Shared.Components.Modal;

/// <summary>
/// Default implementation of <see cref="IModalService"/>.
/// </summary>
/// <remarks>
/// Modals are stacked: a modal which is shown from within another one (e.g. the selection of
/// objects for a field of an object which is being created) is shown on top of it, and the
/// underlying modal keeps its state until the top one is closed.
/// </remarks>
public sealed class ModalService : IModalService, IDisposable
{
    private readonly List<ModalState> _stack = new();

    /// <summary>
    /// Occurs when the modal state changes (shown, closed).
    /// </summary>
    public event Action? StateChanged;

    /// <summary>
    /// Gets the open modals, from the bottom to the top.
    /// </summary>
    internal IReadOnlyList<ModalState> Modals => this._stack;

    /// <summary>
    /// Gets the modal on top, or <see langword="null"/> if none is active.
    /// </summary>
    internal ModalState? Current => this._stack.Count > 0 ? this._stack[^1] : null;

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
        ModalState? state = null;

        // The instance closes exactly its own modal, even if another one has been opened on top of it meanwhile.
        var instance = new ModalInstance(reference, () => this.Close(state!));
        state = new ModalState(componentType, title, parameters, options, instance, reference);
        this._stack.Add(state);
        this.StateChanged?.Invoke();
        return reference;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var state in this._stack)
        {
            state.Reference.TrySetResult(ModalResult.Cancel());
        }

        this._stack.Clear();
    }

    /// <summary>
    /// Dismisses the modal on top.
    /// </summary>
    internal void Dismiss()
    {
        if (this.Current is { } state)
        {
            this.Close(state);
        }
    }

    /// <summary>
    /// Closes the specified modal. If it's already closed with a result, it stays like that.
    /// </summary>
    /// <param name="state">The modal.</param>
    private void Close(ModalState state)
    {
        state.Reference.TrySetResult(ModalResult.Cancel());
        if (this._stack.Remove(state))
        {
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
