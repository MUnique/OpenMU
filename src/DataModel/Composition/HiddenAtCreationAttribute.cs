// <copyright file="HiddenAtCreationAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Composition
{
    using System;

    /// <summary>
    /// Marks a property to be hidden on the UI when the object of the declaring type gets created.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class HiddenAtCreationAttribute : Attribute
    {
    }
}