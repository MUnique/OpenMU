// <copyright file="EditConnectionServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.AdminPanel.Components;

/// <summary>
/// Edit page for the <see cref="ConnectServerConfiguration"/>.
/// </summary>
[Route("/edit-connectionServer/{id:guid}")]
public sealed class EditConnectionServer : EditBase
{
    /// <inheritdoc />
    protected override Type? Type => typeof(ConnectServerDefinition);

    /// <inheritdoc />
    protected override void AddFormToRenderTree(RenderTreeBuilder builder, ref int currentSequence)
    {
        builder.OpenComponent<ConnectServerConfiguration>(++currentSequence);
        builder.AddAttribute(++currentSequence, nameof(ConnectServerConfiguration.Model), this.Model);
        builder.AddAttribute(++currentSequence, nameof(ConnectServerConfiguration.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChangesAsync));
        builder.CloseComponent();
    }
}