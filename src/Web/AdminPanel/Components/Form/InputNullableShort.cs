// <copyright file="InputNullableShort.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;

/// <summary>
/// An input component for editing numeric short values.
/// </summary>
public class InputNullableShort : InputShortBase<short?>
{
    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, out short? result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (string.IsNullOrEmpty(value))
        {
            validationErrorMessage = null;
            result = null;
            return true;
        }

        if (short.TryParse(value, out var parsed))
        {
            validationErrorMessage = null;
            result = parsed;
            return true;
        }

        result = null;
        validationErrorMessage = string.Format(this.ParsingErrorMessage, this.FieldIdentifier.FieldName);
        return false;
    }

    /// <inheritdoc />
    protected override string FormatValueAsString(short? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return BindConverter.FormatValue(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
}