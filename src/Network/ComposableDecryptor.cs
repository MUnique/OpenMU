// <copyright file="ComposableDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Collections.Generic;
    using log4net;

    /// <summary>
    /// A decryptor which can be composed by several other decryptors.
    /// </summary>
    public class ComposableDecryptor : IDecryptor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ComposableDecryptor));

        private readonly ICollection<IDecryptor> decryptors = new List<IDecryptor>(2);

        /// <inheritdoc />
        public bool Decrypt(ref byte[] packet)
        {
            try
            {
                foreach (var decryptor in this.decryptors)
                {
                    if (!decryptor.Decrypt(ref packet))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message, exception);
                return false;
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            foreach (var decryptor in this.decryptors)
            {
                decryptor.Reset();
            }
        }

        /// <summary>
        /// Adds the decryptor at the end of the decryption chain.
        /// </summary>
        /// <param name="decryptor">The decryptor.</param>
        /// <returns>This instance, to allow chained calls.</returns>
        public ComposableDecryptor AddDecryptor(IDecryptor decryptor)
        {
            this.decryptors.Add(decryptor);
            return this;
        }
    }
}
