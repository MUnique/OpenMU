// <copyright file="ILoggerOwner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Interface for an object which has a <see cref="ILogger"/>.
    /// </summary>
    public interface ILoggerOwner
    {
        /// <summary>
        /// Gets the logger of this instance.
        /// </summary>
        ILogger Logger { get; }
    }

    /// <summary>
    /// Interface for an object which has a <see cref="ILogger" />.
    /// </summary>
    /// <typeparam name="T">The type of the implementing class.</typeparam>
    public interface ILoggerOwner<out T> : ILoggerOwner
        where T : class
    {
        /// <summary>
        /// Gets the logger of this instance.
        /// </summary>
        new ILogger<T> Logger { get; }
    }
}