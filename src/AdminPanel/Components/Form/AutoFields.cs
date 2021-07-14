// <copyright file="AutoFields.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Components.Form
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.AspNetCore.Components.Rendering;
    using MUnique.OpenMU.AdminPanel.ComponentBuilders;
    using MUnique.OpenMU.AdminPanel.Services;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// A razor component which automatically generates input fields for all properties for the type of the enclosing form model.
    /// Must be used inside a <see cref="EditForm"/>.
    /// </summary>
    public class AutoFields : ComponentBase
    {
        private static readonly IList<IComponentBuilder> Builders = new List<IComponentBuilder>();

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

        /// <summary>
        /// Initializes static members of the <see cref="AutoFields"/> class.
        /// </summary>
        static AutoFields()
        {
            Builders.Add(new TextFieldBuilder());
            Builders.Add(new NumberFieldBuilder<long>());
            Builders.Add(new NumberFieldBuilder<int>());
            Builders.Add(new NumberFieldBuilder<decimal>());
            Builders.Add(new NumberFieldBuilder<double>());
            Builders.Add(new NumberFieldBuilder<float>());
            Builders.Add(new ByteFieldBuilder());
            Builders.Add(new ShortFieldBuilder());
            Builders.Add(new BooleanFieldBuilder());
            Builders.Add(new DateFieldBuilder());
            Builders.Add(new EnumFieldBuilder());
            Builders.Add(new LookupFieldBuilder());
            Builders.Add(new EmbeddedFormFieldBuilder());
            Builders.Add(new ObjectCollectionFieldBuilder());
            Builders.Add(new IntCollectionFieldBuilder());
            Builders.Add(new ByteArrayFieldBuilder());
        }

        /// <summary>
        /// Gets or sets the context of the <see cref="EditForm"/>.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        [CascadingParameter]
        public EditContext Context { get; set; } = null!;

        /// <summary>
        /// Gets or sets the notification service.
        /// </summary>
        /// <value>
        /// The notification service.
        /// </value>
        [Inject]
        public IChangeNotificationService NotificationService { get; set; } = null!;

        /// <summary>
        /// Gets the properties which should be shown in this component.
        /// </summary>
        /// <returns>The properties which should be shown in this component.</returns>
        protected virtual IEnumerable<PropertyInfo> Properties
        {
            get
            {
                if (this.Context?.Model is null)
                {
                    Log.Error(this.Context is null ? "Context is null" : "Model is null");
                    return Enumerable.Empty<PropertyInfo>();
                }

                try
                {
                    return this.Context.Model.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                        .Where(p => p.GetCustomAttribute<TransientAttribute>() is null)
                        .Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable ?? true)
                        .Where(p => !p.Name.StartsWith("Raw", StringComparison.InvariantCulture))
                        .Where(p => !p.Name.StartsWith("Joined", StringComparison.InvariantCulture))
                        .Where(p => !p.GetIndexParameters().Any())
                        .OrderByDescending(p => p.PropertyType == typeof(string))
                        .ThenByDescending(p => p.PropertyType.IsValueType)
                        .ThenByDescending(p => !p.PropertyType.IsGenericType)
                        .ToList();
                }
                catch (Exception ex)
                {
                    Log.Error($"Error during determining properties of type {this.Context.Model.GetType()}: {ex.Message}{Environment.NewLine}{ex.StackTrace}", ex);
                }

                return Enumerable.Empty<PropertyInfo>();
            }
        }

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int i = 0;
            foreach (var propertyInfo in this.Properties)
            {
                IComponentBuilder? componentBuilder = null;
                try
                {
                    componentBuilder = Builders.FirstOrDefault(b => b.CanBuildComponent(propertyInfo));
                    if (componentBuilder != null)
                    {
                        i = componentBuilder.BuildComponent(this.Context.Model, propertyInfo, builder, i, this.NotificationService);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error building component for property {this.Context.Model.GetType().Name}.{propertyInfo.Name} with component builder {componentBuilder}: {ex.Message}{Environment.NewLine}{ex.StackTrace}", ex);
                }
            }
        }
    }
}
