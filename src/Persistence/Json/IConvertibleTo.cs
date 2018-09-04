// <copyright file="IConvertibleTo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json
{
    /// <summary>
    /// Interface to support convert an object into another one.
    /// Useful to convert a persistent data model class into the basic model.
    /// </summary>
    /// <typeparam name="TTarget">The target type, e.g. of the basic model.</typeparam>
    public interface IConvertibleTo<out TTarget>
        where TTarget : class
    {
        /// <summary>
        /// Converts this instance into a new instance of <typeparamref name="TTarget"/>.
        /// If this instance is already of type <typeparamref name="TTarget"/>, this instance will be returned.
        /// </summary>
        /// <returns>The converted instance.</returns>
        TTarget Convert();
    }
}
