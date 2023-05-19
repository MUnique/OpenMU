// <copyright file="InputByte.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;

/// <summary>
/// An input component for editing numeric byte values.
/// </summary>
/// <typeparam name="TByte">The type of the byte.</typeparam>
public class InputByte : InputByteBase<byte>
{
    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out byte result, [NotNullWhen(false)] out string? validationErrorMessage)
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
        return (string)BindConverter.FormatValue(value, CultureInfo.InvariantCulture)!;
    }
}