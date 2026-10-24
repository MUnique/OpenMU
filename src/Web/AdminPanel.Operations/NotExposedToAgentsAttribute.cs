// <copyright file="NotExposedToAgentsAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Documents why an operation has no agent equivalent.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class NotExposedToAgentsAttribute(string reason) : Attribute
{
    /// <summary>Gets the reason for excluding this operation.</summary>
    public string Reason { get; } = reason;
}
