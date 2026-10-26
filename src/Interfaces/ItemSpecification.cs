// <copyright file="ItemSpecification.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>A structured item request, independent of chat command parsing and persistence.</summary>
/// <param name="Group">The item group.</param>
/// <param name="Number">The item number within the group.</param>
/// <param name="Level">The item level.</param>
/// <param name="Skill">Whether to include the item's skill.</param>
/// <param name="Luck">Whether to include luck.</param>
/// <param name="OptionLevel">The additional option level or fixed-option mask.</param>
/// <param name="ExcellentOptions">The excellent option bit mask.</param>
/// <param name="AncientSet">The ancient set discriminator, or zero.</param>
/// <param name="AncientBonusLevel">The ancient bonus level.</param>
public sealed record ItemSpecification(byte Group, short Number, byte Level = 0, bool Skill = false, bool Luck = false, byte OptionLevel = 0, byte ExcellentOptions = 0, byte AncientSet = 0, byte AncientBonusLevel = 1);
