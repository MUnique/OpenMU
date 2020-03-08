// <copyright file="TransientAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Composition
{
    using System;

    /// <summary>
    /// Marks a property as a transient property. That means, that it's not going to get persisted
    /// and just holds some information at run-time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TransientAttribute : Attribute
    {
    }
}