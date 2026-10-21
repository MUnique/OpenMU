// <copyright file="TerrainEditorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.TerrainEditor;

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Components.TerrainEditor;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using TerrainEditorComponent = MUnique.OpenMU.Web.Shared.Components.TerrainEditor.TerrainEditor;

/// <summary>
/// Tests for the <see cref="TerrainEditorComponent"/>.
/// </summary>
[TestFixture]
public class TerrainEditorTests
{
    private const string ModulePath = "./_content/MUnique.OpenMU.Web.Shared/Components/TerrainEditor/TerrainEditor.razor.js";

    private BunitContext _context = null!;

    private BunitJSModuleInterop _module = null!;

    /// <summary>
    /// Setups the test objects.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
        this._module = this._context.JSInterop.SetupModule(ModulePath);
        this._module.Mode = JSRuntimeMode.Loose;
        this._context.Services.AddSingleton(Mock.Of<IToastService>());
        this._context.Services.AddSingleton(Mock.Of<ILogger<TerrainEditorComponent>>());
    }

    /// <summary>
    /// Tears down the test objects.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._context.Dispose();
    }

    /// <summary>
    /// Tests that the default terrain and the variants of the selected map can be selected.
    /// </summary>
    [Test]
    public void TerrainsOfSelectedMapAreSelectable()
    {
        var map = CreateMap(32);
        var variant = new BasicModel.GameMapTerrainVariant { Number = 2, Description = "War" };
        map.TerrainVariants.Add(variant);

        var component = this.Render(map);

        var options = component.FindAll("#terrainSelect option");
        Assert.That(options.Select(o => o.TextContent), Is.EqualTo(new[] { Shared.Properties.Resources.DefaultTerrain, variant.ToString() }));
    }

    /// <summary>
    /// Tests that the editor is created with the terrain of the selected map.
    /// </summary>
    [Test]
    public void TerrainOfSelectedMapIsLoaded()
    {
        var map = CreateMap(0);
        var tiles = new byte[TerrainFileConverter.TileCount];
        tiles[42] = (byte)TerrainAttributeType.Blocked;
        map.TerrainData = TerrainFileConverter.ToServerFile(tiles);

        this.Render(map);

        this._module.VerifyInvoke("createEditor");
        var setData = this._context.JSInterop.Invocations.Single(i => i.Identifier == "setData");
        Assert.That(setData.Arguments[0], Is.EqualTo(Convert.ToBase64String(tiles)));
        Assert.That(setData.Arguments[1], Is.False);
    }

    /// <summary>
    /// Tests that the client terrain file is downloaded with the file name which the client expects.
    /// </summary>
    [Test]
    public void ClientFileIsDownloaded()
    {
        var map = CreateMap(32);
        var component = this.Render(map);

        component.FindAll("button").Single(b => b.TextContent.Contains(Shared.Properties.Resources.DownloadClientTerrain, StringComparison.Ordinal)).Click();

        var download = this._module.VerifyInvoke("downloadFile");
        Assert.That(download.Arguments[0], Is.EqualTo("EncTerrain33.att"));
        var content = Convert.FromBase64String((string)download.Arguments[1]!);
        Assert.That(TerrainFileConverter.TryReadFile(content, out var tiles), Is.True);
        Assert.That(tiles, Is.All.EqualTo(0));
    }

    private static BasicModel.GameMapDefinition CreateMap(short number)
    {
        return new BasicModel.GameMapDefinition { Number = number, Name = $"Map {number}" };
    }

    private IRenderedComponent<TerrainEditorComponent> Render(GameMapDefinition map)
    {
        return this._context.Render<TerrainEditorComponent>(parameters => parameters
            .Add(p => p.Maps, [map]));
    }
}
