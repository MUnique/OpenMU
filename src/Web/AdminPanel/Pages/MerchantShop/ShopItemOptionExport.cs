// <copyright file="ShopItemOptionExport.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages.MerchantShop;

/// <summary>
/// Serializable representation of an option of a merchant shop item.
/// </summary>
/// <remarks>
/// The referenced option is identified by its <see cref="OptionTypeId"/>, <see cref="Number"/> and
/// <see cref="SubOptionType"/>, because these are stable within a configuration, while its object
/// identity isn't. The ids of option types are not portable between configurations, though.
/// Therefore, the <see cref="OptionTypeName"/> is exported as well, and is used as fallback
/// when the id is unknown to the configuration the file is imported into.
/// </remarks>
public sealed record ShopItemOptionExport
{
    /// <summary>
    /// Gets or sets the id of the <see cref="MUnique.OpenMU.DataModel.Configuration.Items.ItemOption.OptionType"/>.
    /// </summary>
    public Guid OptionTypeId { get; set; }

    /// <summary>
    /// Gets or sets the name (in neutral language) of the
    /// <see cref="MUnique.OpenMU.DataModel.Configuration.Items.ItemOption.OptionType"/>.
    /// It's used to find the option type in another configuration, where its id differs.
    /// </summary>
    public string? OptionTypeName { get; set; }

    /// <summary>
    /// Gets or sets the number of the option within its option type.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the sub option type of the option, e.g. the element of a socket option.
    /// </summary>
    public int SubOptionType { get; set; }

    /// <summary>
    /// Gets or sets the option level.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the index of the option, which is relevant when the options are sorted, e.g. for sockets.
    /// </summary>
    public int Index { get; set; }
}
