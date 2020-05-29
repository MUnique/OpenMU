// <copyright file="TextReaderExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Nito.AsyncEx;

    /// <summary>
    /// Extensions for a <see cref="TextReader"/>.
    /// </summary>
    public static class TextReaderExtensions
    {
        /// <summary>
        /// Reads the line asynchronously and considers the <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The read line or <c>null</c>.</returns>
        public static async Task<string> ReadLineAsync(this TextReader reader, CancellationToken cancellationToken)
        {
            using var taskSource = new CancellationTokenTaskSource<string>(cancellationToken);
            var result = await await Task.WhenAny(taskSource.Task, reader.ReadLineAsync());
            return result;
        }
    }
}