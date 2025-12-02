// <copyright file="ItemOptionTypes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Some standard option types.
/// </summary>
public static class ItemOptionTypes
{
    /// <summary>
    /// Gets the excellent item option type.
    /// </summary>
    public static ItemOptionType Excellent { get; } = new() { Name = "Excellent Option", Id = new Guid("{6487C498-58E0-48E5-B409-35D7598313FC}"), IsVisible = true };

    /// <summary>
    /// Gets the wing option type.
    /// </summary>
    public static ItemOptionType Wing { get; } = new() { Name = "Wing Option", Id = new Guid("{55CB57A7-4FC6-47BB-9FEE-84E6C4EBCE95}") };

    /// <summary>
    /// Gets the luck option type.
    /// </summary>
    public static ItemOptionType Luck { get; } = new() { Name = "Luck (Critical Damage Chance 5%)", Id = new Guid("{3E3E9BE8-4E16-4F27-A7CF-986D48454D76}") };

    /// <summary>
    /// Gets the standard option option type.
    /// </summary>
    public static ItemOptionType Option { get; } = new() { Name = "Option", Id = new Guid("{F193F91E-86D7-4456-ADD8-A3667E731303}") };

    /// <summary>
    /// Gets the harmony option type.
    /// </summary>
    public static ItemOptionType HarmonyOption { get; } = new() { Name = "Jewel of Harmony Option", Id = new Guid("{0CA234F0-4A0F-4FA1-8E07-CFB89C1EC94F}") };

    /// <summary>
    /// Gets the ancient option type.
    /// </summary>
    public static ItemOptionType AncientOption { get; } = new() { Name = "Ancient Option", Id = new Guid("{436D820F-6D50-429D-AF63-BB0F59567DD1}"), IsVisible = true };

    /// <summary>
    /// Gets the ancient bonus option type.
    /// </summary>
    public static ItemOptionType AncientBonus { get; } = new() { Name = "Ancient Bonus Option", Id = new Guid("{5E2C10EF-E580-48D5-A48B-0FFCD0678966}") };

    /// <summary>
    /// Gets the guardian option type.
    /// </summary>
    public static ItemOptionType GuardianOption { get; } = new() { Name = "Guardian Option", Id = new Guid("{4AA95715-1ED3-453D-8D1D-093B281416CA}"), Description = "This option is added by the chaos machine with a jewel of guardian on level 380 items." };

    /// <summary>
    /// Gets the socket option type.
    /// </summary>
    public static ItemOptionType SocketOption { get; } = new() { Name = "Socket Option", Id = new Guid("{AAB309D3-CD97-4F77-AE1B-E9F904102502}") };

    /// <summary>
    /// Gets the socket bonus option type.
    /// </summary>
    public static ItemOptionType SocketBonusOption { get; } = new() { Name = "Socket Bonus Option", Id = new Guid("{43DA2C68-D6E1-4B94-ADB1-8864D92F8FB9}") };

    /// <summary>
    /// Gets the blue fenrir option type.
    /// </summary>
    /// <remarks>Applies only to the fenrir pet.</remarks>
    public static ItemOptionType BlueFenrir { get; } = new() { Name = "Blue Fenrir Option", Id = new Guid("{C3ED45BC-5713-494D-A8C8-DC4AFAE56223}"), IsVisible = true };

    /// <summary>
    /// Gets the black fenrir option type.
    /// </summary>
    /// <remarks>Applies only to the fenrir pet.</remarks>
    public static ItemOptionType BlackFenrir { get; } = new() { Name = "Black Fenrir Option", Id = new Guid("{ED978695-BD3E-46EA-86D8-F8C30EA99B50}"), IsVisible = true };

    /// <summary>
    /// Gets the gold fenrir option type.
    /// </summary>
    /// <remarks>Applies only to the fenrir pet.</remarks>
    public static ItemOptionType GoldFenrir { get; } = new() { Name = "Gold Fenrir Option", Id = new Guid("{78E6DB0B-AC53-454C-956F-CD2B5467856E}"), IsVisible = true };

    /// <summary>
    /// Gets the dark horse option type.
    /// </summary>
    /// <remarks>Applies only to the dark horse pet.</remarks>
    public static ItemOptionType DarkHorse { get; } = new() { Name = "Dark Horse Option", Id = new Guid("{D2295C44-E458-40F8-8555-87CFD9626616}"), IsVisible = true };
}