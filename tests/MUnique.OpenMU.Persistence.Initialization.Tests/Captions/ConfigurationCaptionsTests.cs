// <copyright file="ConfigurationCaptionsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests.Captions;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for <see cref="ConfigurationCaptions"/>.
/// </summary>
[TestFixture]
public class ConfigurationCaptionsTests
{
    private static readonly CultureInfo ChineseSimplified = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de");
    private static readonly CultureInfo French = CultureInfo.GetCultureInfo("fr");

    private static LocalizedString LorenciaFromSource => LocalizedString.FromResource(() => TestNames.Lorencia);

    private static LocalizedString DeviasFromSource => LocalizedString.FromResource(() => TestNames.Devias);

    private static string LorenciaKey => LorenciaFromSource.SourceKey!;

    private static string DeviasKey => DeviasFromSource.SourceKey!;

    /// <summary>
    /// Tests that captions without source key are ignored.
    /// </summary>
    [Test]
    public void CaptionWithoutSourceKey_IsIgnored()
    {
        var (configuration, _) = CreateConfiguration(new LocalizedString("Lorencia"));

        Assert.That(ConfigurationCaptions.DetermineChanges(configuration), Is.Empty);
    }

    /// <summary>
    /// Tests that captions with unresolvable source keys are reported.
    /// </summary>
    [Test]
    public void UnresolvedSourceKeys_AreReported()
    {
        var (configuration, _) = CreateConfiguration(
            new LocalizedString("Lorencia").WithSourceKey("NotRegistered/Lorencia"),
            new LocalizedString("Lorencia").WithSourceKey("NotRegistered/Lorencia"),
            LorenciaFromSource);

        Assert.That(ConfigurationCaptions.FindUnresolvedSourceKeys(configuration), Is.EqualTo(new[] { "NotRegistered/Lorencia" }));
        Assert.That(ConfigurationCaptions.DetermineChanges(configuration), Is.Empty);
    }

    /// <summary>
    /// Tests that a caption which is equal to its source results in no changes.
    /// </summary>
    [Test]
    public void CaptionEqualToSource_AllLocalizationsInPlace()
    {
        var (configuration, _) = CreateConfiguration(LorenciaFromSource, DeviasFromSource);

        Assert.That(ConfigurationCaptions.DetermineChanges(configuration), Is.Empty);
    }

    /// <summary>
    /// Tests that missing translations and copies of the neutral text are reported as recommended <see cref="CaptionChangeKind.Missing"/> changes.
    /// </summary>
    [Test]
    public void MissingTranslations_AreRecommended()
    {
        var (configuration, monsters) = CreateConfiguration(
            new LocalizedString("Devias").WithTranslation(German, "Devias").WithSourceKey(DeviasKey));

        var changes = ConfigurationCaptions.DetermineChanges(configuration);

        Assert.That(changes.Select(c => (c.CultureName, c.Kind, c.SourceText)), Is.EquivalentTo(new[]
        {
            ("de", CaptionChangeKind.Missing, "Devias (de)"),
            ("zh-CN", CaptionChangeKind.Missing, "冰风谷"),
        }));
        Assert.That(changes, Has.All.Matches<CaptionChange>(c => c.IsRecommended));
        Assert.That(changes[0].OwnerId, Is.EqualTo(monsters[0].GetId()));
        Assert.That(changes[0].OwnerType, Is.EqualTo(nameof(MonsterDefinition)));
        Assert.That(changes[0].PropertyName, Is.EqualTo(nameof(MonsterDefinition.Designation)));
        Assert.That(changes[0].SourceKey, Is.EqualTo(DeviasKey));
    }

    /// <summary>
    /// Tests that differing texts of captions without stamp are reported as <see cref="CaptionChangeKind.Customized"/>, which is not recommended.
    /// </summary>
    [Test]
    public void DifferingTextWithoutStamp_IsCustomized()
    {
        var (configuration, _) = CreateConfiguration(
            new LocalizedString("Lorencia City").WithTranslation(ChineseSimplified, "罗兰").WithSourceKey(LorenciaKey));

        var changes = ConfigurationCaptions.DetermineChanges(configuration);

        Assert.That(changes.Select(c => (c.CultureName, c.Kind)), Is.EquivalentTo(new[]
        {
            ((string?)null, CaptionChangeKind.Customized),
            ("zh-CN", CaptionChangeKind.Customized),
        }));
        Assert.That(changes, Has.None.Matches<CaptionChange>(c => c.IsRecommended));
    }

