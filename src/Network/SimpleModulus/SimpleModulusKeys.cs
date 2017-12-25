// <copyright file="SimpleModulusKeys.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System.Linq;

    /// <summary>
    /// Class to hold encryptions keys for the simple modulus algorithm.
    /// </summary>
    public class SimpleModulusKeys
    {
        /// <summary>
        /// Gets the modulus key. These values are used for the modulus operation in the encryption/decryption process.
        /// </summary>
        public uint[] ModulusKey { get; } = { 0, 0, 0, 0 };

        /// <summary>
        /// Gets the xor key. These values are used for a xor operation in the encryption/decryption process.
        /// </summary>
        public uint[] XorKey { get; } = { 0, 0, 0, 0 };

        /// <summary>
        /// Gets the encryption key. These values are used for the multiplication operation in the encryption process.
        /// </summary>
        public uint[] EncryptKey { get; } = { 0, 0, 0, 0 };

        /// <summary>
        /// Gets the decrypt key. These values are used for the multiplication operation in the encryption process.
        /// </summary>
        public uint[] DecryptKey { get; } = { 0, 0, 0, 0 };

        /// <summary>
        /// Creates an instance of <see cref="SimpleModulusKeys"/> with the crypt key as <see cref="DecryptKey"/>.
        /// </summary>
        /// <param name="decryptionKey">The decryption key with 12 integers.</param>
        /// <returns>An instance of <see cref="SimpleModulusKeys"/> with the crypt key as <see cref="DecryptKey"/>.</returns>
        public static SimpleModulusKeys CreateDecryptionKeys(uint[] decryptionKey)
        {
            var keys = new SimpleModulusKeys();
            keys.ModulusKey[0] = decryptionKey[0];
            keys.ModulusKey[1] = decryptionKey[1];
            keys.ModulusKey[2] = decryptionKey[2];
            keys.ModulusKey[3] = decryptionKey[3];
            keys.DecryptKey[0] = decryptionKey[4];
            keys.DecryptKey[1] = decryptionKey[5];
            keys.DecryptKey[2] = decryptionKey[6];
            keys.DecryptKey[3] = decryptionKey[7];
            keys.XorKey[0] = decryptionKey[8];
            keys.XorKey[1] = decryptionKey[9];
            keys.XorKey[2] = decryptionKey[10];
            keys.XorKey[3] = decryptionKey[11];
            return keys;
        }

        /// <summary>
        /// Creates an instance of <see cref="SimpleModulusKeys"/> with the crypt key as <see cref="EncryptKey"/>.
        /// </summary>
        /// <param name="encryptionKey">The decryption key with 12 integers.</param>
        /// <returns>An instance of <see cref="SimpleModulusKeys"/> with the crypt key as <see cref="EncryptKey"/>.</returns>
        public static SimpleModulusKeys CreateEncryptionKeys(uint[] encryptionKey)
        {
            var keys = new SimpleModulusKeys();
            keys.ModulusKey[0] = encryptionKey[0];
            keys.ModulusKey[1] = encryptionKey[1];
            keys.ModulusKey[2] = encryptionKey[2];
            keys.ModulusKey[3] = encryptionKey[3];
            keys.EncryptKey[0] = encryptionKey[4];
            keys.EncryptKey[1] = encryptionKey[5];
            keys.EncryptKey[2] = encryptionKey[6];
            keys.EncryptKey[3] = encryptionKey[7];
            keys.XorKey[0] = encryptionKey[8];
            keys.XorKey[1] = encryptionKey[9];
            keys.XorKey[2] = encryptionKey[10];
            keys.XorKey[3] = encryptionKey[11];
            return keys;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Encryption Key: {this.ModulusKey[0]}, {this.ModulusKey[1]}, {this.ModulusKey[2]}, {this.ModulusKey[3]}, {this.EncryptKey[0]}, {this.EncryptKey[1]}, {this.EncryptKey[2]}, {this.EncryptKey[3]}, {this.XorKey[0]}, {this.XorKey[1]}, {this.XorKey[2]}, {this.XorKey[3]}"
                   + $"\nDecryption Key: {this.ModulusKey[0]}, {this.ModulusKey[1]}, {this.ModulusKey[2]}, {this.ModulusKey[3]}, {this.DecryptKey[0]}, {this.DecryptKey[1]}, {this.DecryptKey[2]}, {this.DecryptKey[3]}, {this.XorKey[0]}, {this.XorKey[1]}, {this.XorKey[2]}, {this.XorKey[3]}";
        }

        /// <summary>
        /// Gets the encryption key values for the <see cref="SimpleModulusEncryptor"/>.
        /// </summary>
        /// <returns>The encryption key values for the <see cref="SimpleModulusEncryptor"/>.</returns>
        public uint[] GetEncryptionKeys()
        {
            return this.ModulusKey.Concat(this.EncryptKey).Concat(this.XorKey).ToArray();
        }

        /// <summary>
        /// Gets the decryption key values for the <see cref="SimpleModulusDecryptor"/>.
        /// </summary>
        /// <returns>The decryption key values for the <see cref="SimpleModulusDecryptor"/>.</returns>
        public uint[] GetDecryptionKeys()
        {
            return this.ModulusKey.Concat(this.DecryptKey).Concat(this.XorKey).ToArray();
        }
    }
}