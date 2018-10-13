// <copyright file="PipelinedDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.IO.Pipelines;
    using MUnique.OpenMU.Network.SimpleModulus;
    using MUnique.OpenMU.Network.Xor;

    /// <summary>
    /// Default server implementation of a <see cref="IPipelinedDecryptor"/> which decrypts incoming data packets.
    /// It decrypts with the "simple modulus" algorithm first, and then with the 32 byte XOR-key.
    /// </summary>
    public class PipelinedDecryptor : PipelinedXor32Decryptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedDecryptor"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public PipelinedDecryptor(PipeReader source)
            : this(source, PipelinedSimpleModulusDecryptor.DefaultServerKey, DefaultKeys.Xor32Key)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedDecryptor"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="decryptionKeys">The decryption keys.</param>
        /// <param name="xor32Key">The xor32 key.</param>
        public PipelinedDecryptor(PipeReader source, SimpleModulusKeys decryptionKeys, byte[] xor32Key)
            : base(new PipelinedSimpleModulusDecryptor(source, decryptionKeys).Reader, xor32Key)
        {
        }
    }
}
