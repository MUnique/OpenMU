// <copyright file="RepositoryNotFoundException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// Description of RepositoryNotFoundException.
/// </summary>
[Serializable]
public class RepositoryNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryNotFoundException"/> class.
    /// </summary>
    public RepositoryNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RepositoryNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public RepositoryNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryNotFoundException"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public RepositoryNotFoundException(Type type)
        : base($"Repository for type {type} not found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryNotFoundException"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="specialRepositoryType">Type of the special repository.</param>
    public RepositoryNotFoundException(Type type, Type specialRepositoryType)
        : base($"Repository for type {type} does not implement {specialRepositoryType}.")
    {
    }
}