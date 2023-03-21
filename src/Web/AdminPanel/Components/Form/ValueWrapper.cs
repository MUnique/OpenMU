// <copyright file="ValueWrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

/// <summary>
/// A wrapper for <see cref="TValue"/> which allows to be bound to edit components.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public sealed class ValueWrapper<TValue> : INotifyPropertyChanged
    where TValue : struct
{
    private TValue _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueWrapper{TValue}"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="index">The index.</param>
    public ValueWrapper(TValue value, int index)
    {
        this.Value = value;
        this.Index = index;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the index of the original value in a <see cref="ValueListWrapper{TValue}"/>.
    /// </summary>
    [Browsable(false)]
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the value which is wrapped.
    /// </summary>
    /// <remarks>The display name is an empty string by purpose. This way, no label is shown for the edit control.</remarks>
    [Display(Name = "")]
    public TValue Value
    {
        get => this._value;
        set
        {
            if (EqualityComparer<TValue>.Default.Equals(value, this._value))
            {
                return;
            }

            this._value = value;
            this.OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}