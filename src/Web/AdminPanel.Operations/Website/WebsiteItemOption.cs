// <copyright file="WebsiteItemOption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// A structured option of an equipped item, as a website shows it.
/// </summary>
/// <param name="Type">The neutral name of the option type, e.g. "Excellent Option", "Luck (Critical Damage Chance 5%)", "Option", "Wing Option", "Socket Option" or "Harmony Option".</param>
/// <param name="Number">The number of the option within its option type.</param>
/// <param name="Level">The level of the option link, e.g. the level of the regular "+option".</param>
/// <param name="Attribute">The designation of the target attribute of the power-up; the level dependent one for <paramref name="Level"/> if present, else the own one of the option.</param>
/// <param name="Value">The constant value of the boost; <c>null</c>, if there is no power-up.</param>
/// <param name="Aggregate">The name of the aggregate type of the boost, e.g. "AddRaw", "Multiplicate" or "AddFinal".</param>
/// <param name="RelatedAttribute">The designation of the input attribute of the first related value, if the boost is relative, e.g. a value per level or a fraction of the maximum health.</param>
/// <param name="RelatedOperator">The name of the input operator of the first related value, e.g. "Multiply".</param>
/// <param name="RelatedOperand">The operand of the first related value, e.g. the factor of the input attribute.</param>
public sealed record WebsiteItemOption(
    string Type,
    int Number,
    int Level,
    string? Attribute,
    float? Value,
    string? Aggregate,
    string? RelatedAttribute,
    string? RelatedOperator,
    float? RelatedOperand);
