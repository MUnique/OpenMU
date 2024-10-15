// <copyright file="GameConfigurationViewItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Models;

using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.DataModel.Entities;

[Display(Name = "Game configuration")]
public class GameConfigurationViewModel
{
    /// <summary>
    /// Gets or sets the maximum reachable level.
    /// </summary>
    [Display(Name = "Maximum level", Description = "Defines the maximum level of a character.")]
    public short MaximumLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum reachable master level.
    /// </summary>
    [Display(Name = "Maximum master level", Description = "Defines the maximum master level of a character.")]
    public short MaximumMasterLevel { get; set; }

    /// <summary>
    /// Gets or sets the experience rate of the game.
    /// </summary>
    public float ExperienceRate { get; set; }

    /// <summary>
    /// Gets or sets the minimum monster level which are required to be killed
    /// in order to gain master experience for master character classes.
    /// </summary>
    public byte MinimumMonsterLevelForMasterExperience { get; set; }

    /// <summary>
    /// Gets or sets the information range. This defines how far players can see other game objects.
    /// </summary>
    public byte InfoRange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether area skills hit players.
    /// </summary>
    /// <remarks>
    /// Usually false, during castle siege this might be true.
    /// </remarks>
    public bool AreaSkillHitsPlayer { get; set; }

    /// <summary>
    /// Gets or sets the maximum inventory money value.
    /// </summary>
    public int MaximumInventoryMoney { get; set; }

    /// <summary>
    /// Gets or sets the maximum vault money value.
    /// </summary>
    public int MaximumVaultMoney { get; set; }

    /// <summary>
    /// Gets or sets the interval for attribute recoveries. See also MUnique.OpenMU.GameLogic.Attributes.Stats.Regeneration.
    /// </summary>
    public int RecoveryInterval { get; set; }

    /// <summary>
    /// Gets or sets the maximum numbers of letters a player can have in his inbox.
    /// </summary>
    public int MaximumLetters { get; set; }

    /// <summary>
    /// Gets or sets the price of sending a letter.
    /// </summary>
    public int LetterSendPrice { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of characters per account.
    /// </summary>
    public byte MaximumCharactersPerAccount { get; set; }

    /// <summary>
    /// Gets or sets the character name regex.
    /// </summary>
    /// <remarks>
    /// "^[a-zA-Z0-9]{3,10}$";.
    /// </remarks>
    public string? CharacterNameRegex { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the password.
    /// </summary>
    public int MaximumPasswordLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of parties.
    /// </summary>
    public byte MaximumPartySize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether if a monster should drop or adds money to the character directly.
    /// </summary>
    public bool ShouldDropMoney { get; set; }

    /// <summary>
    /// Gets or sets the maximum droppable item option level.
    /// </summary>
    public byte MaximumItemOptionLevelDrop { get; set; }

    /// <summary>
    /// Gets or sets the accumulated damage which needs to be done to decrease <see cref="Item.Durability"/> of a defending item by 1.
    /// </summary>
    public double DamagePerOneItemDurability { get; set; }

    /// <summary>
    /// Gets or sets the accumulated damage which needs to be done to decrease <see cref="Item.Durability"/> of a pet item by 1.
    /// </summary>
    public double DamagePerOnePetDurability { get; set; }

    /// <summary>
    /// Gets or sets the number of hits which needs to be done to decrease the <see cref="Item.Durability"/> of an offensive item by 1.
    /// </summary>
    public double HitsPerOneItemDurability { get; set; }
}