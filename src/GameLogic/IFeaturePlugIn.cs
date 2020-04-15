// <copyright file="IFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface for features.
    /// A feature plugin can be used to implicitly group dependent plugins.
    /// For example, you can check if a feature plugin is active by calling <see cref="PlugInManager.IsPlugInActive" />.
    /// Additionally, feature plugins can be used as a common configuration sink by implementing <see cref="ISupportCustomConfiguration{TCustomConfig}"/>.
    /// </summary>
    [Guid("D786314A-4168-4FCF-93F8-A350AD0E752E")]
    [PlugInPoint("Feature Plugins", "Feature plugins can group other plugins and provide a configuration.")]
    public interface IFeaturePlugIn
    {
    }
}