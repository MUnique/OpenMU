// <copyright file="PipelinedEncryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.IO.Pipelines;
    using MUnique.OpenMU.Network.SimpleModulus;

    /// <summary>
    /// Default server implementation of a <see cref="IPipelinedEncryptor"/>.
    /// It encrypts 0xC3 and 0xC4-packets with the "simple modulus" algorithm and writes through every other packet types.
    /// </summary>
    public class PipelinedEncryptor : PipelinedSimpleModulusEncryptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedEncryptor"/> class.
        /// </summary>
        /// <param name="target">The target pipe writer.</param>
        public PipelinedEncryptor(PipeWriter target)
            : base(target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedEncryptor"/> class.
        /// </summary>
        /// <param name="target">The target pipe writer.</param>
        /// <param name="encryptionKeys">The encryption keys.</param>
        public PipelinedEncryptor(PipeWriter target, uint[] encryptionKeys)
            : base(target, encryptionKeys)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedEncryptor"/> class.
        /// </summary>
        /// <param name="target">The target pipe writer.</param>
        /// <param name="encryptionKeys">The encryption keys.</param>
        public PipelinedEncryptor(PipeWriter target, SimpleModulusKeys encryptionKeys)
            : base(target, encryptionKeys)
        {
        }
    }
}
