// <copyright file="ReferenceConfigurationFailedException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

/// <summary>
/// The exception which <see cref="ReferenceConfiguration"/> throws when the configuration of a data initialization can't be created
/// in this process, or a value can't be derived from it, e.g. because the data initialization isn't available.
/// Such a failure can't heal, so it's remembered and thrown again without a new attempt; the inner exception is the original one.
/// </summary>
public sealed class ReferenceConfigurationFailedException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceConfigurationFailedException"/> class.
    /// </summary>
    /// <param name="initializationKey">The key of the data initialization.</param>
    /// <param name="innerException">The original failure.</param>
    public ReferenceConfigurationFailedException(string initializationKey, Exception innerException)
        : base(innerException.Message, innerException)
    {
        this.InitializationKey = initializationKey;
    }

    /// <summary>
    /// Gets the key of the data initialization.
    /// </summary>
    public string InitializationKey { get; }
}
