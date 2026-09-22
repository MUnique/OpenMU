// <copyright file="TestTotpGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Security.Cryptography;

/// <summary>
/// A minimal RFC 6238 implementation which produces the codes an authenticator app would show.
/// </summary>
/// <remarks>
/// The token provider of ASP.NET Core Identity can only validate codes, not generate them, so the
/// tests need their own generator. It uses the standard parameters (SHA-1, 6 digits, 30 seconds),
/// which are the ones the Microsoft Authenticator app uses as well.
/// </remarks>
internal static class TestTotpGenerator
{
    private const int TimeStepSeconds = 30;

    /// <summary>
    /// Generates the code of the current time step for the specified base32 encoded key.
    /// </summary>
    /// <param name="base32Key">The base32 encoded key.</param>
    /// <param name="timeStepOffset">The offset to the current time step.</param>
    /// <returns>The six digit code.</returns>
    public static string Generate(string base32Key, int timeStepOffset = 0)
    {
        var key = DecodeBase32(base32Key);
        var timeStep = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() / TimeStepSeconds) + timeStepOffset;
        var stepBytes = BitConverter.GetBytes(timeStep);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(stepBytes);
        }

        var hash = HMACSHA1.HashData(key, stepBytes);
        var offset = hash[^1] & 0x0f;
        var binaryCode = ((hash[offset] & 0x7f) << 24)
                         | ((hash[offset + 1] & 0xff) << 16)
                         | ((hash[offset + 2] & 0xff) << 8)
                         | (hash[offset + 3] & 0xff);
        return (binaryCode % 1000000).ToString("D6");
    }

    private static byte[] DecodeBase32(string input)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var normalized = input.TrimEnd('=').Replace(" ", string.Empty).ToUpperInvariant();
        var bits = 0;
        var value = 0;
        var result = new List<byte>();
        foreach (var character in normalized)
        {
            var index = alphabet.IndexOf(character, StringComparison.Ordinal);
            if (index < 0)
            {
                throw new FormatException($"'{character}' is not a valid base32 character.");
            }

            value = (value << 5) | index;
            bits += 5;
            if (bits >= 8)
            {
                result.Add((byte)((value >> (bits - 8)) & 0xff));
                bits -= 8;
            }
        }

        return result.ToArray();
    }
}
