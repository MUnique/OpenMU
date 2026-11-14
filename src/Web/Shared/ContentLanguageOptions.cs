// <copyright file="ContentLanguageOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

/// <summary>
/// The languages in which the translatable texts of the configuration (map names, item names, the
/// lines of the bots, ...) can be edited, in addition to the languages of the admin panel itself.
/// </summary>
/// <remarks>
/// The editor of a translatable text shows a field for every language of the admin panel's user
/// interface. A server whose players speak another language - e.g. Polish - needs a field for it,
/// although the admin panel isn't translated to it. Set in the configuration section
/// <c>AdminPanel</c>, e.g. by the environment variable <c>AdminPanel__ContentLanguages__0=pl</c>.
/// </remarks>
public class ContentLanguageOptions
{
    /// <summary>
    /// Gets or sets the codes of the additional languages, e.g. <c>pl</c>.
    /// </summary>
    public string[] ContentLanguages { get; set; } = [];
}
