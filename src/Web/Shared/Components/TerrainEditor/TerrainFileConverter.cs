// <copyright file="TerrainFileConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.TerrainEditor;

using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Converts terrain attribute data between the format which is stored in
/// <see cref="GameMapDefinition.TerrainData"/> (the *.att file of the original server)
/// and the encrypted EncTerrain*.att file of the game client.
/// </summary>
/// <remarks>
/// Both formats hold one attribute byte per tile of the 256 x 256 tiles of a map, row by row (y major).
/// The server file starts with a header of 3 bytes (version, width - 1, height - 1).
/// The client file starts with a header of 4 bytes (version, map number, width - 1, height - 1),
/// is XOR-ed with a 3 byte key and then encrypted with the map file encryption of the client.
/// Newer clients may also use two bytes per tile, of which only the low byte is relevant.
/// </remarks>
public static class TerrainFileConverter
{
    /// <summary>
    /// The number of tiles of a terrain.
    /// </summary>
    public const int TileCount = 256 * 256;

    private const int ServerHeaderSize = 3;

    private const int ClientHeaderSize = 4;

    private const byte MaximumCoordinate = 0xFF;

    private const byte InitialMapKey = 0x5E;

    private const byte MapKeyIncrement = 0x3D;

    private static readonly byte[] MapXorKey = [0xD1, 0x73, 0x52, 0xF6, 0xD2, 0x9A, 0xCB, 0x27, 0x3E, 0xAF, 0x59, 0x31, 0x37, 0xB3, 0xE7, 0xA2];

    private static readonly byte[] BuxKey = [0xFC, 0xCF, 0xAB];

    /// <summary>
    /// Gets the attributes of all tiles from the stored terrain data.
    /// </summary>
    /// <param name="terrainData">The terrain data, as stored in <see cref="GameMapDefinition.TerrainData"/>.</param>
    /// <returns>The attributes of all <see cref="TileCount"/> tiles. Missing data results in walkable tiles.</returns>
    public static byte[] GetTileAttributes(byte[]? terrainData)
    {
        var result = new byte[TileCount];
        if (terrainData is { Length: > ServerHeaderSize })
        {
            var source = terrainData.AsSpan(ServerHeaderSize);
            source[..Math.Min(source.Length, TileCount)].CopyTo(result);
        }

        return result;
    }

    /// <summary>
    /// Creates the terrain data of the server format, as stored in <see cref="GameMapDefinition.TerrainData"/>.
    /// </summary>
    /// <param name="tileAttributes">The attributes of all tiles.</param>
    /// <returns>The terrain data.</returns>
    public static byte[] ToServerFile(ReadOnlySpan<byte> tileAttributes)
    {
        ValidateTileAttributes(tileAttributes);
        var result = new byte[ServerHeaderSize + TileCount];
        result[1] = MaximumCoordinate;
        result[2] = MaximumCoordinate;
        tileAttributes.CopyTo(result.AsSpan(ServerHeaderSize));
        return result;
    }

    /// <summary>
    /// Creates the encrypted terrain file which is loaded by the game client (EncTerrain*.att).
    /// </summary>
    /// <param name="tileAttributes">The attributes of all tiles.</param>
    /// <param name="mapNumber">The number of the map.</param>
    /// <returns>The content of the client terrain file.</returns>
    public static byte[] ToClientFile(ReadOnlySpan<byte> tileAttributes, short mapNumber)
    {
        ValidateTileAttributes(tileAttributes);
        var result = new byte[ClientHeaderSize + TileCount];
        result[1] = unchecked((byte)mapNumber);
        result[2] = MaximumCoordinate;
        result[3] = MaximumCoordinate;
        tileAttributes.CopyTo(result.AsSpan(ClientHeaderSize));
        ApplyBuxKey(result);
        EncryptMapFile(result);
        return result;
    }

    /// <summary>
    /// Reads the tile attributes from a terrain file, which can be either of the server or of the client format.
    /// </summary>
    /// <param name="fileContent">The content of the file.</param>
    /// <param name="tileAttributes">The attributes of all tiles, if the file could be read.</param>
    /// <returns><c>true</c>, if the file has a known format; otherwise, <c>false</c>.</returns>
    public static bool TryReadFile(ReadOnlySpan<byte> fileContent, [NotNullWhen(true)] out byte[]? tileAttributes)
    {
        tileAttributes = null;
        if (fileContent.Length == ServerHeaderSize + TileCount)
        {
            tileAttributes = fileContent[ServerHeaderSize..].ToArray();
            return true;
        }

        var isClientFile = fileContent.Length == ClientHeaderSize + TileCount;
        var isExtendedClientFile = fileContent.Length == ClientHeaderSize + (TileCount * sizeof(ushort));
        if (!isClientFile && !isExtendedClientFile)
        {
            return false;
        }

        var decrypted = fileContent.ToArray();
        DecryptMapFile(decrypted);
        ApplyBuxKey(decrypted);
        if (decrypted[0] != 0 || decrypted[2] != MaximumCoordinate || decrypted[3] != MaximumCoordinate)
        {
            return false;
        }

        var data = decrypted.AsSpan(ClientHeaderSize);
        if (isClientFile)
        {
            tileAttributes = data.ToArray();
        }
        else
        {
            // Two bytes per tile (little endian), of which the client just uses the low byte.
            tileAttributes = new byte[TileCount];
            for (int i = 0; i < TileCount; i++)
            {
                tileAttributes[i] = data[i * sizeof(ushort)];
            }
        }

        return true;
    }

    private static void ValidateTileAttributes(ReadOnlySpan<byte> tileAttributes)
    {
        if (tileAttributes.Length != TileCount)
        {
            throw new ArgumentException($"The terrain must have exactly {TileCount} tiles, but has {tileAttributes.Length}.", nameof(tileAttributes));
        }
    }

    private static void ApplyBuxKey(Span<byte> data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= BuxKey[i % BuxKey.Length];
        }
    }

    private static void EncryptMapFile(Span<byte> data)
    {
        byte mapKey = InitialMapKey;
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)((data[i] + mapKey) ^ MapXorKey[i % MapXorKey.Length]);
            mapKey = (byte)(data[i] + MapKeyIncrement);
        }
    }

    private static void DecryptMapFile(Span<byte> data)
    {
        byte mapKey = InitialMapKey;
        for (int i = 0; i < data.Length; i++)
        {
            var encrypted = data[i];
            data[i] = (byte)((encrypted ^ MapXorKey[i % MapXorKey.Length]) - mapKey);
            mapKey = (byte)(encrypted + MapKeyIncrement);
        }
    }
}
