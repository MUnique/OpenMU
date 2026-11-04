// <copyright file="ServerHostSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher;

using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// Settings for one server.
/// </summary>
public class ServerHostSettings : INotifyPropertyChanged
{
    private string? _description;
    private string? _address;
    private int _port;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the name of the configuration.
    /// </summary>
    public string? Description
    {
        get => this._description;
        set
        {
            if (value == this._description)
            {
                return;
            }

            this._description = value;
            this.RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the host ip.
    /// </summary>
    public string? Address
    {
        get => this._address;
        set
        {
            if (value == this._address)
            {
                return;
            }

            this._address = value;
            this.RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the host port.
    /// </summary>
    public int Port
    {
        get => this._port;
        set
        {
            if (value == this._port)
            {
                return;
            }

            this._port = value;
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Description} ({this.Address}:{this.Port})";
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}