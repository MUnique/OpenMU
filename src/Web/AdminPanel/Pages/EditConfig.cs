// <copyright file="EditConfig.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;
using MUnique.OpenMU.Web.AdminPanel.Components.ItemEdit;

/// <summary>
/// A generic edit page, which shows an <see cref="AutoForm{T}"/> for the given <see cref="EditBase.TypeString"/> and <see cref="EditBase.Id"/>.
/// </summary>
[Route("/edit-config/{typeString}/")]
[Route("/edit-config/{typeString}/{id:guid}")]
[Route("/edit-config/{typeString}/{id:guid}/hide-collections")]
public sealed class EditConfig : EditBase
{
    private static readonly IDictionary<Type, IList<(string Caption, string Path)>> EditorPages =
        new Dictionary<Type, IList<(string, string)>>
        {
            { typeof(GameMapDefinition), new List<(string, string)> { ("Map Editor", "/map-editor/{0}") } },
        };

    /// <inheritdoc />
    protected override void AddFormToRenderTree(RenderTreeBuilder builder, ref int currentSequence)
    {
        var hideCollections = this.NavigationManager.Uri.EndsWith("hide-collections");

        if (this.Type == typeof(Item))
        {
            builder.OpenComponent(++currentSequence, typeof(ItemEdit));
            builder.AddAttribute(++currentSequence, nameof(ItemEdit.Item), this.Model);
            builder.AddAttribute(++currentSequence, nameof(ItemEdit.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChangesAsync));
            builder.CloseComponent();
        }
        else
        {
            builder.OpenComponent(++currentSequence, typeof(AutoForm<>).MakeGenericType(this.Type!));
            builder.AddAttribute(++currentSequence, nameof(AutoForm<object>.Model), this.Model);
            builder.AddAttribute(++currentSequence, nameof(AutoForm<object>.HideCollections), hideCollections);
            builder.AddAttribute(++currentSequence, nameof(AutoForm<object>.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChangesAsync));
            builder.CloseComponent();
        }
    }

    /// <inheritdoc />
    protected override string? GetEditorsMarkup()
    {
        StringBuilder? stringBuilder = null;
        if (this.Type is not null
            && (EditorPages.TryGetValue(this.Type, out var editors)
                || this.Type.BaseType is { } baseType && EditorPages.TryGetValue(baseType, out editors)))
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