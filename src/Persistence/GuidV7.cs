// <copyright file="GuidHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Security.Cryptography;

/// <summary>
/// A generator for version 7 GUIDs. Layout:
/// The first 6 bytes is a unix timestamp in milliseconds.
/// The 7th byte contains the version number (7) in the upper 4 bits.
/// The rest is random.
/// </summary>
public static class GuidV7
{
    public static Guid NewGuid() => NewGuid(DateTimeOffset.UtcNow);

    public static Guid NewGuid(DateTimeOffset dateTimeOffset)
    {
        // We create a buffer which is two bytes bigger than the Guid,
        // because we don't need the first two bytes of the timestamp.
        Span<byte> buffer = stackalloc byte[18];
        var uuidAsBytes = buffer[2..];
        var currentTimestamp = dateTimeOffset.ToUnixTimeMilliseconds();
        Span<byte> timestampBytes = stackalloc byte[sizeof(long)];
        if (!BitConverter.TryWriteBytes(buffer, currentTimestamp))
        {
            throw new InvalidOperationException("Could not convert the timestamp to bytes.");
        }

        RandomNumberGenerator.Fill(uuidAsBytes[6..]);

        uuidAsBytes[6] &= 0x0F;
        uuidAsBytes[6] |= 0x70;

        return new Guid(uuidAsBytes, true);
    }
}