    /// <summary>
    /// Tests that captions which were changed after they were taken from their source are reported as <see cref="CaptionChangeKind.Customized"/>.
    /// </summary>
    [Test]
    public void ChangedAfterStamp_IsCustomized()
    {
        var (configuration, _) = CreateConfiguration(LorenciaFromSource.WithTranslation(ChineseSimplified, "罗兰"));

        var change = ConfigurationCaptions.DetermineChanges(configuration).Single();

        Assert.That(change.Kind, Is.EqualTo(CaptionChangeKind.Customized));
        Assert.That(change.CurrentText, Is.EqualTo("罗兰"));
        Assert.That(change.SourceText, Is.EqualTo("勇者大陆"));
    }

    /// <summary>
    /// Tests that captions which are unchanged since they were taken from an older version of the source
    /// are reported as recommended <see cref="CaptionChangeKind.Updated"/> and <see cref="CaptionChangeKind.Removed"/> changes.
    /// </summary>
    [Test]
    public void UnchangedSinceOlderSource_IsUpdatedAndRemoved()
    {
        // Simulates a value which was taken from an older version of the source.
        var olderSourceValue = new LocalizedString("Lorencia")
            .WithTranslation(ChineseSimplified, "罗兰")
            .WithTranslation(French, "Lorencia (fr)")
            .WithSourceKey(LorenciaKey)
            .WithSourceStamp();
        var (configuration, _) = CreateConfiguration(olderSourceValue);

        var changes = ConfigurationCaptions.DetermineChanges(configuration);

        Assert.That(changes.Select(c => (c.CultureName, c.Kind, c.SourceText)), Is.EquivalentTo(new[]
        {
            ("zh-CN", CaptionChangeKind.Updated, (string?)"勇者大陆"),
            ("fr", CaptionChangeKind.Removed, null),
        }));
        Assert.That(changes, Has.All.Matches<CaptionChange>(c => c.IsRecommended));
    }

    /// <summary>
    /// Tests that applying all changes makes the caption equal to its source, including a fresh stamp.
    /// </summary>
    [Test]
    public void ApplyAllChanges_EqualsSourceAfterwards()
    {
        var olderSourceValue = new LocalizedString("Lorencia")
            .WithTranslation(ChineseSimplified, "罗兰")
            .WithTranslation(French, "Lorencia (fr)")
            .WithSourceKey(LorenciaKey)
            .WithSourceStamp();
        var (configuration, monsters) = CreateConfiguration(olderSourceValue);
        var changes = ConfigurationCaptions.DetermineChanges(configuration);

        var applied = ConfigurationCaptions.ApplyChanges(configuration, changes.Select(c => c.Id));

        Assert.That(applied, Is.EqualTo(2));
        Assert.That(ConfigurationCaptions.DetermineChanges(configuration), Is.Empty);
        Assert.That(monsters[0].Designation.ComputeContentHash(), Is.EqualTo(LorenciaFromSource.ComputeContentHash()));
        Assert.That(monsters[0].Designation.IsUnchangedSinceSourceStamp, Is.True);
        Assert.That(monsters[0].Designation.SourceKey, Is.EqualTo(LorenciaKey));
    }

    /// <summary>
    /// Tests that only the selected changes are applied and the not selected customized texts are kept.
    /// </summary>
    [Test]
    public void ApplySelectedChanges_KeepsUnselected()
    {
        var (configuration, monsters) = CreateConfiguration(
            new LocalizedString("Devias").WithTranslation(ChineseSimplified, "冰谷").WithSourceKey(DeviasKey));
        var changes = ConfigurationCaptions.DetermineChanges(configuration);
        var recommended = changes.Where(c => c.IsRecommended).Select(c => c.Id).ToList();

        var applied = ConfigurationCaptions.ApplyChanges(configuration, recommended);

        Assert.That(applied, Is.EqualTo(1));
        Assert.That(monsters[0].Designation.GetOwnTranslation(German), Is.EqualTo("Devias (de)"));
        Assert.That(monsters[0].Designation.GetOwnTranslation(ChineseSimplified), Is.EqualTo("冰谷"), "customized text is kept");
        Assert.That(ConfigurationCaptions.DetermineChanges(configuration).Single().Kind, Is.EqualTo(CaptionChangeKind.Customized));
    }

