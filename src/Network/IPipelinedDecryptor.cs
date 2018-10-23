// <copyright file="IPipelinedDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.IO.Pipelines;

    /// <summary>
    /// Interface for a pipelined decryptor.
    /// </summary>
    public interface IPipelinedDecryptor
    {
        /// <summary>
        /// Gets the reader of the decryptor which contains the decrypted data.
        /// </summary>
        PipeReader Reader { get; }
    }
}
