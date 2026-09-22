// <copyright file="ApiKeyGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Cryptography;

/// <summary>
/// Creates API keys and hashes them for the storage.
/// </summary>
public static class ApiKeyGenerator
{
    /// <summary>
    /// Gets the prefix of every generated key, so it can be recognized as one, e.g. in a log or a
    /// secret scanner.
    /// </summary>
    public static string KeyPrefix => "omu_";

    /// <summary>
    /// Gets the number of leading characters of a key which are stored in plain text, so a key can be
    /// told apart from another one in the admin panel without knowing it.
    /// </summary>
    public static int VisiblePrefixLength => 12;

    private const int SecretByteCount = 32;

    /// <summary>
    /// Creates a new random key.
    /// </summary>
    /// <returns>The key, which is only ever available here and is not recoverable afterwards.</returns>
    public static string GenerateKey()
    {
        var secret = RandomNumberGenerator.GetBytes(SecretByteCount);
        return KeyPrefix + Convert.ToBase64String(secret).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    /// <summary>
    /// Hashes the specified key for the storage.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The base64 encoded SHA-256 hash of the key.</returns>
    /// <remarks>
    /// A plain SHA-256 without a salt is enough here, because a generated key is a random value of
    /// <see cref="SecretByteCount"/> bytes and therefore not guessable. It also has to be computed
    /// on every request of the public API, which rules out a slow hash.
    /// </remarks>
    public static string Hash(string key)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(key)));
    }

    /// <summary>
    /// Gets the visible prefix of the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The first characters of the key.</returns>
    public static string GetVisiblePrefix(string key)
    {
        return key.Length <= VisiblePrefixLength ? key : key[..VisiblePrefixLength];
    }
}
