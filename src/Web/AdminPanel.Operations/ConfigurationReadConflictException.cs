// <copyright file="ConfigurationReadConflictException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Signals that a shared configuration collection changed while it was read.</summary>
/// <param name="innerException">The collection enumeration failure.</param>
public sealed class ConfigurationReadConflictException(InvalidOperationException innerException)
    : Exception("Configuration is being edited. Retry this read after the edit completes.", innerException);
