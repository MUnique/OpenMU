// <copyright file="AdminOperationException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>An operational error whose fixed message is safe to show to a caller.</summary>
/// <param name="message">The safe error message.</param>
/// <param name="kind">The operational error category.</param>
/// <param name="innerException">The original failure for server diagnostics, never returned to a caller.</param>
public sealed class AdminOperationException(string message, AdminOperationErrorKind kind = AdminOperationErrorKind.Conflict, Exception? innerException = null) : Exception(message, innerException)
{
    /// <summary>Gets the operational error category.</summary>
    public AdminOperationErrorKind Kind { get; } = kind;
}
