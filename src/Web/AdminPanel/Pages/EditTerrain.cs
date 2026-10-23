// <copyright file="EditTerrain.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.TerrainEditor;

/// <summary>
/// A page, which shows a <see cref="TerrainEditor"/> for the terrains of all <see cref="GameConfiguration.Maps"/>.
/// </summary>
[Route("/terrain-editor")]
[Route("/terrain-editor/{SelectedMapId:guid}")]
public sealed class EditTerrain : MapEditorPageBase<TerrainEditor>
{
    /// <inheritdoc />
    protected override string Caption => Resources.TerrainEditor;
}
