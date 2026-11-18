// <copyright file="ExposesOperationAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Mcp;

/// <summary>Links a tool to an admin operation for parity and role checks.</summary>
/// <param name="serviceType">The service type.</param>
/// <param name="methodName">The method name.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ExposesOperationAttribute(Type serviceType, string methodName) : Attribute
{
    /// <summary>Gets the operations service type.</summary>
    public Type ServiceType { get; } = serviceType;

    /// <summary>Gets the name of the operation.</summary>
    public string MethodName { get; } = methodName;
}
