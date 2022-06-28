// <copyright file="ExitGateField.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Field for an <see cref="ExitGate"/> property.
/// </summary>
public partial class ExitGateField : NotifyableInputBase<ExitGate>
{
    private bool _showPicker;

    /// <summary>
    /// Gets or sets the label which should be displayed. If it's not explicitly provided, the component shows the
    /// Name defined in the <see cref="DisplayAttribute"/>. If there is no Name in a <see cref="DisplayAttribute"/>, it shows the property name instead.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out ExitGate result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        throw new NotImplementedException();
    }
}