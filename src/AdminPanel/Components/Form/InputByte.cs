// <copyright file="InputByte.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Components.Form
{
    using System.Globalization;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.AspNetCore.Components.Rendering;

    /// <summary>
    /// An input component for editing numeric values.
    /// Supported numeric types are <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>.
    /// </summary>
    public class InputByte : InputBase<byte>
    {
        /// <summary>
        /// Gets or sets the error message used when displaying an a parsing error.
        /// </summary>
        [Parameter]
        public string ParsingErrorMessage { get; set; } = "The {0} field must be a number between 0 and 255.";

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "step", 1);
            builder.AddMultipleAttributes(2, this.AdditionalAttributes);
            builder.AddAttribute(3, "type", "number");
            builder.AddAttribute(4, "class", this.CssClass);
            builder.AddAttribute(5, "value", BindConverter.FormatValue(this.CurrentValueAsString));
            builder.AddAttribute(6, "onchange", EventCallback.Factory.CreateBinder<string>(this, v => this.CurrentValueAsString = v, this.CurrentValueAsString));
            builder.AddAttribute(7, "min", "0");
            builder.AddAttribute(8, "max", "255");
            builder.CloseElement();
        }

        /// <inheritdoc />
        protected override bool TryParseValueFromString(string value, out byte result, out string validationErrorMessage)
        {
            if (byte.TryParse(value, out result))
            {
                validationErrorMessage = null;
                return true;
            }

            validationErrorMessage = string.Format(this.ParsingErrorMessage, this.FieldIdentifier.FieldName);
            return false;
        }

        /// <inheritdoc />
        protected override string FormatValueAsString(byte value)
        {
            return (string)BindConverter.FormatValue(value, CultureInfo.InvariantCulture);
        }
    }
}
