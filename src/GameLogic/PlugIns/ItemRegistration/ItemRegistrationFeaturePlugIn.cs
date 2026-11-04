// <copyright file="ItemRegistrationFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ItemRegistration;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Feature plugin providing configuration for item registration NPCs.
/// </summary>
[PlugIn]
[Guid("E3D4C5B6-A789-4012-B345-C67890D1E2F3")]
public class ItemRegistrationFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<ItemRegistrationConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc/>
    public ItemRegistrationConfiguration? Configuration { get; set; }

    /// <inheritdoc/>
    public object CreateDefaultConfig() => new ItemRegistrationConfiguration();
}
