// <copyright file="ConfigurationCaptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Captions;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Compares the captions of a configuration with their sources (see <see cref="LocalizedString.SourceKey"/>)
/// and applies selected differences.
/// </summary>
public static class ConfigurationCaptions
{
    /// <summary>
    /// Determines the differences between the captions of the configuration and their sources.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The differences. If it's empty, all available localizations are in place.</returns>
    public static IReadOnlyList<CaptionChange> DetermineChanges(GameConfiguration gameConfiguration)
    {
        return LocalizedCaption.FindAll(gameConfiguration)
            .SelectMany(caption => DetermineChanges(caption))
            .OrderBy(change => change.OwnerType, StringComparer.Ordinal)
            .ThenBy(change => change.NeutralText, StringComparer.CurrentCulture)
            .ThenBy(change => change.PropertyName, StringComparer.Ordinal)
            .ThenBy(change => change.CultureName, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Counts the captions which are linked to a source, i.e. which have a <see cref="LocalizedString.SourceKey"/>.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The number of linked captions.</returns>
    public static int CountLinkedCaptions(GameConfiguration gameConfiguration)
    {
        return LocalizedCaption.FindAll(gameConfiguration).Count(caption => caption.Value.SourceKey is not null);
    }

    /// <summary>
    /// Finds the source keys of captions which can't be resolved, e.g. because the source isn't registered
    /// at <see cref="LocalizedStringResources"/> or doesn't contain the key anymore.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The distinct unresolved source keys.</returns>
    public static IReadOnlyList<string> FindUnresolvedSourceKeys(GameConfiguration gameConfiguration)
    {
        return LocalizedCaption.FindAll(gameConfiguration)
            .Where(caption => caption.Value.SourceKey is not null && caption.Value.GetFromSource() is null)
            .Select(caption => caption.Value.SourceKey!)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Applies the selected changes to the configuration.
    /// Captions which are equal to their source afterwards get an updated <see cref="LocalizedString.SourceStamp"/>,
    /// so that later changes of the source are detected as <see cref="CaptionChangeKind.Updated"/>.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="selectedChangeIds">The identifiers of the selected changes, see <see cref="CaptionChange.Id"/>.</param>
    /// <returns>The number of applied changes.</returns>
    public static int ApplyChanges(GameConfiguration gameConfiguration, IEnumerable<string> selectedChangeIds)
    {
        var selected = selectedChangeIds.ToHashSet(StringComparer.Ordinal);
        var applied = 0;
        foreach (var caption in LocalizedCaption.FindAll(gameConfiguration).ToList())
        {
            if (caption.Value.GetFromSource() is not { } source)
            {
                continue;
            }

            var value = caption.Value;
            foreach (var change in DetermineChanges(caption, source).Where(c => selected.Contains(c.Id)))
            {
                var culture = change.CultureName is null
                    ? CultureInfo.GetCultureInfo(LocalizedString.NeutralLanguageCode)
                    : CultureInfo.GetCultureInfo(change.CultureName);
                value = value.WithTranslation(culture, change.SourceText);
                applied++;
            }

            if (value.ComputeContentHash() == source.ComputeContentHash() && value.SourceStamp != source.SourceStamp)
            {
                value = value.WithSourceStamp();
            }

            if (!value.Equals(caption.Value))
            {
                caption.SetValue(value);
            }
        }

        return applied;
    }

    /// <summary>
    /// Adds the <see cref="LocalizedString.SourceKey"/>s of a reference configuration (usually a freshly initialized one)
    /// to the matching captions of the configuration, which don't have a source key yet.
    /// Captions are matched by the identifier of their owner and the property. Some built-in objects don't have
    /// deterministic identifiers, so the remaining captions are matched by the type of their owner, its number
    /// and the property, if this combination is unique in both configurations.
    /// In any case, the neutral texts must be equal. No texts are changed, so the differences
    /// can be reviewed with <see cref="DetermineChanges(GameConfiguration)"/> afterwards.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="referenceConfiguration">The reference configuration.</param>
    /// <returns>The number of linked captions and the number of captions which were skipped because their neutral text was customized.</returns>
    public static (int Linked, int SkippedBecauseOfCustomizedNeutralText) LinkSourceKeys(GameConfiguration gameConfiguration, GameConfiguration referenceConfiguration)
    {
        var references = LocalizedCaption.FindAll(referenceConfiguration).ToList();
        var referencesById = references
            .Where(caption => caption.Value.SourceKey is not null)
            .GroupBy(caption => caption.Key)
            .ToDictionary(g => g.Key, g => g.First().Value);
        var targets = LocalizedCaption.FindAll(gameConfiguration).ToList();
        var targetIds = targets.Select(caption => caption.Key).ToHashSet();
        var referencesByNumber = references
            .Where(caption => caption.Value.SourceKey is not null && !targetIds.Contains(caption.Key))
            .GroupBy(GetNumberKey)
            .Where(g => g.Key is not null && g.Count() == 1)
            .ToDictionary(g => g.Key!.Value, g => g.Single().Value);
        var ambiguousTargetNumbers = targets
            .GroupBy(GetNumberKey)
            .Where(g => g.Key is not null && g.Count() > 1)
            .Select(g => g.Key!.Value)
            .ToHashSet();

        var linked = 0;
        var skipped = 0;
        foreach (var caption in targets)
        {
            if (caption.Value.SourceKey is not null)
            {
                continue;
            }

            if (!referencesById.TryGetValue(caption.Key, out var reference)
                && (GetNumberKey(caption) is not { } numberKey
                    || ambiguousTargetNumbers.Contains(numberKey)
                    || !referencesByNumber.TryGetValue(numberKey, out reference)))
            {
                continue;
            }

            if (caption.Value.ValueInNeutralLanguage != reference.ValueInNeutralLanguage)
            {
                skipped++;
                continue;
            }

            caption.SetValue(caption.Value.WithSourceKey(reference.SourceKey));
            linked++;
        }

        return (linked, skipped);
    }

    private static IEnumerable<CaptionChange> DetermineChanges(LocalizedCaption caption)
    {
        return caption.Value.GetFromSource() is { } source
            ? DetermineChanges(caption, source)
            : [];
    }

    private static IEnumerable<CaptionChange> DetermineChanges(LocalizedCaption caption, LocalizedString source)
    {
        var current = caption.Value;
        var neutral = current.ValueInNeutralLanguage;
        var isUnchanged = current.IsUnchangedSinceSourceStamp;

        CaptionChange Create(string? cultureName, string? currentText, string? sourceText, CaptionChangeKind kind) => new(
            $"{caption.OwnerId:N}/{caption.Property.Name}/{cultureName ?? LocalizedString.NeutralLanguageCode}",
            caption.OwnerId,
            GetTypeName(caption.Owner),
            caption.Property.Name,
            source.SourceKey!,
            neutral,
            cultureName,
            currentText,
            sourceText,
            kind);

        var sourceNeutral = source.ValueInNeutralLanguage;
        if (neutral != sourceNeutral)
        {
            yield return Create(null, neutral, sourceNeutral, isUnchanged ? CaptionChangeKind.Updated : CaptionChangeKind.Customized);
        }

        var sourceCultures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (cultureName, sourceText) in source.GetTranslations())
        {
            sourceCultures.Add(cultureName);
            var currentText = current.GetOwnTranslation(CultureInfo.GetCultureInfo(cultureName));
            if (currentText == sourceText)
            {
                continue;
            }

            var kind = string.IsNullOrEmpty(currentText) || currentText == neutral
                ? CaptionChangeKind.Missing
                : isUnchanged ? CaptionChangeKind.Updated : CaptionChangeKind.Customized;
            yield return Create(cultureName, currentText, sourceText, kind);
        }

        if (isUnchanged)
        {
            foreach (var (cultureName, currentText) in current.GetTranslations())
            {
                if (!sourceCultures.Contains(cultureName))
                {
                    yield return Create(cultureName, currentText, null, CaptionChangeKind.Removed);
                }
            }
        }
    }

    private static (string OwnerType, long Number, string PropertyName)? GetNumberKey(LocalizedCaption caption)
    {
        var numberProperty = caption.Owner.GetType().GetProperty("Number");
        return numberProperty?.GetValue(caption.Owner) is { } number and (byte or short or int or long or ushort)
            ? (GetTypeName(caption.Owner), Convert.ToInt64(number, CultureInfo.InvariantCulture), caption.Property.Name)
            : null;
    }

    private static string GetTypeName(object owner)
    {
        var type = owner.GetType();
        while (type.BaseType is { } baseType && type.Assembly != typeof(GameConfiguration).Assembly)
        {
            type = baseType;
        }

        return type.Name;
    }
}
