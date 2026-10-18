// <copyright file="SimplePngEncoder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;

/// <summary>
/// A minimal, dependency-free PNG encoder for 8-bit RGB images.
/// It's sufficient for rendering the small, flat-colored terrain images
/// and avoids the need of a full imaging library.
/// </summary>
internal static class SimplePngEncoder
{
    private static readonly byte[] Signature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

    private static readonly uint[] CrcTable = CreateCrcTable();

    /// <summary>
    /// Encodes the specified raw 8-bit RGB pixel data as PNG.
    /// </summary>
    /// <param name="rgb">The pixel data, 3 bytes per pixel (R, G, B), row by row, without padding.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>The PNG encoded image.</returns>
    public static byte[] EncodeRgb(byte[] rgb, int width, int height)
    {
        var stride = width * 3;
        if (rgb.Length != stride * height)
        {
            throw new ArgumentException("The pixel data length does not match the image dimensions.", nameof(rgb));
        }

        using var output = new MemoryStream();
        output.Write(Signature);

        Span<byte> header = stackalloc byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header, width);
        BinaryPrimitives.WriteInt32BigEndian(header[4..], height);
        header[8] = 8; // bit depth
        header[9] = 2; // color type: truecolor (RGB)
        header[10] = 0; // compression method
        header[11] = 0; // filter method
        header[12] = 0; // interlace method: none
        WriteChunk(output, "IHDR"u8, header);

        using var raw = new MemoryStream();
        using (var zlib = new ZLibStream(raw, CompressionLevel.Optimal, leaveOpen: true))
        {
            for (int y = 0; y < height; y++)
            {
                zlib.WriteByte(0); // filter type "None" for each scanline
                zlib.Write(rgb, y * stride, stride);
            }
        }

        WriteChunk(output, "IDAT"u8, raw.ToArray());
        WriteChunk(output, "IEND"u8, ReadOnlySpan<byte>.Empty);
        return output.ToArray();
    }

    private static void WriteChunk(Stream output, ReadOnlySpan<byte> type, ReadOnlySpan<byte> data)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(buffer, data.Length);
        output.Write(buffer);
        output.Write(type);
        output.Write(data);

        var crc = UpdateCrc(0xFFFFFFFF, type);
        crc = UpdateCrc(crc, data) ^ 0xFFFFFFFF;
        BinaryPrimitives.WriteUInt32BigEndian(buffer, crc);
        output.Write(buffer);
    }

    private static uint UpdateCrc(uint crc, ReadOnlySpan<byte> data)
    {
        foreach (var b in data)
        {
            crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
        }

        return crc;
    }

    private static uint[] CreateCrcTable()
    {
        var table = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            var c = n;
            for (int k = 0; k < 8; k++)
            {
                c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
            }

            table[n] = c;
        }

        return table;
    }
}
