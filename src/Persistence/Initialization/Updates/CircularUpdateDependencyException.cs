// <copyright file="CircularUpdateDependencyException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Exception that is thrown when a circular dependency between updates is detected.
/// </summary>
public sealed class CircularUpdateDependencyException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CircularUpdateDependencyException"/> class.
    /// </summary>
    /// <param name="plugin">The plugin at which the circular dependency was detected.</param>
    public CircularUpdateDependencyException(IConfigurationUpdatePlugIn plugin)
        : base($"A circular dependency was detected involving update '{plugin.Name}'.")
    {
    }
}
