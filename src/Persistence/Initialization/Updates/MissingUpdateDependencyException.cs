// <copyright file="MissingUpdateDependencyException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Exception that is thrown when an update depends on other updates which are neither installed nor available.
/// </summary>
public sealed class MissingUpdateDependencyException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingUpdateDependencyException"/> class.
    /// </summary>
    /// <param name="plugin">The plugin with the missing dependencies.</param>
    /// <param name="missingDependencies">The missing dependencies.</param>
    public MissingUpdateDependencyException(IConfigurationUpdatePlugIn plugin, IReadOnlyList<Guid> missingDependencies)
        : base($"Update '{plugin.Name}' depends on update(s) that are neither installed nor available: {string.Join(", ", missingDependencies)}.")
    {
    }
}
