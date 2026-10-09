// <copyright file="EditConnectionServer.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Edit page for the <see cref="MUnique.OpenMU.Web.AdminPanel.Components.ConnectServer.ConnectServerConfiguration"/>.
/// </summary>
[Route("/edit-connectionServer/{id:guid}")]
public sealed partial class EditConnectionServer : EditBase
{
    /// <inheritdoc />
    protected override Type? Type => typeof(ConnectServerDefinition);
}