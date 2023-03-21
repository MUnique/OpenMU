// <copyright file="NotifyableInputBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// An abstract base class which updates it's current value when
/// <see cref="IChangeNotificationService.PropertyChanged"/> is fired for the bound property.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public abstract class NotifyableInputBase<TValue> : InputBase<TValue>
{
    private Func<TValue>? _getter;

    /// <summary>
    /// Gets or sets the notification service.
    /// </summary>
    /// <value>
    /// The notification service.
    /// </value>
    [Inject]
    public IChangeNotificationService NotificationService { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.NotificationService.PropertyChanged += this.OnPropertyChanged;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        this.NotificationService.PropertyChanged -= this.OnPropertyChanged;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        try
        {
            if (sender != this.EditContext.Model)
            {
                return;
            }

            if (args.PropertyName != null && args.PropertyName != this.FieldIdentifier.FieldName)
            {
                return;
            }

            await this.InvokeAsync(() =>
            {
                this._getter ??= this.ValueExpression!.Compile();
                this.CurrentValue = this._getter();
                this.StateHasChanged();
            }).ConfigureAwait(false);
        }
        catch
        {
            // must be catched because it's an async void method.
        }
    }
}