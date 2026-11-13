// <copyright file="BotClassAlias.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// The words players use for a line of character classes in a party search, e.g. "dk, bk, bm" for
/// the Dark Knight, the Blade Knight and the Blade Master (see <see cref="BotPartyRequest"/>).
/// </summary>
/// <remarks>
/// The <see cref="ClassNumber"/> stands for its whole line of classes: a search for "bk" is answered
/// by Dark Knights, Blade Knights and Blade Masters, whichever of them the alias names.
/// </remarks>
public class BotClassAlias
{
    /// <summary>
    /// Gets or sets the comma-separated words for the class line, e.g. <c>dk, bk, bm, knight</c>.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotClassAlias_Keywords_Name), Description = nameof(PlugInResources.BotClassAlias_Keywords_Description))]
    public string Keywords { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of a character class of the line, e.g. 4 for the Dark Knight.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotClassAlias_ClassNumber_Name), Description = nameof(PlugInResources.BotClassAlias_ClassNumber_Description))]
    public byte ClassNumber { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{this.ClassNumber}: {this.Keywords}";
}
