// <copyright file="PlugInConfigurationChangeForwarder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services;

using MUnique.OpenMU.PlugIns;

/// <summary>
/// A class which forwards events of the <see cref="ISupportPlugInConfigurationChangedNotification"/> to a <see cref="IPlugInConfigurationChangeListener"/>.
/// </summary>
public sealed class PlugInConfigurationChangeForwarder : IDisposable
{
    private readonly ISupportPlugInConfigurationChangedNotification _notifier;
    private readonly IPlugInConfigurationChangeListener _listener;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlugInConfigurationChangeForwarder"/> class.
    /// </summary>
    /// <param name="notifier">The controller.</param>
    /// <param name="listener">The listener.</param>
    public PlugInConfigurationChangeForwarder(ISupportPlugInConfigurationChangedNotification notifier, IPlugInConfigurationChangeListener listener)
    {
        this._notifier = notifier;
        this._listener = listener;

        this._notifier.PlugInActivated += this.OnPlugInActivated;
        this._notifier.PlugInDeactivated += this.OnPlugInDeactivated;
        this._notifier.PlugInConfigurationChanged += this.OnPlugInConfigurationChanged;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._notifier.PlugInActivated -= this.OnPlugInActivated;
        this._notifier.PlugInDeactivated -= this.OnPlugInDeactivated;
        this._notifier.PlugInConfigurationChanged -= this.OnPlugInConfigurationChanged;
    }

    private void OnPlugInConfigurationChanged(object? sender, (Guid, PlugInConfiguration) args)
    {
        var (id, config) = args;
        this._listener.PlugInConfigured(id, config);
    }

    private void OnPlugInActivated(object? sender, Guid id)
    {
        this._listener.PlugInActivated(id);
    }

    private void OnPlugInDeactivated(object? sender, Guid id)
    {
        this._listener.PlugInDeactivated(id);
    }
}