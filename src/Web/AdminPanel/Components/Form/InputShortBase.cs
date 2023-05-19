// <copyright file="InputShortBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// An input component for editing numeric short values.
/// </summary>
/// <typeparam name="TShort">The type of the short.</typeparam>
public abstract class InputShortBase<TShort> : InputBase<TShort>
{
    /// <summary>
    /// Gets or sets the error message used when displaying an a parsing error.
    /// </summary>
    [Parameter]
    public string ParsingErrorMessage { get; set; } = $"The {0} field must be a number between {short.MinValue} and {short.MaxValue}.";

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddAttribute(1, "step", 1);
        builder.AddMultipleAttributes(2, this.AdditionalAttributes);
        builder.AddAttribute(3, "type", "number");
        builder.AddAttribute(4, "class", this.CssClass);
        builder.AddAttribute(5, "value", BindConverter.FormatValue(this.CurrentValueAsString));
        builder.AddAttribute(6, "onchange", EventCallback.Factory.CreateBinder<string>(this, v => this.CurrentValueAsString = v, this.CurrentValueAsString!));
        builder.AddAttribute(7, "min", short.MinValue.ToString(CultureInfo.InvariantCulture));
        builder.AddAttribute(8, "max", short.MaxValue.ToString(CultureInfo.InvariantCulture));
        builder.CloseElement();
    }
}