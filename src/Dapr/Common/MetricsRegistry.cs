// <copyright file="MetricsRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

/// <summary>
/// Registry for metrics which should be exposed by the application.
/// </summary>
public class MetricsRegistry
{
    private readonly HashSet<string> _meters = new HashSet<string>();

    /// <summary>
    /// Gets the registered meters.
    /// </summary>
    public IEnumerable<string> Meters => this._meters;

    /// <summary>
    /// Adds the meter with the specified name.
    /// </summary>
    /// <param name="meterName">Name of the meter.</param>
    public void AddMeter(string meterName)
    {
        this._meters.Add(meterName);
    }

    /// <summary>
    /// Adds the meters with the specified names.
    /// </summary>
    /// <param name="meterNames">The names of the meters.</param>
    public void AddMeters(IEnumerable<string> meterNames)
    {
        foreach (var meter in meterNames)
        {
            this.AddMeter(meter);
        }
    }

    /// <summary>
    /// Adds the network meters.
    /// </summary>
    public void AddNetworkMeters()
    {
        this.AddMeters(MUnique.OpenMU.Network.Metrics.Meters);
    }
}