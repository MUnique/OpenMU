// <copyright file="IComposableAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    using System.Collections.Generic;

    /// <summary>
    /// The Attribute... Hint: every attribute could be wrapped into an element to chain them.
    /// Example: We have a base strength attribute which could be wrapped into an element,
    ///          and this element could be added to a total strength attribute, which will
    ///          also contain the elements gotten from master tree or ancient items.
    ///          This way we could identify the base stats, which are needed for the character stats packet.
    ///          Another use could define an aggregate function (either addition or multiplication), and again
    ///          we could chain this calculations together (keeping commutative property).
    /// </summary>
    public interface IComposableAttribute : IAttribute
    {
        /// <summary>
        /// Gets the elements, of which this attribute is calculated.
        /// </summary>
        IEnumerable<IElement> Elements { get; }

        /// <summary>
        /// Adds the element to the composition.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The composable attribute itself, to be able to chain adding.</returns>
        IComposableAttribute AddElement(IElement element);

        /// <summary>
        /// Removes the element from the composition.
        /// </summary>
        /// <param name="element">The element.</param>
        void RemoveElement(IElement element);
    }
}
