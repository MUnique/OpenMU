// <copyright file="IPipelinedEncryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.IO.Pipelines;

    /// <summary>
    /// Interface for a pipelined encryptor.
    /// </summary>
    public interface IPipelinedEncryptor
    {
        /// <summary>
        /// Gets the writer of the encryptor which takes the decrypted data.
        /// The implementation can decide to foward the data written to it
        /// to another pipe.
        /// </summary>
        PipeWriter Writer { get; }
    }
}
