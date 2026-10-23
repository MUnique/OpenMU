// <copyright file="AdminOperationAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>Declares the minimum role of a UI independent admin operation.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class AdminOperationAttribute(AdminRole role) : Attribute
{
    /// <summary>Gets the minimum role required to run the operation.</summary>
    public AdminRole Role { get; } = role;
}
