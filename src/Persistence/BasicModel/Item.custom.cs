// <copyright file="Item.custom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Extended <see cref="Item"/> to implement <see cref="CloneItemOptionLink"/>.
/// </summary>
public partial class Item
{
    /// <summary>
    /// Clones the item option link.
    /// </summary>
    /// <param name="link">The link.</param>
    /// <returns>The cloned item option link.</returns>
    protected override DataModel.Entities.ItemOptionLink CloneItemOptionLink(DataModel.Entities.ItemOptionLink link)
    {
        var persistentLink = new ItemOptionLink();
        persistentLink.AssignValues(link);
        return persistentLink;
    }
}