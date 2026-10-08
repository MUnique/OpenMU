// <copyright file="PlugInConfigurationTypedContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.TypedContexts;

/// <summary>
/// The <see cref="TypedContext"/> for <see cref="MUnique.OpenMU.PlugIns.PlugInConfiguration"/>, which uses a compiled model.
/// </summary>
internal sealed class PlugInConfigurationTypedContext : TypedContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlugInConfigurationTypedContext"/> class.
    /// </summary>
    public PlugInConfigurationTypedContext()
        : base(typeof(MUnique.OpenMU.PlugIns.PlugInConfiguration))
    {
    }
}
