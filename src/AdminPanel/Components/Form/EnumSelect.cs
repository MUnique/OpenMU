// <copyright file="EnumSelect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;

namespace MUnique.OpenMU.AdminPanel.Components.Form
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Reflection;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.AspNetCore.Components.Rendering;

    /// <summary>
    /// A dropdown selection component for enum values of <see cref="TValue"/>.
    /// </summary>
    public class EnumSelect<TValue> : InputBase<TValue>
    {
        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int i = 0;
            builder.OpenElement(i++, "select");
            builder.AddMultipleAttributes(i++, this.AdditionalAttributes);
            builder.AddAttribute(i++, "class", this.CssClass);
            builder.AddAttribute(i++, "id", this.FieldIdentifier.FieldName);
            builder.AddAttribute(i++, "value", this.CurrentValueAsString);
            builder.AddAttribute(i++, "onchange", EventCallback.Factory.CreateBinder<string>(this, value => this.CurrentValueAsString = value, this.CurrentValueAsString));

            foreach (var enumValue in Enum.GetValues(typeof(TValue)))
            {
                var name = Enum.GetName(typeof(TValue), enumValue);
                var displayName = typeof(TValue).GetField(name).GetCustomAttribute<DisplayAttribute>()?.Name;
                builder.OpenElement(i++, "option");
                builder.AddAttribute(i++, "value", enumValue.ToString());
                if (this.CurrentValueAsString == enumValue.ToString())
                {
                    builder.AddAttribute(i++, "selected", true);
                }

                builder.AddContent(i++, displayName ?? name);
                builder.CloseElement();
            }

            builder.CloseElement();
        }

        /// <inheritdoc />
        protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
        {
            if (!typeof(TValue).IsEnum)
            {
                throw new InvalidOperationException($"{this.GetType()} does not support the type '{typeof(TValue)}'.");
            }

            var success = BindConverter.TryConvertTo<TValue>(value, CultureInfo.CurrentCulture, out var parsedValue);
            if (success)
            {
                result = parsedValue;
                validationErrorMessage = null;
                return true;
            }

            result = default;
            validationErrorMessage = $"The {this.FieldIdentifier.FieldName} field is not valid.";
            return false;
        }

        private void OnValueChanged(string value)
        {
            if (this.TryParseValueFromString(value, out var enumValue, out var _))
            {
                this.CurrentValueAsString = value;
                this.ValueChanged.InvokeAsync(enumValue);
            }
        }
    }
}
