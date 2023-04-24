// <copyright file="InputShort.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;

/// <summary>
/// An input component for editing numeric short values.
/// </summary>
public class InputShort : InputShortBase<short>
{
    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, out short result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (short.TryParse(value, out result))
        {
            validationErrorMessage = null;
            return true;
        }

        validationErrorMessage = string.Format(this.ParsingErrorMessage, this.FieldIdentifier.FieldName);
        return false;
    }

    /// <inheritdoc />
    protected override string FormatValueAsString(short value)
    {
        return BindConverter.FormatValue(value, CultureInfo.InvariantCulture);
    }
}