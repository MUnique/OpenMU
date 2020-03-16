// <copyright file="Extensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    /// <summary>
    /// Common extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the input operator as string.
        /// </summary>
        /// <param name="inputOperator">The input operator.</param>
        /// <returns>The textual representation of the input operator.</returns>
        public static string AsString(this InputOperator inputOperator)
        {
            switch (inputOperator)
            {
                case InputOperator.Add: return "+";
                case InputOperator.Multiply: return "*";
                case InputOperator.Exponentiate: return "^";
                default: return string.Empty;
            }
        }
    }
}
