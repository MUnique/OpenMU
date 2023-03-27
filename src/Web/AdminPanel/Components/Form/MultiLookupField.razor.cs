// <copyright file="MultiLookupField.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Lookup field which allows to select multiple objects which will be stored in a bound <see cref="IList{TObject}"/>.
/// </summary>
/// <typeparam name="TObject">The type of the object.</typeparam>
public partial class MultiLookupField<TObject>
    where TObject : class
{
    /// <summary>
    /// Gets or sets the label which should be displayed. If it's not explicitly provided, the component shows the
    /// Name defined in the <see cref="DisplayAttribute"/>. If there is no Name in a <see cref="DisplayAttribute"/>, it shows the property name instead.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the persistence context.
    /// </summary>
    [CascadingParameter(Name = "PersistenceContext")]
    protected IContext PersistenceContext { get; set; } = null!;

    /// <summary>
    /// Gets or sets the lookup controller.
    /// </summary>
    [Inject]
    public ILookupController LookupController { get; set; } = null!;

    /// <summary>
    /// Gets or sets the explicit lookup controller which should be used instead
    /// of the injected <see cref="LookupController"/>.
    /// </summary>
    [Parameter]
    public ILookupController? ExplicitLookupController { get; set; }

    /// <summary>
    /// Gets or sets the caption factory.
    /// </summary>
    [Parameter]
    public Func<TObject, string> CaptionFactory { get; set; } = obj => obj.GetName();

    private ILookupController EffectiveLookupController => this.ExplicitLookupController ?? this.LookupController;

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out IList<TObject> result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        throw new NotImplementedException();
    }
}