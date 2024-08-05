// <copyright file="PlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

/// <summary>
/// Configuration for plugins.
/// </summary>
public class PlugInConfiguration : INotifyPropertyChanged
{
    private bool _isActive;
    private string? _customConfiguration;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the type identifier of the plugin.
    /// </summary>
    public Guid TypeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the plugin is active.
    /// </summary>
    public bool IsActive
    {
        get => this._isActive;
        set
        {
            if (value == this._isActive)
            {
                return;
            }

            this._isActive = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the custom plug in source which will be compiled at run-time.
    /// </summary>
    public string? CustomPlugInSource { get; set; }

    /// <summary>
    /// Gets or sets the name of the external assembly which will be loaded at run-time.
    /// </summary>
    public string? ExternalAssemblyName { get; set; }

    /// <summary>
    /// Gets or sets a custom configuration.
    /// </summary>
    public string? CustomConfiguration
    {
        get => this._customConfiguration;
        set
        {
            if (value == this._customConfiguration)
            {
                return;
            }

            this._customConfiguration = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the (display) name of this plugin.
    /// </summary>
    [JsonIgnore]
    public string Name
    {
        get
        {
            var plugInType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.DefinedTypes)
                .FirstOrDefault(t => t.GUID == this.TypeId);
            var plugInAttribute = plugInType?.GetCustomAttribute<PlugInAttribute>();

            return plugInAttribute?.Name ?? this.TypeId.ToString();
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Name;
    }

    /// <summary>
    /// Triggers the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}