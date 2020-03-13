// <copyright file="Edit.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Pages
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Rendering;
    using MUnique.OpenMU.AdminPanel.Components.Form;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// A generic edit page, which shows an <see cref="AutoForm{T}"/> for the given <see cref="TypeString"/> and <see cref="Id"/>.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
    [Route("/edit/{typeString}/{id:guid}")]
    public sealed class Edit : ComponentBase, IDisposable
    {
        private object model;
        private Type type;
        private IContext persistenceContext;

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

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.persistenceContext is IDisposable disposable)
            {
                disposable.Dispose();
                this.persistenceContext = null;
            }
        }

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddMarkupContent(0, $"<h1>Edit {this.type.Name}</h1>\r\n");
            var autoFormType = typeof(AutoForm<>).MakeGenericType(this.type);
            builder.OpenComponent(1, autoFormType);
            builder.AddAttribute(2, nameof(AutoForm<object>.Model), this.model);
            builder.AddAttribute(3, nameof(AutoForm<object>.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChanges));
            builder.CloseComponent();
        }

        /// <inheritdoc />
        protected override Task OnParametersSetAsync()
        {
            this.type = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName.StartsWith(nameof(MUnique)))
                .Select(assembly => assembly.GetType(this.TypeString)).FirstOrDefault(t => t != null);
            if (this.type == null)
            {
                throw new ArgumentException($"Only types of namespace {nameof(MUnique)} can be edited on this page.");
            }

            var createContextMethod = typeof(IPersistenceContextProvider).GetMethod(nameof(IPersistenceContextProvider.CreateNewTypedContext)).MakeGenericMethod(this.type);
            this.persistenceContext = (IContext)createContextMethod.Invoke(this.PersistenceContextProvider, Array.Empty<object>());

            var method = typeof(IContext).GetMethod(nameof(IContext.GetById)).MakeGenericMethod(this.type);

            this.model = method.Invoke(this.persistenceContext, new object[] { this.Id });
            return base.OnParametersSetAsync();
        }

        private void SaveChanges()
        {
            this.persistenceContext.SaveChanges();
        }
    }
}
