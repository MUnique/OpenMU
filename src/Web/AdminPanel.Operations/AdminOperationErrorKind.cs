// <copyright file="AdminOperationErrorKind.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Classifies safe operational errors for HTTP clients.</summary>
public enum AdminOperationErrorKind
{
    /// <summary>The requested operation conflicts with the current state.</summary>
    Conflict,

    /// <summary>The caller lacks authorization.</summary>
    Forbidden,

    /// <summary>The request contains invalid operational arguments.</summary>
    InvalidInput,

    /// <summary>A required service is unavailable and no action was executed.</summary>
    Unavailable,

    /// <summary>The action may have completed; inspect state before retrying.</summary>
    Uncertain,
}
