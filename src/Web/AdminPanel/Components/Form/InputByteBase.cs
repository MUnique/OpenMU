// <copyright file="InputByteBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// An input component for editing numeric byte values.
/// </summary>
/// <typeparam name="TByte">The type of the byte.</typeparam>
public abstract class InputByteBase<TByte> : InputBase<TByte>
{
    /// <summary>
    /// Gets or sets the error message used when displaying an a parsing error.
    /// </summary>
    [Parameter]
    public string ParsingErrorMessage { get; set; } = "The {0} field must be a number between 0 and 255.";

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    [Parameter]
    public byte Min { get; set; } = byte.MinValue;

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    [Parameter]
    public byte Max { get; set; } = byte.MaxValue;

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
        builder.AddAttribute(7, "min", this.Min);
        builder.AddAttribute(8, "max", this.Max);
        builder.CloseElement();
    }
}