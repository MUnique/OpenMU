// <copyright file="BaseComponentBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.ComponentBuilders
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Rendering;

    /// <summary>
    /// Base class for component builders which offers some generic functions to create generic components.
    /// </summary>
    public class BaseComponentBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseComponentBuilder"/> class.
        /// </summary>
        protected BaseComponentBuilder()
        {
        }

        /// <summary>
        /// Builds a generic component of type <paramref name="genericBaseComponentType"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="genericBaseComponentType">Type of the generic base component.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="currentIndex">The current index in the render tree.</param>
        /// <returns>The updated index in the render tree.</returns>
        protected int BuildGenericField(object model, Type genericBaseComponentType, RenderTreeBuilder builder, PropertyInfo propertyInfo, int currentIndex)
        {
            var method = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == nameof(this.BuildField))
                .First(m => m.ContainsGenericParameters && m.GetGenericArguments().Length == 1)
                .MakeGenericMethod(propertyInfo.PropertyType);
            var componentType = genericBaseComponentType.MakeGenericType(propertyInfo.PropertyType);
            var parameters = new[] { model, componentType, propertyInfo, builder, currentIndex };
            return (int)method.Invoke(this, parameters);
        }

        /// <summary>
        /// Builds a <typeparamref name="TComponent"/> for the type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="currentIndex">The current index in the render tree.</param>
        /// <returns>The updated index in the render tree.</returns>
        protected int BuildField<TValue, TComponent>(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex)
        {
            return this.BuildField<TValue>(model, typeof(TComponent), propertyInfo, builder, currentIndex);
        }

        /// <summary>
        /// Builds a <paramref name="componentType"/> for the type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="componentType">The type of the component.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="currentIndex">The current index in the render tree.</param>
        /// <returns>The updated index in the render tree.</returns>
        protected int BuildField<TValue>(object model, Type componentType, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex)
        {
            builder.OpenComponent(currentIndex++, componentType);
            try
            {
                builder.AddAttribute(currentIndex++, "ValueExpression", model.CreatePropertyExpression<TValue>(propertyInfo));
                builder.AddAttribute(currentIndex++, "Value", propertyInfo.GetValue(model));
                builder.AddAttribute(currentIndex++, "ValueChanged", EventCallback.Factory.Create(this, EventCallback.Factory.Create<TValue>(
                    this,
                    value =>
                    {
                        if (propertyInfo.SetMethod is { })
                        {
                            propertyInfo.SetValue(model, value);
                        }
                    })));
            }
            finally
            {
                builder.CloseComponent();
            }

            return currentIndex;
        }
    }
}