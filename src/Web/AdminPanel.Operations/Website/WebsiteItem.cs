// <copyright file="WebsiteItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// An equipped item of a character, as a website shows it.
/// </summary>
/// <param name="Slot">The inventory slot, e.g. 0 for the left hand.</param>
/// <param name="Group">The group of the item definition; with <paramref name="Number"/>, it identifies the item, e.g. for its picture.</param>
/// <param name="Number">The number of the item definition in its group.</param>
/// <param name="Name">The name of the item.</param>
/// <param name="Level">The level of the item, e.g. 13 for +13.</param>
/// <param name="IsExcellent">A value indicating whether the item has excellent options.</param>
/// <param name="IsAncient">A value indicating whether the item is part of an ancient set.</param>
/// <param name="Sockets">The number of sockets of the item.</param>
/// <param name="Image">The file name of the picture of the item, e.g. <c>item_0_16_13_e.png</c>, like the pictures of the item editor.</param>
/// <param name="HasSkill">A value indicating whether the item has a skill.</param>
/// <param name="AncientSetName">The english name of the ancient set of the item; <c>null</c>, if the item isn't ancient.</param>
/// <param name="Options">The structured options of the item, one entry per option, ordered by type and number.</param>
public sealed record WebsiteItem(int Slot, int Group, int Number, string Name, int Level, bool IsExcellent, bool IsAncient, int Sockets, string Image, bool HasSkill, string? AncientSetName, IReadOnlyList<WebsiteItemOption> Options);
