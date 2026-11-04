// <copyright file="PersistentType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// A persistent type of a persistence model, e.g. of the basic model or of the entity framework model,
/// as listed by its generated <see cref="IPersistentTypeRegistry"/>.
/// </summary>
public abstract class PersistentType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentType"/> class.
    /// </summary>
    /// <param name="type">The persistent type.</param>
    protected PersistentType(Type type)
    {
        this.Type = type;
    }

    /// <summary>
    /// Gets the persistent type.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Gets the base type of the <see cref="Type"/>.
    /// </summary>
    public Type? BaseType => this.Type.BaseType;

    /// <summary>
    /// Creates a new instance of the <see cref="Type"/> with its public parameterless constructor.
    /// </summary>
    /// <returns>The new instance; <c>null</c>, if the type has no public parameterless constructor.</returns>
    public abstract object? CreateInstance();

    /// <summary>
    /// Calls the <see cref="IPersistentTypeVisitor{TResult}.Visit{T}"/> of the visitor with the <see cref="Type"/> as type argument.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="visitor">The visitor.</param>
    /// <returns>The result of the visitor.</returns>
    public abstract TResult Accept<TResult>(IPersistentTypeVisitor<TResult> visitor);
}
