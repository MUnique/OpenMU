// <copyright file="AdminOperationAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>Declares the minimum role of a UI independent admin operation.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class AdminOperationAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="AdminOperationAttribute"/> class for an admin role.</summary>
    /// <param name="role">The minimum role.</param>
    public AdminOperationAttribute(AdminRole role)
        : this(role.ToString())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AdminOperationAttribute"/> class for an independent role.</summary>
    /// <param name="role">The required role name.</param>
    public AdminOperationAttribute(string role) => this.Role = role;

    /// <summary>Gets the minimum role required to run the operation.</summary>
    public string Role { get; }

    /// <summary>Gets or sets a value indicating whether this operation has no side effects.</summary>
    public bool ReadOnly { get; set; } = true;
}
