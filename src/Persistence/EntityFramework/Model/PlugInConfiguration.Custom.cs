// <copyright file="PlugInConfiguration.Custom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Custom additions to the generated <see cref="PlugInConfiguration"/>.
/// </summary>
internal partial class PlugInConfiguration
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{base.ToString()} ({this.Id})";
    }
}