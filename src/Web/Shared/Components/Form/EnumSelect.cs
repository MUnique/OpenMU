// <copyright file="EnumSelect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.DataModel;

/// <summary>
/// A dropdown selection component for enum values of <typeparamref name="TValue" />.
/// </summary>
/// <typeparam name="TValue">The type of the enum.</typeparam>
public class EnumSelect<TValue> : NotifyableInputBase<TValue>
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
        builder.AddAttribute(i++, "onchange", EventCallback.Factory.CreateBinder<string>(this, value => this.CurrentValueAsString = value, this.CurrentValueAsString ?? string.Empty));

        foreach (var enumValue in Enum.GetValues(typeof(TValue)))
        {
            var name = Enum.GetName(typeof(TValue), enumValue);
            var displayName = typeof(TValue).GetField(name!)!.GetCustomAttribute<DisplayAttribute>()?.GetName();
            builder.OpenElement(i++, "option");
            builder.AddAttribute(i++, "value", enumValue.ToString());
            if (this.CurrentValueAsString == enumValue.ToString())
            {
                builder.AddAttribute(i++, "selected", true);
            }

            var caption = displayName ?? (enumValue is DayOfWeek day
                ? CultureHelper.GetDayName(day)
                : ModelResourceProvider.GetEnumCaption(typeof(TValue), (Enum)enumValue));
            builder.AddContent(i++, caption);
            builder.CloseElement();
        }

        builder.CloseElement();
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (!typeof(TValue).IsEnum)
        {
            throw new InvalidOperationException($"{this.GetType()} does not support the type '{typeof(TValue)}'.");
        }

        if (BindConverter.TryConvertTo<TValue>(value, CultureInfo.CurrentCulture, out var parsedValue))
        {
            result = parsedValue;
            validationErrorMessage = null;
            return true;
        }

        result = default;
        validationErrorMessage = string.Format(MUnique.OpenMU.Web.Shared.Properties.Resources.InvalidField, this.FieldIdentifier.Model.GetType().GetPropertyCaption(this.FieldIdentifier.FieldName));
        return false;
    }
}