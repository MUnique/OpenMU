// <copyright file="MoneyRateEditor.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Configuration;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Edits the global money rate of a <see cref="GameConfiguration"/>.
/// </summary>
/// <remarks>
/// The rate is the value of the global <see cref="Stats.MoneyAmountRate"/> attribute, which is applied
/// to the money which monsters drop. It gets its own field, because it is one of the rates a server
/// operator looks for - but it stays a base attribute value of the configuration, so no separate
/// configuration property (and database column) is needed for it.
/// </remarks>
public partial class MoneyRateEditor
{
    private ConstValueAttribute? _moneyRate;

    private double _value;

    /// <summary>
    /// Gets or sets the configuration which contains the money rate.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public GameConfiguration Configuration { get; set; } = null!;

    /// <summary>
    /// Gets or sets the callback which saves the changed configuration.
    /// </summary>
    [Parameter]
    public EventCallback OnSave { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        this._moneyRate = this.Configuration.GlobalBaseAttributeValues
            .FirstOrDefault(attribute => attribute.Definition?.Id == Stats.MoneyAmountRate.Id);
        this._value = this._moneyRate?.Value ?? 1.0;
    }

    private async Task ApplyAsync()
    {
        if (this._moneyRate is not { } moneyRate || moneyRate is not IAssignable assignable)
        {
            return;
        }

        if (double.IsNaN(this._value) || double.IsInfinity(this._value) || this._value < 0)
        {
            // Show the stored value again, instead of saving an invalid rate.
            this._value = moneyRate.Value;
            return;
        }

        if (moneyRate.Value == (float)this._value)
        {
            return;
        }

        // The value of a constant attribute can't be set directly; assigning the values of another
        // instance is the seam which the admin operations use for it, too.
        var changedValue = new ConstValueAttribute((float)this._value, moneyRate.Definition, moneyRate.AggregateType);
        assignable.AssignValuesOf(changedValue, this.Configuration);
        await this.OnSave.InvokeAsync().ConfigureAwait(true);
    }
}
