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
    private ModalState? _state;

    /// <summary>
    /// Occurs when the modal state changes (shown, closed).
    /// </summary>
    public event Action? StateChanged;

    /// <summary>
    /// Gets the current modal state, or <see langword="null"/> if none is active.
    /// </summary>
    internal ModalState? Current => this._state;

    /// <inheritdoc />
    public IModalReference Show<TComponent>(string title, ModalParameters? parameters = null, ModalOptions? options = null)
        where TComponent : class, IComponent
    {
        return this.Show(typeof(TComponent), title, parameters, options);
    }

    /// <inheritdoc />
    public IModalReference Show(Type componentType, string title, ModalParameters? parameters = null, ModalOptions? options = null)
    {
        if (this._state is { } state)
        {
            state.Reference.TrySetResult(ModalResult.Cancel());
        }

        var reference = new ModalReference();
        var instance = new ModalInstance(reference, this.Dismiss);
        this._state = new ModalState(componentType, title, parameters, options, instance, reference);
        this.StateChanged?.Invoke();
        return reference;
    }

    /// <summary>
    /// Dismisses the currently active modal.
    /// </summary>
    internal void Dismiss()
    {
        if (this._state is { } state)
        {
            state.Reference.TrySetResult(ModalResult.Cancel());
        }

        this._state = null;
        this.StateChanged?.Invoke();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._state is { } state)
        {
            state.Reference.TrySetResult(ModalResult.Cancel());
        }

        this._state = null;
    }

    /// <summary>
    /// Holds the full state of an active modal.
    /// </summary>
    internal sealed class ModalState
    {
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
