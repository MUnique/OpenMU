// <copyright file="SelfDefensePlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="SelfDefensePlugIn"/>.
/// </summary>
public class SelfDefensePlugInConfiguration
{
    /// <summary>
    /// Gets or sets the self defense timeout.
    /// </summary>
    public TimeSpan SelfDefenseTimeOut { get; set; } = TimeSpan.FromMinutes(1);
}