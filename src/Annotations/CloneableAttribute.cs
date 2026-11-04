// <copyright file="CloneableAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Annotations;

/// <summary>
/// Classes marked with this attribute will get a generic Clonable-Interface
/// implemented by a code generator.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CloneableAttribute : Attribute
{
}