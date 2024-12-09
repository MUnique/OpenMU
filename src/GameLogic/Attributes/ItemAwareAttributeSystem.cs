// <copyright file="ItemAwareAttributeSystem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes;

using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// An attribute system which considers items of a character.
/// </summary>
public sealed class ItemAwareAttributeSystem : AttributeSystem, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemAwareAttributeSystem" /> class.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="character">The character.</param>
    public ItemAwareAttributeSystem(Account account, Character character)
        : base(
            character.Attributes.Concat(account.Attributes),
            character.CharacterClass!.BaseAttributeValues,
            character.CharacterClass.AttributeCombinations)
    {
        this.ItemPowerUps = new Dictionary<Item, IReadOnlyList<PowerUpWrapper>>();
        foreach (var attribute in this)
        {
            this.OnAttributeAdded(attribute);
        }
    }

    /// <summary>
    /// Occurs when an attribute value changed.
    /// </summary>
    public event EventHandler<IAttribute>? AttributeValueChanged;

    /// <summary>
    /// Gets the item power ups.
    /// </summary>
    public IDictionary<Item, IReadOnlyList<PowerUpWrapper>> ItemPowerUps { get; }

    /// <summary>
    /// Gets or sets the item set power ups.
    /// </summary>
    public IReadOnlyList<PowerUpWrapper>? ItemSetPowerUps { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        this.AttributeValueChanged = null;
        foreach (var attribute in this)
        {
            attribute.ValueChanged -= this.OnAttributeValueChanged;
        }

        foreach (var powerUpWrapper in this.ItemPowerUps.SelectMany(p => p.Value))
        {
            powerUpWrapper.Dispose();
        }

        this.ItemPowerUps.Clear();

        if (this.ItemSetPowerUps is { } itemSetPowerUps)
        {
            foreach (var powerUp in itemSetPowerUps)
            {
                powerUp.Dispose();
            }

            this.ItemSetPowerUps = null;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.ToString());
        stringBuilder.AppendLine("Item Power Ups:");
        foreach (var (item, powerUps) in this.ItemPowerUps)
        {
            stringBuilder.Append(" ").AppendLine(item.ToString());
            foreach (var powerUp in powerUps)
            {
                stringBuilder.Append("  ").AppendLine(powerUp.ToString());
            }
        }

        if (this.ItemSetPowerUps != null)
        {
            stringBuilder.AppendLine("Item Set Power Ups:");
            foreach (var attribute in this.ItemSetPowerUps)
            {
                stringBuilder.Append(" ").AppendLine(attribute.ToString());
            }
        }

        return stringBuilder.ToString();
    }

    /// <inheritdoc />
    protected override void OnAttributeAdded(IAttribute attribute)
    {
        base.OnAttributeAdded(attribute);
        attribute.ValueChanged += this.OnAttributeValueChanged;
    }

    /// <inheritdoc />
    protected override void OnAttributeRemoved(IAttribute attribute)
    {
        attribute.ValueChanged -= this.OnAttributeValueChanged;
        base.OnAttributeRemoved(attribute);
    }

    /// <summary>
    /// Called when an attribute value changed. Forwards the event to <see cref="AttributeValueChanged"/>.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void OnAttributeValueChanged(object? sender, EventArgs e)
    {
        if (sender is IAttribute attribute)
        {
            this.AttributeValueChanged?.Invoke(this, attribute);
        }
    }
}