// <copyright file="InputNullableByte.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;

/// <summary>
/// An input component for editing numeric byte values.
/// </summary>
public class InputNullableByte : InputByteBase<byte?>
{
    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out byte? result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (string.IsNullOrEmpty(value))
        {
            validationErrorMessage = null;
            result = null;
            return true;
        }

        if (byte.TryParse(value, out var parsed))
        {
            result = parsed;
            validationErrorMessage = null;
            return true;
        }

        result = null;
        validationErrorMessage = string.Format(this.ParsingErrorMessage, this.FieldIdentifier.FieldName);
        return false;
    }

    /// <inheritdoc />
    protected override string FormatValueAsString(byte? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return BindConverter.FormatValue(value, CultureInfo.InvariantCulture) as string ?? string.Empty;
    }
}