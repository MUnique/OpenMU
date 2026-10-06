// <copyright file="ItemPriceFormula.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using MUnique.OpenMU.DataModel.Configuration.Items;
using org.mariuszgromada.math.mxparser;

/// <summary>
/// A parsed <see cref="ItemPriceDefinition.BasePriceFormula"/>, which can be evaluated for an item.
/// </summary>
/// <remarks>
/// An mXparser expression keeps the values of its arguments as state, so it's not thread-safe.
/// Each formula is parsed once and evaluated under a lock.
/// </remarks>
internal sealed class ItemPriceFormula
{
    private static readonly ConcurrentDictionary<string, ItemPriceFormula> Cache = new();

    private readonly object _syncRoot = new();
    private readonly string _formula;
    private readonly Expression _expression;
    private readonly Argument _level = new("level", 0);
    private readonly Argument _durability = new("durability", 0);
    private readonly Argument _maxDurability = new("maxDurability", 0);
    private readonly Argument _value = new("value", 0);
    private readonly Argument _dropLevel = new("dropLevel", 0);
    private readonly Argument _optionLevel = new("optionLevel", 0);
    private readonly Argument _optionCount = new("optionCount", 0);
    private readonly Argument _healthRecoveryOptionLevel = new("healthRecoveryOptionLevel", 0);
    private readonly Argument _automaticPrice = new("automaticPrice", 0);

    private ItemPriceFormula(string formula)
    {
        this._formula = formula;
        this._expression = new Expression(
            formula,
            this._level,
            this._durability,
            this._maxDurability,
            this._value,
            this._dropLevel,
            this._optionLevel,
            this._optionCount,
            this._healthRecoveryOptionLevel,
            this._automaticPrice);
    }

    /// <summary>
    /// Gets the parsed formula.
    /// </summary>
    /// <param name="formula">The formula.</param>
    /// <returns>The parsed formula.</returns>
    public static ItemPriceFormula Get(string formula) => Cache.GetOrAdd(formula, f => new ItemPriceFormula(f));

    /// <summary>
    /// Evaluates the formula with the specified variable values.
    /// </summary>
    /// <param name="variables">The values of the variables.</param>
    /// <returns>The result, truncated to an integer.</returns>
    /// <exception cref="InvalidOperationException">The formula can't be evaluated, e.g. because of a syntax error.</exception>
    public long Evaluate(in Variables variables)
    {
        double result;
        lock (this._syncRoot)
        {
            this._level.setArgumentValue(variables.Level);
            this._durability.setArgumentValue(variables.Durability);
            this._maxDurability.setArgumentValue(variables.MaxDurability);
            this._value.setArgumentValue(variables.Value);
            this._dropLevel.setArgumentValue(variables.DropLevel);
            this._optionLevel.setArgumentValue(variables.OptionLevel);
            this._optionCount.setArgumentValue(variables.OptionCount);
            this._healthRecoveryOptionLevel.setArgumentValue(variables.HealthRecoveryOptionLevel);
            this._automaticPrice.setArgumentValue(variables.AutomaticPrice);
            result = this._expression.calculate();
            if (double.IsNaN(result))
            {
                // A price of 0 would let players buy the item for free, so we better fail.
                throw new InvalidOperationException($"The item price formula '{this._formula}' can't be evaluated: {this._expression.getErrorMessage()}");
            }
        }

        return (long)result;
    }

    /// <summary>
    /// The values of the variables of an item price formula.
    /// See <see cref="ItemPriceDefinition.BasePriceFormula"/> for their meaning.
    /// </summary>
    /// <param name="Level">The item level.</param>
    /// <param name="Durability">The durability.</param>
    /// <param name="MaxDurability">The maximum durability of the item definition.</param>
    /// <param name="Value">The value of the item definition.</param>
    /// <param name="DropLevel">The drop level, including the item level and excellent bonus.</param>
    /// <param name="OptionLevel">The level of the normal option.</param>
    /// <param name="OptionCount">The number of normal options.</param>
    /// <param name="HealthRecoveryOptionLevel">The level of the normal option, if it's a health recovery option.</param>
    /// <param name="AutomaticPrice">The automatic equipment price.</param>
    internal readonly record struct Variables(
        long Level,
        long Durability,
        long MaxDurability,
        long Value,
        long DropLevel,
        long OptionLevel,
        long OptionCount,
        long HealthRecoveryOptionLevel,
        long AutomaticPrice);
}
