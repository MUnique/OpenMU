// <copyright file="SimpleModulusKeySerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System.IO;

    /// <summary>
    /// Serializer to load/save the simple modulus keys into files in the original format of Webzen.
    /// </summary>
    public class SimpleModulusKeySerializer
    {
        private readonly uint[] encryptionKeys = { 0x3F08A79B, 0xE25CC287, 0x93D27AB9, 0x20DEA7BF }; // They're used to "encrypt" and "decrypt" (XOR) the encryption keys to dat files.
        private readonly byte[] header = { 0x12, 0x11, 0x36, 0, 0, 0 }; // file header, always the same

        /// <summary>
        /// Serializes (saves) the specified keys to the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="modKey">The mod key.</param>
        /// <param name="key">The key.</param>
        /// <param name="xorKey">The xor key.</param>
        public void Serialize(string fileName, uint[] modKey, uint[] key, uint[] xorKey)
        {
            using (var fileStream = File.OpenWrite(fileName))
            {
                fileStream.Write(this.header, 0, this.header.Length);
                for (int i = 0; i < 4; i++)
                {
                    var value = modKey[i] ^ this.encryptionKeys[i];
                    fileStream.Write(value.ToBytesBigEndian(), 0, 4);
                }

                for (int i = 0; i < 4; i++)
                {
                    var value = key[i] ^ this.encryptionKeys[i];
                    fileStream.Write(value.ToBytesBigEndian(), 0, 4);
                }

                for (int i = 0; i < 4; i++)
                {
                    var value = xorKey[i] ^ this.encryptionKeys[i];
                    fileStream.Write(value.ToBytesBigEndian(), 0, 4);
                }

                fileStream.Flush(true);
            }
        }

        /// <summary>
        /// Deserializes (loads) the keys from the specified file.
        /// </summary>
        /// <param name="fileName">Name (and path) of the file.</param>
        /// <param name="modulusKey">The modulus key.</param>
        /// <param name="key">The key.</param>
        /// <param name="xorKey">The xor key.</param>
        /// <returns><c>True</c>, if successful; Otherwise, <c>false</c>.</returns>
        public bool TryDeserialize(string fileName, out uint[] modulusKey, out uint[] key, out uint[] xorKey)
        {
            var buffer = new byte[(12 * sizeof(uint)) + this.header.Length];
            using (var fileStream = File.OpenRead(fileName))
            {
                if (fileStream.Read(buffer, 0, buffer.Length) != buffer.Length)
                {
                    modulusKey = null;
                    key = null;
                    xorKey = null;
                    return false;
                }
            }

            var offset = this.header.Length;
            modulusKey = new[]
            {
                buffer.MakeDwordBigEndian(offset) ^ this.encryptionKeys[0],
                buffer.MakeDwordBigEndian(offset + 4) ^ this.encryptionKeys[1],
                buffer.MakeDwordBigEndian(offset + 8) ^ this.encryptionKeys[2],
                buffer.MakeDwordBigEndian(offset + 12) ^ this.encryptionKeys[3]
            };
            offset += 16;
            key = new[]
            {
                buffer.MakeDwordBigEndian(offset) ^ this.encryptionKeys[0],
                buffer.MakeDwordBigEndian(offset + 4) ^ this.encryptionKeys[1],
                buffer.MakeDwordBigEndian(offset + 8) ^ this.encryptionKeys[2],
                buffer.MakeDwordBigEndian(offset + 12) ^ this.encryptionKeys[3]
            };
            offset += 16;
            xorKey = new[]
            {
                buffer.MakeDwordBigEndian(offset) ^ this.encryptionKeys[0],
                buffer.MakeDwordBigEndian(offset + 4) ^ this.encryptionKeys[1],
                buffer.MakeDwordBigEndian(offset + 8) ^ this.encryptionKeys[2],
                buffer.MakeDwordBigEndian(offset + 12) ^ this.encryptionKeys[3]
            };

            return true;
        }
    }
}