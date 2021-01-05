// <copyright file="EditMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Pages
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Blazored.Modal.Services;
    using log4net;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Rendering;
    using MUnique.OpenMU.AdminPanel.Components;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// A page, which shows an <see cref="MapEditor"/> for the given <see cref="Id"/> of a <see cref="GameMapDefinition"/>.
    /// </summary>
    [Route("/map-editor/{id:guid}")]
    public sealed class EditMap : ComponentBase, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private object? model;
        private IContext? persistenceContext;
        private CancellationTokenSource? disposeCts;

        /// <summary>
        /// Gets or sets the identifier of the object which should be edited.
        /// </summary>
        [Parameter]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the persistence context provider which loads and saves the object.
        /// </summary>
        [Inject]
        public IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

        /// <summary>
        /// Gets or sets the modal service.
        /// </summary>
        [Inject]
        public IModalService ModalService { get; set; } = null!;

        /// <inheritdoc />
        public void Dispose()
        {
            this.disposeCts?.Cancel();
            this.disposeCts?.Dispose();
            this.disposeCts = null;
            if (this.persistenceContext is IDisposable disposable)
            {
                disposable.Dispose();
                this.persistenceContext = null;
            }
        }

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (this.model is { })
            {
                builder.OpenComponent<CascadingValue<IContext>>(1);
                builder.AddAttribute(2, nameof(CascadingValue<IContext>.Value), this.persistenceContext);
                builder.AddAttribute(3, nameof(CascadingValue<IContext>.IsFixed), true);
                builder.AddAttribute(4, nameof(CascadingValue<IContext>.ChildContent), (RenderFragment)(builder2 =>
                {
                    builder2.OpenComponent(5, typeof(MapEditor));
                    builder2.AddAttribute(6, nameof(MapEditor.Map), this.model);
                    builder2.AddAttribute(7, nameof(MapEditor.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChanges));
                    builder2.CloseComponent();
                }));

                builder.CloseComponent();
            }
        }

        /// <inheritdoc />
        protected override Task OnParametersSetAsync()
        {
            this.disposeCts?.Cancel();
            this.disposeCts?.Dispose();
            var cts = new CancellationTokenSource();
            this.disposeCts = cts;
            this.model = null;
            Task.Run(() => this.LoadData(cts.Token), cts.Token);
            return base.OnParametersSetAsync();
        }

        private async Task LoadData(CancellationToken cancellationToken)
        {
            IDisposable? modal = null;
            var showModalTask = this.InvokeAsync(() => modal = this.ModalService.ShowLoadingIndicator());

            this.persistenceContext = this.PersistenceContextProvider.CreateNewTypedContext<GameMapDefinition>();
            try
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        this.model = this.persistenceContext.GetById<GameMapDefinition>(this.Id);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Could not load game map with {this.Id}: {ex.Message}{Environment.NewLine}{ex.StackTrace}", ex);
                        await this.ModalService.ShowMessageAsync("Error", "Could not load the map data. Check the logs for details.");
                    }

                    await showModalTask;
                    modal?.Dispose();
                    await this.InvokeAsync(this.StateHasChanged);
                }
            }
            catch (TargetInvocationException ex) when (ex.InnerException is ObjectDisposedException)
            {
            }
            catch (ObjectDisposedException)
            {
                // Happens when the user navigated away (shouldn't happen with the modal loading indicator, but we check it anyway).
                // It would be great to have an async api with cancellation token support in the persistence layer
                // For the moment, we swallow the exception
            }
        }

        private Task SaveChanges()
        {
            string text;
            try
            {
                text = this.persistenceContext?.SaveChanges() ?? false ? "The changes have been saved." : "There were no changes to save.";
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(this.GetType()).Error($"Error during saving {this.Id}", ex);
                text = $"An unexpected error occured: {ex.Message}.";
            }

            return this.ModalService.ShowMessageAsync("Save", text);
        }
    }
}
