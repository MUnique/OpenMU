// <copyright file="Edit.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;

/// <summary>
/// A generic edit page, which shows an <see cref="AutoForm{T}"/> for the given <see cref="EditBase.TypeString"/> and <see cref="EditBase.Id"/>.
/// </summary>
[Route("/edit/{typeString}/{id:guid}")]
public sealed class Edit : EditBase
{
    /// <inheritdoc />
    protected override void AddFormToRenderTree(RenderTreeBuilder builder, ref int currentSequence)
    {
        builder.OpenComponent(++currentSequence, typeof(AutoForm<>).MakeGenericType(this.Type!));
        builder.AddAttribute(++currentSequence, nameof(AutoForm<object>.Model), this.Model);
        builder.AddAttribute(++currentSequence, nameof(AutoForm<object>.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChangesAsync));
        builder.CloseComponent();
    }
}