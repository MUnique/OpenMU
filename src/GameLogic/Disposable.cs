// <copyright file="Disposable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;

    /// <summary>
    /// Base class for a disposable class.
    /// </summary>
    public class Disposable : IDisposable
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="Disposable"/> class.
        /// </summary>
        ~Disposable()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is disposing; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposing { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed && !this.IsDisposing)
            {
                this.IsDisposing = true;
                try
                {
                    this.Dispose(true);
                    GC.SuppressFinalize(this);
                    this.IsDisposed = true;
                }
                finally
                {
                    this.IsDisposing = false;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}