    /// <summary>
    /// Tests that captions which are equal to their source get a stamp when changes are applied, even without selected changes for them.
    /// </summary>
    [Test]
    public void ApplyChanges_StampsCaptionsEqualToSource()
    {
        var unstamped = new LocalizedString("Lorencia||zh-CN=勇者大陆").WithSourceKey(LorenciaKey);
        var (configuration, monsters) = CreateConfiguration(unstamped);
        Assert.That(monsters[0].Designation.SourceStamp, Is.Null);

        ConfigurationCaptions.ApplyChanges(configuration, []);

        Assert.That(monsters[0].Designation.IsUnchangedSinceSourceStamp, Is.True);
    }

    /// <summary>
    /// Tests that unknown change ids are ignored.
    /// </summary>
    [Test]
    public void ApplyChanges_UnknownIds_AreIgnored()
    {
        var value = new LocalizedString("Devias").WithSourceKey(DeviasKey);
        var (configuration, monsters) = CreateConfiguration(value);

        var applied = ConfigurationCaptions.ApplyChanges(configuration, ["unknown"]);

        Assert.That(applied, Is.Zero);
        Assert.That(monsters[0].Designation, Is.EqualTo(value));
    }

    /// <summary>
    /// Tests that source keys are linked by owner identifier and property, but only if the neutral texts match.
    /// </summary>
    [Test]
    public void LinkSourceKeys_LinksMatchingCaptions()
    {
        var (reference, referenceMonsters) = CreateConfiguration(LorenciaFromSource, DeviasFromSource, LorenciaFromSource);
        var (target, targetMonsters) = CreateConfiguration(
            new LocalizedString("Lorencia||zh-CN=罗兰"),
            new LocalizedString("Devias City"),
            new LocalizedString("Lorencia").WithSourceKey("Other/Key"));
        for (var i = 0; i < targetMonsters.Count; i++)
        {
            targetMonsters[i].SetId(referenceMonsters[i].GetId());
        }

        Assert.That(ConfigurationCaptions.CountLinkedCaptions(target), Is.EqualTo(1));

        var (linked, skipped) = ConfigurationCaptions.LinkSourceKeys(target, reference);

        Assert.That(linked, Is.EqualTo(1));
        Assert.That(skipped, Is.EqualTo(1));
        Assert.That(ConfigurationCaptions.CountLinkedCaptions(target), Is.EqualTo(2));
        Assert.That(targetMonsters[0].Designation.Value, Is.EqualTo("Lorencia||zh-CN=罗兰||@src=" + LorenciaKey), "texts are unchanged, no stamp");
        Assert.That(targetMonsters[1].Designation.SourceKey, Is.Null, "customized neutral text");
        Assert.That(targetMonsters[2].Designation.SourceKey, Is.EqualTo("Other/Key"), "existing source keys are kept");
        Assert.That(ConfigurationCaptions.DetermineChanges(target).Single().Kind, Is.EqualTo(CaptionChangeKind.Customized));
    }

    /// <summary>
    /// Tests that captions without matching identifier are linked by the number of their owner, but only if it's unique.
    /// </summary>
    [Test]
    public void LinkSourceKeys_ByNumber_OnlyIfUnique()
    {
        var (reference, referenceMonsters) = CreateConfiguration(LorenciaFromSource, DeviasFromSource);
        referenceMonsters[0].Number = 1;
        referenceMonsters[1].Number = 2;
        var (target, targetMonsters) = CreateConfiguration(new LocalizedString("Lorencia"), new LocalizedString("Devias"), new LocalizedString("Devias"));
        targetMonsters[0].Number = 1;
        targetMonsters[1].Number = 2;
        targetMonsters[2].Number = 2;

        var (linked, skipped) = ConfigurationCaptions.LinkSourceKeys(target, reference);

        Assert.That((linked, skipped), Is.EqualTo((1, 0)));
        Assert.That(targetMonsters[0].Designation.SourceKey, Is.EqualTo(LorenciaKey));
        Assert.That(targetMonsters[1].Designation.SourceKey, Is.Null, "ambiguous number");
        Assert.That(targetMonsters[2].Designation.SourceKey, Is.Null, "ambiguous number");
    }

