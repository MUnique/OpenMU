// <copyright file="HackCheckCrypto.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.HackCheck;

/// <summary>
/// Provides HackCheck stream encryption and decryption.
/// </summary>
internal static class HackCheckCrypto
{
    /// <summary>
    /// Encrypts the buffer in-place.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <param name="keys">The keys.</param>
    public static void Encrypt(Span<byte> buffer, HackCheckKeys keys)
    {
        var keyMul = unchecked((byte)(keys.Key1 * keys.Key2));
        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = unchecked((byte)((buffer[i] + keyMul) ^ keys.Key1));
        }
    }

    /// <summary>
    /// Decrypts the buffer in-place.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <param name="keys">The keys.</param>
    public static void Decrypt(Span<byte> buffer, HackCheckKeys keys)
    {
        var keyMul = unchecked((byte)(keys.Key1 * keys.Key2));
        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = unchecked((byte)((buffer[i] ^ keys.Key1) - keyMul));
        }
    }
}
