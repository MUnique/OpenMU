// <copyright file="IsLinkToParentAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Attribute to mark properties which are a link to the parent object.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class IsLinkToParentAttribute : Attribute
{
}