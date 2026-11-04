// <copyright file="EditConfig.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Globalization;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Form;

/// <summary>
/// A generic edit page, which shows an <see cref="AutoForm{T}"/> for the given <see cref="EditBase.TypeString"/> and <see cref="EditBase.Id"/>.
/// </summary>
[Route("/edit-config/{typeString}/")]
[Route("/edit-config/{typeString}/{id:guid}")]
[Route("/edit-config/{typeString}/{id:guid}/hide-collections")]
public sealed partial class EditConfig : EditBase
{
    private static readonly IDictionary<Type, IList<(string Caption, string Path)>> EditorPages =
        new Dictionary<Type, IList<(string, string)>>
        {
            { typeof(GameMapDefinition), new List<(string, string)> { (Resources.MapEditor, "/map-editor/{0}") } },
        };

    /// <summary>
    /// Gets or sets the optional search term to pre-filter fields.
    /// </summary>
    [SupplyParameterFromQuery(Name = "search")]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets a value indicating whether collection properties should be hidden.
    /// </summary>
    private bool HideCollections => this.NavigationManager.Uri.EndsWith("hide-collections");

    /// <summary>
    /// Gets the closed <see cref="AutoForm{T}"/> type for the current <see cref="EditBase.Type"/>.
    /// </summary>
    private Type? AutoFormType => this.Type is null ? null : typeof(AutoForm<>).MakeGenericType(this.Type);

    /// <summary>
    /// Gets the parameters for the <see cref="AutoForm{T}"/> component.
    /// </summary>
    private Dictionary<string, object?> AutoFormParameters => new()
    {
        [nameof(AutoForm<object>.Model)] = this.Model,
        [nameof(AutoForm<object>.HideCollections)] = this.HideCollections,
        [nameof(AutoForm<object>.SearchTerm)] = this.SearchTerm,
        [nameof(AutoForm<object>.OnValidSubmit)] = EventCallback.Factory.Create(this, this.SaveChangesAsync),
        [nameof(AutoForm<object>.OnRefresh)] = EventCallback.Factory.Create(this, this.RefreshAsync),
    };

    /// <inheritdoc />
    protected override string? GetEditorsMarkup()
    {
        StringBuilder? stringBuilder = null;
        if (this.Type is not null
            && (EditorPages.TryGetValue(this.Type, out var editors)
                || (this.Type.BaseType is { } baseType && EditorPages.TryGetValue(baseType, out editors))))
        {
            foreach (var editor in editors)
            {
                var uri = string.Format(CultureInfo.InvariantCulture, editor.Path, this.Id);
                stringBuilder ??= new StringBuilder();
                stringBuilder.Append($@"<p><a href=""{uri}"">{editor.Caption}</a></p>");
            }
        }

        return stringBuilder?.ToString();
    }
}
