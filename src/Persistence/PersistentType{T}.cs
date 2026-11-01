// <copyright file="PersistentType{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// A persistent type of a persistence model.
/// </summary>
/// <typeparam name="T">The persistent type.</typeparam>
public sealed class PersistentType<T> : PersistentType
    where T : class
{
    private readonly Func<T>? _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentType{T}"/> class.
    /// </summary>
    /// <param name="factory">The factory which calls the public parameterless constructor; <c>null</c>, if there is none.</param>
    public PersistentType(Func<T>? factory)
        : base(typeof(T))
    {
        this._factory = factory;
    }

    /// <inheritdoc />
    public override object? CreateInstance() => this._factory?.Invoke();

    /// <inheritdoc />
    public override TResult Accept<TResult>(IPersistentTypeVisitor<TResult> visitor) => visitor.Visit<T>();
}
