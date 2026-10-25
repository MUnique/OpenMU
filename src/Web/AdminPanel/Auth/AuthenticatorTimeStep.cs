// <copyright file="AuthenticatorTimeStep.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Buffers.Binary;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Determines the RFC 6238 time step of an authenticator code.
/// </summary>
/// <remarks>
/// The token provider of ASP.NET Core Identity validates a code against the current and the two
/// adjacent time steps in each direction, but it doesn't tell which step matched. To reject a
/// replayed code reliably, the step of the code itself has to be remembered - the current step
/// changes while the code is still accepted. This uses the same parameters as the token provider:
/// SHA-1, 6 digits and 30 seconds per step.
/// </remarks>
internal static class AuthenticatorTimeStep
{
    /// <summary>
    /// The duration of a time step in seconds.
    /// </summary>
    internal const int StepSeconds = 30;

    /// <summary>
    /// The number of steps before and after the current step in which a code is accepted.
    /// </summary>
    internal const int Window = 2;

    private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    /// <summary>
    /// Gets the time step of the specified point in time.
    /// </summary>
    /// <param name="time">The point in time.</param>
    /// <returns>The time step.</returns>
    /// <remarks>Like the token provider, this rounds to whole seconds, so both use the same window.</remarks>
    internal static long GetStep(DateTimeOffset time) => (long)Math.Round((time - DateTimeOffset.UnixEpoch).TotalSeconds) / StepSeconds;

    /// <summary>
    /// Finds the time step around <paramref name="currentStep"/> which produces the specified code.
    /// </summary>
    /// <param name="base32Key">The base32 encoded authenticator key.</param>
    /// <param name="code">The six digit code.</param>
    /// <param name="currentStep">The current time step.</param>
    /// <returns>The latest matching time step, or <c>null</c> if no step within the window matches.</returns>
    internal static long? FindStep(string base32Key, string code, long currentStep)
    {
        var key = DecodeBase32(base32Key);
        var codeBytes = Encoding.ASCII.GetBytes(code);
        long? match = null;
        for (var step = currentStep - Window; step <= currentStep + Window; step++)
        {
            // All steps are checked, so that the time doesn't depend on which one matched.
            if (CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(ComputeCode(key, step)), codeBytes))
            {
                match = step;
            }
        }

        return match;
    }

    /// <summary>
    /// Computes the six digit code of the specified time step.
    /// </summary>
    /// <param name="key">The decoded key.</param>
    /// <param name="step">The time step.</param>
    /// <returns>The code.</returns>
    internal static string ComputeCode(byte[] key, long step)
    {
        Span<byte> stepBytes = stackalloc byte[sizeof(long)];
        BinaryPrimitives.WriteInt64BigEndian(stepBytes, step);
        Span<byte> hash = stackalloc byte[HMACSHA1.HashSizeInBytes];
        HMACSHA1.HashData(key, stepBytes, hash);
        var offset = hash[^1] & 0x0f;
        var binaryCode = ((hash[offset] & 0x7f) << 24)
                         | (hash[offset + 1] << 16)
                         | (hash[offset + 2] << 8)
                         | hash[offset + 3];
        return (binaryCode % 1_000_000).ToString("D6", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Decodes a base32 encoded key, ignoring spaces, padding and the case.
    /// </summary>
    /// <param name="input">The base32 encoded key.</param>
    /// <returns>The decoded key.</returns>
    internal static byte[] DecodeBase32(string input)
    {
        var normalized = input.Replace(" ", string.Empty, StringComparison.Ordinal).TrimEnd('=').ToUpperInvariant();
        var result = new List<byte>(normalized.Length * 5 / 8);
        var bits = 0;
        var value = 0;
        foreach (var character in normalized)
        {
            var index = Base32Alphabet.IndexOf(character, StringComparison.Ordinal);
            if (index < 0)
            {
                throw new FormatException("The authenticator key is not a valid base32 string.");
            }

            value = (value << 5) | index;
            bits += 5;
            if (bits >= 8)
            {
                result.Add((byte)((value >> (bits - 8)) & 0xff));
                bits -= 8;
                value &= (1 << bits) - 1;
            }
        }

        return result.ToArray();
    }
}