    /// <summary>
    /// Tests that the linkable captions are exactly the ones which are linked afterwards, and that finding them changes nothing.
    /// </summary>
    [Test]
    public void FindLinkableCaptions_FindsTheCaptionsWhichWouldBeLinked()
    {
        var (referenceConfiguration, referenceMonsters) = CreateConfiguration(LorenciaFromSource, DeviasFromSource, LorenciaFromSource);
        var (target, targetMonsters) = CreateConfiguration(
            new LocalizedString("Lorencia"),
            new LocalizedString("Devias City"),
            LorenciaFromSource);
        for (var i = 0; i < targetMonsters.Count; i++)
        {
            targetMonsters[i].SetId(referenceMonsters[i].GetId());
        }

        var reference = CaptionLinkReference.Create(referenceConfiguration);

        Assert.That(ConfigurationCaptions.FindLinkableCaptions(target, reference), Is.EquivalentTo(new Dictionary<string, int> { { nameof(MonsterDefinition), 1 } }));
        Assert.That(targetMonsters[0].Designation.SourceKey, Is.Null, "nothing is changed");
        Assert.That(ConfigurationCaptions.LinkSourceKeys(target, reference), Is.EqualTo((1, 1)));
        Assert.That(ConfigurationCaptions.FindLinkableCaptions(target, reference), Is.Empty);
    }

    /// <summary>
    /// Tests that a complete, freshly initialized configuration has all available localizations in place.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task FreshSeason6Configuration_AllLocalizationsInPlaceAsync()
    {
        var (_, configuration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        Assert.That(ConfigurationCaptions.DetermineChanges(configuration), Is.Empty);
    }

    /// <summary>
    /// Tests that the source keys of an independently initialized reference configuration are linked to all
    /// matching captions, including the built-in objects without deterministic identifiers.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task LinkSourceKeys_IndependentSeason6Configurations_LinksAllBuiltInCaptionsAsync()
    {
        var (referenceContext, reference) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        var (targetContext, target) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var r = referenceContext;
        using var t = targetContext;
        // Simulates a configuration which was created before captions had source keys.
        foreach (var monster in target.Monsters)
        {
            monster.Designation = monster.Designation.WithSourceKey(null);
        }

        foreach (var map in target.Maps)
        {
            map.Name = map.Name.WithSourceKey(null);
        }

        foreach (var monster in reference.Monsters)
        {
            monster.Designation = monster.Designation.WithSourceKey($"Monsters/{monster.Number}");
        }

        foreach (var map in reference.Maps)
        {
            map.Name = map.Name.WithSourceKey($"Maps/{map.Number}");
        }

        var (linked, skipped) = ConfigurationCaptions.LinkSourceKeys(target, reference);

        Assert.That(skipped, Is.Zero);
        Assert.That(linked, Is.EqualTo(reference.Monsters.Count + reference.Maps.Count));
        Assert.That(target.Monsters.First(m => m.Number == 0).Designation.SourceKey, Is.EqualTo("Monsters/0"));
        Assert.That(target.Maps.First(m => m.Number == 0).Name.SourceKey, Is.EqualTo("Maps/0"));
    }

    private static async Task<(IContext Context, GameConfiguration Configuration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory()).CreateInitialDataAsync(1, false).ConfigureAwait(false);
        var context = contextProvider.CreateNewContext();
        var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        return (context, configuration);
    }

    private static (GameConfiguration Configuration, IReadOnlyList<MonsterDefinition> Monsters) CreateConfiguration(params LocalizedString[] designations)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        using var context = contextProvider.CreateNewContext();
        var configuration = context.CreateNew<GameConfiguration>();
        var monsters = new List<MonsterDefinition>();
        foreach (var designation in designations)
        {
            var monster = context.CreateNew<MonsterDefinition>();
            monster.SetId(Guid.NewGuid());
            monster.Designation = designation;
            configuration.Monsters.Add(monster);
            monsters.Add(monster);
        }

        return (configuration, monsters);
    }
}

/// <summary>
/// Extensions to access the identifiers of the test objects.
/// </summary>
internal static class IdentifiableTestExtensions
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>The identifier.</returns>
    public static Guid GetId(this object obj) => ((IIdentifiable)obj).Id;

    /// <summary>
    /// Sets the identifier.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="id">The identifier.</param>
    public static void SetId(this object obj, Guid id) => ((IIdentifiable)obj).Id = id;
}
