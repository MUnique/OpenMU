// <copyright file="PlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Configuration for plugins.
    /// </summary>
    public class PlugInConfiguration : INotifyPropertyChanged
    {
        private bool isActive;

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the type identifier of the plugin.
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the plugin is active.
        /// </summary>
        public bool IsActive
        {
            get => this.isActive;
            set
            {
                if (value == this.isActive)
                {
                    return;
                }

                this.isActive = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the custom plug in source which will be compiled at run-time.
        /// </summary>
        public string CustomPlugInSource { get; set; }

        /// <summary>
        /// Gets or sets the name of the external assembly which will be loaded at run-time.
        /// </summary>
        public string ExternalAssemblyName { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var plugInType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.DefinedTypes)
                .FirstOrDefault(t => t.GUID == this.TypeId);
            var plugInAttribute = plugInType?.GetCustomAttribute<PlugInAttribute>();

            return plugInAttribute?.Name ?? this.TypeId.ToString();
        }

        /// <summary>
        /// Triggers the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}