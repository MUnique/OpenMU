// <copyright file="TerrainFileConverterTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.TerrainEditor;

using MUnique.OpenMU.Web.Shared.Components.TerrainEditor;

/// <summary>
/// Tests for the <see cref="TerrainFileConverter"/>.
/// </summary>
[TestFixture]
public class TerrainFileConverterTests
{
    /// <summary>
    /// Tests that the server file has the header of the original server, followed by the tiles.
    /// </summary>
    [Test]
    public void ServerFileHasHeaderAndTiles()
    {
        var tiles = CreateTiles();

        var file = TerrainFileConverter.ToServerFile(tiles);

        Assert.That(file, Has.Length.EqualTo(3 + TerrainFileConverter.TileCount));
        Assert.That(file[..3], Is.EqualTo(new byte[] { 0, 0xFF, 0xFF }));
        Assert.That(file[3..], Is.EqualTo(tiles));
    }

    /// <summary>
    /// Tests that the tiles of a server file can be read again.
    /// </summary>
    [Test]
    public void ServerFileCanBeRead()
    {
        var tiles = CreateTiles();

        var success = TerrainFileConverter.TryReadFile(TerrainFileConverter.ToServerFile(tiles), out var result);

        Assert.That(success, Is.True);
        Assert.That(result, Is.EqualTo(tiles));
    }

    /// <summary>
    /// Tests that the tiles of a client file can be read again.
    /// </summary>
    [Test]
    public void ClientFileCanBeRead()
    {
        var tiles = CreateTiles();

        var file = TerrainFileConverter.ToClientFile(tiles, 33);
        var success = TerrainFileConverter.TryReadFile(file, out var result);

        Assert.That(file, Has.Length.EqualTo(4 + TerrainFileConverter.TileCount));
        Assert.That(file[4..], Is.Not.EqualTo(tiles), "The client file should be encrypted.");
        Assert.That(success, Is.True);
        Assert.That(result, Is.EqualTo(tiles));
    }

    /// <summary>
    /// Tests that the start of an encrypted client file matches the known result of the client's encryption
    /// for the header of the first map (version 0, map 0, width and height 255) and a walkable first tile.
    /// </summary>
    [Test]
    public void ClientFileIsEncryptedLikeTheClient()
    {
        var file = TerrainFileConverter.ToClientFile(new byte[TerrainFileConverter.TileCount], 0);

        Assert.That(file[..5], Is.EqualTo(new byte[] { 0x8B, 0xE4, 0x27, 0x91, 0x4F }));
    }

    /// <summary>
    /// Tests that files of an unknown size are rejected.
    /// </summary>
    [Test]
    public void FileWithUnknownSizeIsRejected()
    {
        var success = TerrainFileConverter.TryReadFile(new byte[1000], out var result);

        Assert.That(success, Is.False);
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Tests that a file with the size of a client file, but which doesn't decrypt to a valid header, is rejected.
    /// </summary>
    [Test]
    public void ClientFileWithInvalidHeaderIsRejected()
    {
        var success = TerrainFileConverter.TryReadFile(new byte[4 + TerrainFileConverter.TileCount], out _);

        Assert.That(success, Is.False);
    }

    /// <summary>
    /// Tests that missing terrain data results in walkable tiles.
    /// </summary>
    [Test]
    public void MissingTerrainDataResultsInWalkableTiles()
    {
        var tiles = TerrainFileConverter.GetTileAttributes(null);

        Assert.That(tiles, Has.Length.EqualTo(TerrainFileConverter.TileCount));
        Assert.That(tiles, Is.All.EqualTo(0));
    }

    private static byte[] CreateTiles()
    {
        var tiles = new byte[TerrainFileConverter.TileCount];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = (byte)(i * 7);
        }

        return tiles;
    }
}
