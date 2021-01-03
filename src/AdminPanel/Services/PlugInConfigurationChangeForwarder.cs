// <copyright file="PlugInConfigurationChangeForwarder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A class which forwards events of the <see cref="ISupportPlugInConfigurationChangedNotification"/> to a <see cref="IPlugInConfigurationChangeListener"/>.
    /// </summary>
    public sealed class PlugInConfigurationChangeForwarder : IDisposable
    {
        private readonly ISupportPlugInConfigurationChangedNotification notifier;
        private readonly IPlugInConfigurationChangeListener listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlugInConfigurationChangeForwarder"/> class.
        /// </summary>
        /// <param name="notifier">The controller.</param>
        /// <param name="listener">The listener.</param>
        public PlugInConfigurationChangeForwarder(ISupportPlugInConfigurationChangedNotification notifier, IPlugInConfigurationChangeListener listener)
        {
            this.notifier = notifier;
            this.listener = listener;

            this.notifier.PlugInActivated += this.OnPlugInActivated;
            this.notifier.PlugInDeactivated += this.OnPlugInDeactivated;
            this.notifier.PlugInConfigurationChanged += this.OnPlugInConfigurationChanged;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.notifier.PlugInActivated -= this.OnPlugInActivated;
            this.notifier.PlugInDeactivated -= this.OnPlugInDeactivated;
            this.notifier.PlugInConfigurationChanged -= this.OnPlugInConfigurationChanged;
        }

        private void OnPlugInConfigurationChanged(object? sender, (Guid, PlugInConfiguration) args)
        {
            var (id, config) = args;
            this.listener.PlugInConfigured(id, config);
        }

        private void OnPlugInActivated(object? sender, Guid id)
        {
            this.listener.PlugInActivated(id);
        }

        private void OnPlugInDeactivated(object? sender, Guid id)
        {
            this.listener.PlugInDeactivated(id);
        }
    }
}