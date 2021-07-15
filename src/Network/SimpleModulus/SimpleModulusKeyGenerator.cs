// <copyright file="SimpleModulusKeyGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System;
    using System.Collections.Generic;
    using System.IO.Pipelines;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// A key generator which is able to generate a new pair of encryption/decryption keys.
    /// </summary>
    public class SimpleModulusKeyGenerator
    {
        private readonly Random randomizer = new ();

        /// <summary>
        /// Generates a new pair of keys.
        /// </summary>
        /// <returns>The generated pair of new keys.</returns>
        public async Task<SimpleModulusKeys> GenerateKeys()
        {
            var result = new SimpleModulusKeys();
            do
            {
                for (int i = 0; i < 4; i++)
                {
                    this.FindKeys(out uint xorKey, out uint modulusKey, out uint encryptKey, out uint decryptKey);
                    result.ModulusKey[i] = modulusKey;
                    result.DecryptKey[i] = decryptKey;
                    result.EncryptKey[i] = encryptKey;
                    result.XorKey[i] = xorKey;
                }
            }
            while (!await this.ValidateResult(result).ConfigureAwait(false));

            return result;
        }

        /// <summary>
        /// Finds the other key when one of the decryption or encryption key is known.
        /// It works in both directions, when the encryption key is known and passed, the decryption key is returned and vice versa.
        /// This greatly demonstrates the "secureness" of this algorithm...
        /// </summary>
        /// <param name="modulusKey">The modulus key.</param>
        /// <param name="cryptKey">The crypt key.</param>
        /// <returns>The other crypt key.</returns>
        public uint[] FindOtherKey(uint[] modulusKey, uint[] cryptKey)
        {
            var result = new uint[modulusKey.Length];
            for (int i = 0; i < modulusKey.Length; i++)
            {
                if (this.TryFindKey(modulusKey[i], cryptKey[i], out uint otherKey))
                {
                    result[i] = otherKey;
                }
                else
                {
                    throw new KeyNotFoundException(modulusKey[i], cryptKey[i]);
                }
            }

            return result;
        }

        private static IEnumerable<uint> GetFactors(uint value)
        {
            var currentValue = value;
            for (uint multiplier = 2; multiplier <= value; multiplier++)
            {
                if (currentValue % multiplier == 0)
                {
                    currentValue /= multiplier;
                    yield return multiplier;
                    multiplier = 1;
                }
            }
        }

        private void FindKeys(out uint xorKey, out uint modulusKey, out uint encryptKey, out uint decryptKey)
        {
            xorKey = 0;
            modulusKey = 0;
            encryptKey = 0;
            decryptKey = 0;

            var count = 0;
            var foundResult = false;
            while (!foundResult)
            {
                if (count % ushort.MaxValue == 0)
                {
                    // we try other keys
                    xorKey = (uint)this.randomizer.Next(0, ushort.MaxValue) + 1;
                    modulusKey = (uint)this.randomizer.Next(0, 0x30000);
                }

                var randomKey = (uint)this.randomizer.Next(2, (int)modulusKey);

                if (this.NumbersAreCoPrime(modulusKey, randomKey) && this.TryFindKey(modulusKey, randomKey, out uint tempDecryptKey))
                {
                    encryptKey = randomKey;
                    decryptKey = tempDecryptKey;
                    foundResult = true;
                }

                count++;
            }
        }

        private bool NumbersAreCoPrime(uint a, uint b)
        {
            var primeFactorsOfA = new SortedSet<uint>(GetFactors(a));
            return !GetFactors(b).Any(primeFactorsOfA.Contains);
        }

        private async Task<bool> ValidateResult(SimpleModulusKeys result)
        {
            return await this.ValidateKeys(result.GetEncryptionKeys(), result.GetDecryptionKeys()).ConfigureAwait(false);
        }

        private async Task<bool> ValidateKeys(uint[] encryptionKeys, uint[] decryptionKeys)
        {
            var packet = Convert.FromBase64String("w7kxFgK8hYpGGLgdXe7ZpTZViB+r3sRI3YSqZs7/Mh5Vmh2mXqs+3dqkvURmXrL57ASs+FkJz/236Tl9ER67R+WZyMLRMkeLF6tEBiB/4X7SsXrKUznES8of73RxwMy76HZezJbvJ7m9IOGuxcjcNwe6q1+k8fOs1Hz3sULSGlbfiB6qIBXo4onADTNYFoYCQrdtthVsF/aDsvcZ93V36gaKzzyqMhby0sjV4+TAU7719W6LZWNAcnA=");
            var pipe = new Pipe();
            var encryptor = new PipelinedSimpleModulusEncryptor(pipe.Writer, encryptionKeys);
            var decryptor = new PipelinedSimpleModulusDecryptor(pipe.Reader, decryptionKeys);
            try
            {
                await encryptor.Writer.WriteAsync(packet).ConfigureAwait(false);
                await pipe.Writer.FlushAsync().ConfigureAwait(false);
                var result = await decryptor.Reader.ReadAsync().ConfigureAwait(false);

                return result.Buffer.Length == packet.Length;
            }
            catch (InvalidBlockChecksumException)
            {
                return false;
            }
            catch (InvalidPacketCounterException)
            {
                return false;
            }
        }

        private bool TryFindKey(uint modulusKey, uint knownKey, out uint unknownKey)
        {
            unknownKey = 0;
            var max = Math.Min(modulusKey, ushort.MaxValue);
            for (uint i = 0; i < max; i++)
            {
                if ((knownKey * i) % modulusKey == 1)
                {
                    unknownKey = i;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Exception which is thrown when a key is not found for the specified modulus and crypt keys.
        /// </summary>
        public class KeyNotFoundException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="KeyNotFoundException" /> class.
            /// </summary>
            /// <param name="modulusKey">The modulus key.</param>
            /// <param name="cryptKey">The crypt key.</param>
            public KeyNotFoundException(uint modulusKey, uint cryptKey)
            : base($"Key could not be found. ModulusKey: {modulusKey}, CryptKey: {cryptKey}")
            {
            }
        }
    }
}
