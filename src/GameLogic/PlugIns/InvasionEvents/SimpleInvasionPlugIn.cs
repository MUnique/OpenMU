// <copyright file="SimpleInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using MUnique.OpenMU.PlugIns;

/// <summary>
/// Convenience base for invasion plugins that need no logic beyond returning a default configuration.
/// </summary>
public abstract class SimpleInvasionPlugIn
    : BaseInvasionPlugIn<PeriodicInvasionConfiguration>, ISupportDefaultCustomConfiguration
{
    private readonly Func<PeriodicInvasionConfiguration> _defaultConfigFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleInvasionPlugIn"/> class.
    /// </summary>
    /// <param name="defaultConfigFactory">Factory that returns the default configuration.</param>
    protected SimpleInvasionPlugIn(Func<PeriodicInvasionConfiguration> defaultConfigFactory)
    {
        this._defaultConfigFactory = defaultConfigFactory;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => this._defaultConfigFactory();
}