// <copyright file="Edit.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Pages
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Blazored.Modal.Services;
    using log4net;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Rendering;
    using MUnique.OpenMU.AdminPanel.Components.Form;
    using MUnique.OpenMU.AdminPanel.Services;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// A generic edit page, which shows an <see cref="AutoForm{T}"/> for the given <see cref="TypeString"/> and <see cref="Id"/>.
    /// </summary>
    [Route("/edit/{typeString}/{id:guid}")]
    public sealed class Edit : ComponentBase, IDisposable
    {
        private object model;
        private Type type;
        private IContext persistenceContext;
        private CancellationTokenSource disposeCts;

        /// <summary>
        /// Gets or sets the identifier of the object which should be edited.
        /// </summary>
        [Parameter]
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Type.FullName"/> of the object which should be edited.
        /// </summary>
        [Parameter]
        public string TypeString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the persistence context provider which loads and saves the object.
        /// </summary>
        [Inject]
        public IPersistenceContextProvider PersistenceContextProvider { get; set; }

        /// <summary>
        /// Gets or sets the modal service.
        /// </summary>
        [Inject]
        public IModalService ModalService { get; set; }

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
                var downloadMarkup = this.GetDownloadMarkup();
                builder.AddMarkupContent(0, $"<h1>Edit {this.type.Name}</h1>{downloadMarkup}\r\n");
                builder.OpenComponent<CascadingValue<IContext>>(1);
                builder.AddAttribute(2, nameof(CascadingValue<IContext>.Value), this.persistenceContext);
                builder.AddAttribute(3, nameof(CascadingValue<IContext>.IsFixed), true);
                builder.AddAttribute(4, nameof(CascadingValue<IContext>.ChildContent), (RenderFragment)(builder2 =>
                {
                    builder2.OpenComponent(5, typeof(AutoForm<>).MakeGenericType(this.type));
                    builder2.AddAttribute(6, nameof(AutoForm<object>.Model), this.model);
                    builder2.AddAttribute(7, nameof(AutoForm<object>.OnValidSubmit),
                        EventCallback.Factory.Create(this, this.SaveChanges));
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
            this.disposeCts = new CancellationTokenSource();
            this.model = null;
            Task.Run(() => this.LoadData(this.disposeCts.Token), this.disposeCts.Token);
            return base.OnParametersSetAsync();
        }

        private string GetDownloadMarkup()
        {
            if (GenericControllerFeatureProvider.SupportedTypes.Any(t => t.Item1 == type))
            {
                var uri = $"/download/{this.type.Name}/{this.type.Name}_{this.Id}.json";
                return $"<p>Download as json: <a href=\"{uri}\" download><span class=\"oi oi-data-transfer-download\"></span></a></p>";
            }

            return null;
        }

        private async Task LoadData(CancellationToken cancellationToken)
        {
            IDisposable modal = null;
            var showModalTask = this.InvokeAsync(() => modal = this.ModalService.ShowLoadingIndicator());
            this.type = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName.StartsWith(nameof(MUnique)))
                .Select(assembly => assembly.GetType(this.TypeString)).FirstOrDefault(t => t != null);
            if (this.type == null)
            {
                throw new InvalidOperationException($"Only types of namespace {nameof(MUnique)} can be edited on this page.");
            }

            var createContextMethod = typeof(IPersistenceContextProvider).GetMethod(nameof(IPersistenceContextProvider.CreateNewTypedContext)).MakeGenericMethod(this.type);
            this.persistenceContext = (IContext)createContextMethod.Invoke(this.PersistenceContextProvider, Array.Empty<object>());

            var method = typeof(IContext).GetMethod(nameof(IContext.GetById)).MakeGenericMethod(this.type);
            try
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    this.model = method.Invoke(this.persistenceContext, new object[] { this.Id });
                    await showModalTask;
                    modal.Dispose();
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
                text = this.persistenceContext.SaveChanges() ? "The changes have been saved." : "There were no changes to save.";
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
