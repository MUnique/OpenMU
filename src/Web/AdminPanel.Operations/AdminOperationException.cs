// <copyright file="AdminOperationException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>An operational error whose fixed message is safe to show to a caller.</summary>
/// <param name="message">The safe error message.</param>
/// <param name="forbidden">Whether the caller lacks authorization.</param>
public sealed class AdminOperationException(string message, bool forbidden = false) : Exception(message)
{
    /// <summary>Gets a value indicating whether access was denied.</summary>
    public bool Forbidden { get; } = forbidden;
}
