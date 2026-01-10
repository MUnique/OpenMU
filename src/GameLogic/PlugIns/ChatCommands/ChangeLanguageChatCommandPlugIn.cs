// <copyright file="ChangeLanguageChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the language change command.
/// </summary>
[Guid("06870B25-3240-49CF-ADD4-F3060EA1FA7D")]
[PlugIn]
[Display(Name = nameof(PlugInResources.ChangeLanguageChatCommandPlugIn_Name), Description = nameof(PlugInResources.ChangeLanguageChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, "Changes the language of the account.", typeof(ChangeLanguageChatCommandArgs), CharacterStatus.Normal)]
public class ChangeLanguageChatCommandPlugIn : ChatCommandPlugInBase<ChangeLanguageChatCommandArgs>
{
    private const string Command = "/language";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, ChangeLanguageChatCommandArgs arguments)
    {
        var languages = CultureInfo
            .GetCultures(CultureTypes.AllCultures)
            .Where(c => !object.Equals(c, CultureInfo.InvariantCulture))
            .Where(c => c is { IsNeutralCulture: true, TwoLetterISOLanguageName: "en" }
                || PlayerMessage.ResourceManager.GetResourceSet(c, true, false) is not null)
            .ToList();

        if (string.IsNullOrWhiteSpace(arguments.IsoLanguageCode))
        {
            await ShowAvailableLanguagesAsync(player, languages).ConfigureAwait(false);
            return;
        }

        var requestedCulture = languages.FirstOrDefault(cu => cu.TwoLetterISOLanguageName == arguments.IsoLanguageCode
                                                              || cu.ThreeLetterISOLanguageName == arguments.IsoLanguageCode);
        if (requestedCulture is null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.RequestedLanguageNotFound), arguments.IsoLanguageCode).ConfigureAwait(false);
            await ShowAvailableLanguagesAsync(player, languages).ConfigureAwait(false);
        }
        else
        {
            player.Culture = requestedCulture;
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.LanguageChanged), requestedCulture.NativeName, requestedCulture.TwoLetterISOLanguageName).ConfigureAwait(false);
        }
    }

    private static async ValueTask ShowAvailableLanguagesAsync(Player player, List<CultureInfo> languages)
    {
        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.AvailableLanguagesCaption)).ConfigureAwait(false);
        foreach (var lang in languages.OrderBy(l => l.TwoLetterISOLanguageName))
        {
            await player.ShowBlueMessageAsync($"  {lang.TwoLetterISOLanguageName} - {lang.NativeName} / {lang.EnglishName}").ConfigureAwait(false);
        }
    }
}