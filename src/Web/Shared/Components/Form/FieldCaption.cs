// <copyright file="FieldCaption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using MUnique.OpenMU.DataModel;

/// <summary>Resolves field names consistently for validation messages and form labels.</summary>
public static class FieldCaption
{
    /// <summary>Gets the localized display name of a bound field.</summary>
    /// <param name="field">The model and property of the bound field.</param>
    /// <returns>The display attribute caption or model resource caption.</returns>
    public static string Get(FieldIdentifier field)
    {
        var type = field.Model.GetType();
        var property = type.GetProperty(field.FieldName);
        var display = property is null ? null : Attribute.GetCustomAttribute(property, typeof(DisplayAttribute), true) as DisplayAttribute;
        return display?.GetName() ?? type.GetPropertyCaption(field.FieldName);
    }
}
