// <copyright file="UpdateDependency.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Describes the dependency of a configuration update on another update.
/// </summary>
public readonly struct UpdateDependency
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDependency"/> struct.
    /// </summary>
    /// <param name="key">The key of the required update.</param>
    /// <param name="minVersion">The minimum required version of the update, or <c>null</c> for any version.</param>
    public UpdateDependency(Guid key, int? minVersion = null)
    {
        this.Key = key;
        this.MinVersion = minVersion;
    }

    /// <summary>
    /// Gets the key of the required update.
    /// </summary>
    public Guid Key { get; }

    /// <summary>
    /// Gets the minimum required version of the update, or <c>null</c> for any version.
    /// </summary>
    public int? MinVersion { get; }

    /// <summary>
    /// Converts an update key into a dependency on any version of that update.
    /// </summary>
    /// <param name="key">The key of the required update.</param>
    public static implicit operator UpdateDependency(Guid key) => new(key);

    /// <summary>
    /// Converts a key and minimum version into a dependency.
    /// </summary>
    /// <param name="dependency">The key and minimum version of the required update.</param>
    public static implicit operator UpdateDependency((Guid Key, int MinVersion) dependency) => new(dependency.Key, dependency.MinVersion);
}
