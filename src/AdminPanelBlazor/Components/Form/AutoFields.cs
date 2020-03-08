// <copyright file="AutoFields.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanelBlazor.Components.Form
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.AspNetCore.Components.Rendering;

    /// <summary>
    /// A razor component which automatically generates input fields for all properties for the type of the enclosing form model.
    /// Must be used inside a <see cref="EditForm"/>.
    /// </summary>
    public class AutoFields : ComponentBase
    {
        /// <summary>
        /// Gets or sets the context of the <see cref="EditForm"/>.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        [CascadingParameter]
        public EditContext Context { get; set; }

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            // The next code is not clean yet - I plan to create builder classes for each type and the possibility to register builders for specific properties or object types.
            int i = 0;
            foreach (var propertyInfo in this.Context.Model.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
            {
                if (!(propertyInfo.GetCustomAttribute<BrowsableAttribute>()?.Browsable ?? true))
                {
                    continue;
                }

                if (propertyInfo.Name.StartsWith("Raw") || propertyInfo.Name.StartsWith("Joined"))
                {
                    continue;
                }

                if (propertyInfo.PropertyType == typeof(string))
                {
                    this.BuildField<string, TextField>(propertyInfo, builder, ref i);
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    this.BuildField<int, NumberField<int>>(propertyInfo, builder, ref i);
                }
                else if (propertyInfo.PropertyType == typeof(float))
                {
                    this.BuildField<float, NumberField<float>>(propertyInfo, builder, ref i);
                }
                else if (propertyInfo.PropertyType == typeof(double))
                {
                    this.BuildField<double, NumberField<double>>(propertyInfo, builder, ref i);
                }
                else if (propertyInfo.PropertyType == typeof(long))
                {
                    this.BuildField<long, NumberField<long>>(propertyInfo, builder, ref i);
                }
                else if (propertyInfo.PropertyType == typeof(byte))
                {
                    this.BuildField<byte, ByteField>(propertyInfo, builder, ref i);
                }
                else if (propertyInfo.PropertyType == typeof(bool))
                {
                    this.BuildField<bool, BooleanField>(propertyInfo, builder, ref i);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    this.BuildField<DateTime, DateField>(propertyInfo, builder, ref i);
                }
                else if (propertyInfo.PropertyType.IsEnum)
                {
                    i = this.BuildEnumField(builder, propertyInfo, i);
                }
                else if (propertyInfo.PropertyType.IsArray)
                {
                    // not supported.
                }
                else if (propertyInfo.PropertyType.IsClass)
                {
                    i = this.BuildLookUpField(builder, propertyInfo, i);
                }
                else if (propertyInfo.PropertyType.IsInterface
                         && propertyInfo.PropertyType.IsGenericType
                         && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    i = this.BuiltItemTableField(builder, propertyInfo, i);
                }
                else
                {
                    // not supported.
                }
            }
        }

        private int BuildEnumField(RenderTreeBuilder builder, PropertyInfo propertyInfo, int i)
        {
            var method = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == nameof(this.BuildField))
                .First(m => m.ContainsGenericParameters && m.GetGenericArguments().Length == 2)
                .MakeGenericMethod(propertyInfo.PropertyType, typeof(EnumField<>).MakeGenericType(propertyInfo.PropertyType));
            var parameters = new object[] {propertyInfo, builder, i};
            method.Invoke(this, parameters);
            i = (int)parameters[2];
            return i;
        }

        private int BuildLookUpField(RenderTreeBuilder builder, PropertyInfo propertyInfo, int i)
        {
            var method = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == nameof(this.BuildField))
                .First(m => m.ContainsGenericParameters && m.GetGenericArguments().Length == 2)
                .MakeGenericMethod(propertyInfo.PropertyType, typeof(LookupField<>).MakeGenericType(propertyInfo.PropertyType));
            var parameters = new object[] { propertyInfo, builder, i };
            method.Invoke(this, parameters);
            i = (int)parameters[2];
            return i;
        }

        private int BuiltItemTableField(RenderTreeBuilder builder, PropertyInfo propertyInfo, int i)
        {
            var method = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == nameof(this.BuildField))
                .First(m => m.ContainsGenericParameters && m.GetGenericArguments().Length == 2)
                .MakeGenericMethod(propertyInfo.PropertyType, typeof(ItemTable<>).MakeGenericType(propertyInfo.PropertyType.GenericTypeArguments[0]));
            var parameters = new object[] { propertyInfo, builder, i };
            method.Invoke(this, parameters);
            i = (int)parameters[2];
            return i;
        }

        private void BuildField<TValue, TComponent>(PropertyInfo propertyInfo, RenderTreeBuilder builder, ref int i)
        {
            this.BuildField<TValue>(typeof(TComponent), propertyInfo, builder, ref i);
        }

        private void BuildField<TValue>(Type componentType, PropertyInfo propertyInfo, RenderTreeBuilder builder, ref int i)
        {
            builder.OpenComponent(i++, componentType);
            try
            {
                builder.AddAttribute(i++, "ValueExpression", this.Context.Model.CreatePropertyExpression<TValue>(propertyInfo));
                builder.AddAttribute(i++, "Value", propertyInfo.GetValue(this.Context.Model));
                builder.AddAttribute(i++, "ValueChanged", EventCallback.Factory.Create<TValue>(this, EventCallback.Factory.Create<TValue>(
                    this,
                    value =>
                    {
                        if (propertyInfo.SetMethod is { })
                        {
                            propertyInfo.SetValue(this.Context.Model, value);
                        }
                    })));
            }
            finally
            {
                builder.CloseComponent();
            }
        }
    }
}
