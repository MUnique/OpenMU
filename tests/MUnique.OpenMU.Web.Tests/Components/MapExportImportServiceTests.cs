// <copyright file="MapExportImportServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using System.Text.Json;
using System.Text.Json.Nodes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.Shared.Components.MapEditor;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="MapExportImportService"/>.
/// </summary>
[TestFixture]
public class MapExportImportServiceTests
{
    private readonly MapExportImportService _service = new();

    private InMemoryPersistenceContextProvider _persistence = null!;
    private IContext _context = null!;

    /// <summary>
    /// Sets up the in-memory persistence context.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._persistence = new InMemoryPersistenceContextProvider();
        this._context = this._persistence.CreateNewContext();
    }

    /// <summary>
    /// Disposes the persistence context.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this._context.Dispose();
    }

    /// <summary>
    /// The export only contains the selected kinds and declares them.
    /// </summary>
    [Test]
    public void Export_ContainsOnlySelectedKinds()
    {
        var map = this.CreateMap(0);
        this.AddSpawn(map, this.CreateMonster(1, NpcObjectKind.Monster));
        this.AddSpawn(map, this.CreateMonster(2, NpcObjectKind.PassiveNpc));
        this.AddSpawn(map, this.CreateMonster(3, NpcObjectKind.Trap));

        var json = this._service.BuildExport([map], new HashSet<MapObjectKind> { MapObjectKind.Monsters });

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        Assert.That(root.GetProperty("formatVersion").GetString(), Is.EqualTo("2.0"));
        Assert.That(root.GetProperty("kinds").EnumerateArray().Select(kind => kind.GetString()), Is.EqualTo(new[] { "Monsters" }));

        var maps = root.GetProperty("maps").EnumerateArray().ToList();
        Assert.That(maps, Has.Count.EqualTo(1));
        Assert.That(maps[0].GetProperty("mapNumber").GetInt16(), Is.EqualTo((short)0));

        var spawns = maps[0].GetProperty("spawns").EnumerateArray().ToList();
        Assert.That(spawns, Has.Count.EqualTo(1));
        Assert.That(spawns[0].GetProperty("kind").GetString(), Is.EqualTo("Monsters"));
        Assert.That(spawns[0].GetProperty("monsterNumber").GetInt32(), Is.EqualTo(1));
        Assert.That(maps[0].GetProperty("exitGates").GetArrayLength(), Is.Zero);
        Assert.That(maps[0].GetProperty("enterGates").GetArrayLength(), Is.Zero);
    }

    /// <summary>
    /// An import only replaces the kinds which are selected by the user.
    /// </summary>
    [Test]
    public async Task Import_ReplacesOnlySelectedKindsAsync()
    {
        var source = this.CreateMap(5);
        this.AddSpawn(source, this.CreateMonster(1, NpcObjectKind.Monster), x1: 10);
        this.AddSpawn(source, this.CreateMonster(2, NpcObjectKind.PassiveNpc), x1: 20);
        var json = this._service.BuildExport(
            [source],
            new HashSet<MapObjectKind> { MapObjectKind.Monsters, MapObjectKind.Npcs });

        var target = this.CreateMap(7);
        var existingMonsterSpawn = this.AddSpawn(target, this.CreateMonster(3, NpcObjectKind.Monster), x1: 30);
        var existingNpcSpawn = this.AddSpawn(target, this.CreateMonster(4, NpcObjectKind.PassiveNpc), x1: 40);

        var preview = await this._service
            .PreviewAsync(json, [target], includeGates: false, this._context)
            .ConfigureAwait(false);

        Assert.That(preview.Error, Is.Null);
        await this._service
            .ApplyAsync(preview, new HashSet<MapObjectKind> { MapObjectKind.Monsters }, this._context)
            .ConfigureAwait(false);

        Assert.That(target.MonsterSpawns, Has.None.EqualTo(existingMonsterSpawn));
        Assert.That(target.MonsterSpawns, Has.Member(existingNpcSpawn), "Kinds which were not selected must stay untouched.");

        var imported = target.MonsterSpawns.Where(spawn => spawn.MonsterDefinition?.Number == 1).ToList();
        Assert.That(imported, Has.Count.EqualTo(1));
        Assert.That(imported[0].X1, Is.EqualTo(10));
    }

    /// <summary>
    /// A single-map file is imported into the currently selected map, even if its number differs.
    /// </summary>
    [Test]
    public async Task Import_SingleMapFile_IntoSelectedMapAsync()
    {
        var source = this.CreateMap(5);
        this.AddSpawn(source, this.CreateMonster(1, NpcObjectKind.Monster));
        var json = this._service.BuildExport([source], new HashSet<MapObjectKind> { MapObjectKind.Monsters });

        var target = this.CreateMap(7);
        var preview = await this._service
            .PreviewAsync(json, [target], includeGates: false, this._context)
            .ConfigureAwait(false);

        Assert.That(preview.Error, Is.Null);
        Assert.That(preview.Maps.Single().Map, Is.SameAs(target));

        await this._service
            .ApplyAsync(preview, new HashSet<MapObjectKind> { MapObjectKind.Monsters }, this._context)
            .ConfigureAwait(false);

        Assert.That(target.MonsterSpawns, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// An all-maps file is matched by the map numbers, and unknown maps are reported as warning.
    /// </summary>
    [Test]
    public async Task Import_AllMapsFile_MatchesByNumberAndWarnsAboutUnknownMapsAsync()
    {
        var monster = this.CreateMonster(1, NpcObjectKind.Monster);
        var sourceOne = this.CreateMap(1);
        this.AddSpawn(sourceOne, monster);
        var sourceUnknown = this.CreateMap(99);
        this.AddSpawn(sourceUnknown, monster);
        var json = this._service.BuildExport(
            [sourceOne, sourceUnknown],
            new HashSet<MapObjectKind> { MapObjectKind.Monsters });

        var targetOne = this.CreateMap(1);
        var targetTwo = this.CreateMap(2);
        var preview = await this._service
            .PreviewAsync(json, [targetOne, targetTwo], includeGates: false, this._context)
            .ConfigureAwait(false);

        Assert.That(preview.Error, Is.Null);
        Assert.That(preview.Maps.Select(map => map.Map), Is.EquivalentTo(new[] { targetOne }));
        Assert.That(
            preview.Warnings,
            Has.Some.Matches<MapImportWarning>(warning =>
                warning.Kind == MapImportWarningKind.UnknownMapNumber && warning.MapNumber == 99));
    }

    /// <summary>
    /// Spawns with unknown monsters or a mismatching kind are skipped with a warning.
    /// </summary>
    [Test]
    public async Task Import_SkipsUnknownMonstersAndKindMismatchesAsync()
    {
        var source = this.CreateMap(0);
        this.AddSpawn(source, this.CreateMonster(1, NpcObjectKind.Monster));
        this.AddSpawn(source, this.CreateMonster(2, NpcObjectKind.PassiveNpc));
        var json = this._service.BuildExport(
            [source],
            new HashSet<MapObjectKind> { MapObjectKind.Monsters, MapObjectKind.Npcs });

        // Monster 999 is unknown, and monster 2 is a NPC although the file declares it as monster.
        var file = JsonNode.Parse(json)!.AsObject();
        var spawns = file["maps"]![0]!["spawns"]!.AsArray();
        spawns.Add(new JsonObject
        {
            ["kind"] = nameof(MapObjectKind.Monsters),
            ["monsterNumber"] = 999,
            ["quantity"] = 1,
        });
        spawns.Single(spawn => spawn!["monsterNumber"]!.GetValue<int>() == 2)!["kind"] = nameof(MapObjectKind.Monsters);

        var preview = await this._service
            .PreviewAsync(file.ToJsonString(), [this.CreateMap(0)], includeGates: false, this._context)
            .ConfigureAwait(false);

        Assert.That(preview.Error, Is.Null);
        Assert.That(
            preview.Warnings,
            Has.Some.Matches<MapImportWarning>(warning =>
                warning.Kind == MapImportWarningKind.UnknownMonsterNumber && warning.MonsterNumber == 999));
        Assert.That(
            preview.Warnings,
            Has.Some.Matches<MapImportWarning>(warning =>
                warning.Kind == MapImportWarningKind.KindMismatch
                && warning.MonsterNumber == 2
                && warning.ExpectedKind == MapObjectKind.Monsters
                && warning.ActualKind == MapObjectKind.Npcs));
    }

    /// <summary>
    /// The export writes file-local gate keys, and enter gates refer to their target exit gate.
    /// </summary>
    [Test]
    public void Export_Gates_UseFileLocalKeys()
    {
        var mapOne = this.CreateMap(1);
        var mapTwo = this.CreateMap(2);
        var exitGate = this.AddExitGate(mapOne, x1: 10);
        this.AddEnterGate(mapTwo, exitGate, number: 7);

        var json = this._service.BuildExport([mapOne, mapTwo], new HashSet<MapObjectKind> { MapObjectKind.Gates });

        using var document = JsonDocument.Parse(json);
        var maps = document.RootElement.GetProperty("maps").EnumerateArray().ToList();
        var exportedExitGate = maps[0].GetProperty("exitGates").EnumerateArray().Single();
        var exportedEnterGate = maps[1].GetProperty("enterGates").EnumerateArray().Single();

        Assert.That(exportedExitGate.GetProperty("key").GetInt32(), Is.EqualTo(1));
        Assert.That(exportedEnterGate.GetProperty("targetGateKey").GetInt32(), Is.EqualTo(1));
        Assert.That(exportedEnterGate.GetProperty("number").GetInt16(), Is.EqualTo((short)7));
    }

    /// <summary>
    /// Importing gates updates matching exit gates in place, so external references stay intact,
    /// and creates the enter gates which target them.
    /// </summary>
    [Test]
    public async Task Import_Gates_UpdatesExitGatesInPlaceAndCreatesEnterGatesAsync()
    {
        var sourceOne = this.CreateMap(1);
        var sourceTwo = this.CreateMap(2);
        var sourceExitGate = this.AddExitGate(sourceOne, x1: 10);
        this.AddEnterGate(sourceTwo, sourceExitGate, number: 7);
        var json = this._service.BuildExport(
            [sourceOne, sourceTwo],
            new HashSet<MapObjectKind> { MapObjectKind.Gates });

        var targetOne = this.CreateMap(1);
        var targetTwo = this.CreateMap(2);
        var existingExitGate = this.AddExitGate(targetOne, x1: 10);
        var obsoleteEnterGate = this.AddEnterGate(targetTwo, existingExitGate, number: 1);

        var preview = await this._service
            .PreviewAsync(json, [targetOne, targetTwo], includeGates: true, this._context)
            .ConfigureAwait(false);

        Assert.That(preview.Error, Is.Null);
        Assert.That(preview.DeclaredKinds, Is.EqualTo(new[] { MapObjectKind.Gates }));

        await this._service
            .ApplyAsync(preview, new HashSet<MapObjectKind> { MapObjectKind.Gates }, this._context)
            .ConfigureAwait(false);

        Assert.That(targetOne.ExitGates, Has.Count.EqualTo(1));
        Assert.That(targetOne.ExitGates.Single(), Is.SameAs(existingExitGate), "Exit gates must be updated in place.");
        Assert.That(targetTwo.EnterGates, Has.None.EqualTo(obsoleteEnterGate));
        Assert.That(targetTwo.EnterGates, Has.Count.EqualTo(1));
        Assert.That(targetTwo.EnterGates.Single().Number, Is.EqualTo((short)7));
        Assert.That(targetTwo.EnterGates.Single().TargetGate, Is.SameAs(existingExitGate));
    }

    /// <summary>
    /// Gates are not touched when they are not selected for import.
    /// </summary>
    [Test]
    public async Task Import_DoesNotTouchGatesWhenNotSelectedAsync()
    {
        var source = this.CreateMap(0);
        var sourceExitGate = this.AddExitGate(source, x1: 10);
        this.AddEnterGate(source, sourceExitGate, number: 7);
        var json = this._service.BuildExport([source], new HashSet<MapObjectKind> { MapObjectKind.Gates });

        var target = this.CreateMap(0);
        var existingExitGate = this.AddExitGate(target, x1: 30);
        var existingEnterGate = this.AddEnterGate(target, existingExitGate, number: 3);

        var preview = await this._service
            .PreviewAsync(json, [target], includeGates: true, this._context)
            .ConfigureAwait(false);

        await this._service
            .ApplyAsync(preview, new HashSet<MapObjectKind>(), this._context)
            .ConfigureAwait(false);

        Assert.That(target.ExitGates, Is.EquivalentTo(new[] { existingExitGate }));
        Assert.That(target.EnterGates, Is.EquivalentTo(new[] { existingEnterGate }));
    }

    /// <summary>
    /// Invalid files produce a specific error instead of an exception.
    /// </summary>
    [Test]
    public async Task Preview_ReturnsErrorForInvalidFilesAsync()
    {
        var target = this.CreateMap(0);

        var invalidJson = await this._service
            .PreviewAsync("not json", [target], includeGates: false, this._context)
            .ConfigureAwait(false);
        Assert.That(invalidJson.Error, Is.EqualTo(MapImportError.InvalidJson));

        var unsupportedVersion = await this._service
            .PreviewAsync("""{ "formatVersion": "3.0", "kinds": ["Monsters"], "maps": [] }""", [target], includeGates: false, this._context)
            .ConfigureAwait(false);
        Assert.That(unsupportedVersion.Error, Is.EqualTo(MapImportError.UnsupportedFormatVersion));

        var noMaps = await this._service
            .PreviewAsync("""{ "formatVersion": "2.0", "kinds": ["Monsters"], "maps": [] }""", [target], includeGates: false, this._context)
            .ConfigureAwait(false);
        Assert.That(noMaps.Error, Is.EqualTo(MapImportError.NoMaps));

        var noKinds = await this._service
            .PreviewAsync("""{ "formatVersion": "2.0", "kinds": [], "maps": [ { "mapNumber": 0, "spawns": [] } ] }""", [target], includeGates: false, this._context)
            .ConfigureAwait(false);
        Assert.That(noKinds.Error, Is.EqualTo(MapImportError.NoKinds));
    }

    /// <summary>
    /// Importing gates into maps without any gates creates the exit gates and links the enter gates to them.
    /// </summary>
    [Test]
    public async Task Import_Gates_CreatesMissingExitGatesAndLinksEnterGatesAsync()
    {
        var sourceOne = this.CreateMap(1);
        var sourceTwo = this.CreateMap(2);
        var sourceExitGate = this.AddExitGate(sourceOne, x1: 10);
        sourceExitGate.Direction = Direction.South;
        sourceExitGate.IsSpawnGate = false;
        this.AddEnterGate(sourceTwo, sourceExitGate, number: 7);
        var json = this._service.BuildExport(
            [sourceOne, sourceTwo],
            new HashSet<MapObjectKind> { MapObjectKind.Gates });

        var targetOne = this.CreateMap(1);
        var targetTwo = this.CreateMap(2);
        var preview = await this._service
            .PreviewAsync(json, [targetOne, targetTwo], includeGates: true, this._context)
            .ConfigureAwait(false);
        await this._service
            .ApplyAsync(preview, new HashSet<MapObjectKind> { MapObjectKind.Gates }, this._context)
            .ConfigureAwait(false);

        var createdExitGate = targetOne.ExitGates.Single();
        Assert.That(createdExitGate.Map, Is.SameAs(targetOne));
        Assert.That(createdExitGate.X1, Is.EqualTo(10));
        Assert.That(createdExitGate.Direction, Is.EqualTo(Direction.South));
        Assert.That(createdExitGate.IsSpawnGate, Is.False);

        var createdEnterGate = targetTwo.EnterGates.Single();
        Assert.That(createdEnterGate.Number, Is.EqualTo((short)7));
        Assert.That(createdEnterGate.TargetGate, Is.SameAs(createdExitGate));
    }

    /// <summary>
    /// Two exit gates of the file with the same coordinates must not be merged into one existing gate.
    /// </summary>
    [Test]
    public async Task Import_Gates_DoesNotMapTwoFileGatesOntoOneExistingGateAsync()
    {
        var source = this.CreateMap(1);
        var firstGate = this.AddExitGate(source, x1: 10);
        firstGate.Direction = Direction.North;
        var secondGate = this.AddExitGate(source, x1: 10);
        secondGate.Direction = Direction.South;
        var json = this._service.BuildExport([source], new HashSet<MapObjectKind> { MapObjectKind.Gates });

        var target = this.CreateMap(1);
        var existingGate = this.AddExitGate(target, x1: 10);

        var preview = await this._service
            .PreviewAsync(json, [target, this.CreateMap(2)], includeGates: true, this._context)
            .ConfigureAwait(false);
        await this._service
            .ApplyAsync(preview, new HashSet<MapObjectKind> { MapObjectKind.Gates }, this._context)
            .ConfigureAwait(false);

        Assert.That(target.ExitGates, Has.Count.EqualTo(2));
        Assert.That(target.ExitGates, Has.Member(existingGate));
        Assert.That(target.ExitGates.Select(gate => gate.Direction), Is.EquivalentTo(new[] { Direction.North, Direction.South }));
    }

    /// <summary>
    /// The user can import only some of the kinds which are declared by the file; the others stay untouched.
    /// </summary>
    [Test]
    public async Task Import_OnlyNpcsSelected_LeavesMonstersUntouchedAsync()
    {
        var source = this.CreateMap(5);
        this.AddSpawn(source, this.CreateMonster(1, NpcObjectKind.Monster), x1: 10);
        this.AddSpawn(source, this.CreateMonster(2, NpcObjectKind.PassiveNpc), x1: 20);
        var json = this._service.BuildExport(
            [source],
            new HashSet<MapObjectKind> { MapObjectKind.Monsters, MapObjectKind.Npcs });

        var target = this.CreateMap(5);
        var existingMonsterSpawn = this.AddSpawn(target, this.CreateMonster(3, NpcObjectKind.Monster), x1: 30);
        var existingNpcSpawn = this.AddSpawn(target, this.CreateMonster(4, NpcObjectKind.PassiveNpc), x1: 40);

        var preview = await this._service
            .PreviewAsync(json, [target], includeGates: false, this._context)
            .ConfigureAwait(false);
        await this._service
            .ApplyAsync(preview, new HashSet<MapObjectKind> { MapObjectKind.Npcs }, this._context)
            .ConfigureAwait(false);

        Assert.That(target.MonsterSpawns, Has.Member(existingMonsterSpawn), "Monsters were not selected.");
        Assert.That(target.MonsterSpawns, Has.None.EqualTo(existingNpcSpawn));
        Assert.That(target.MonsterSpawns.Where(spawn => spawn.MonsterDefinition?.Number == 2), Has.Exactly(1).Items);
        Assert.That(target.MonsterSpawns.Where(spawn => spawn.MonsterDefinition?.Number == 1), Is.Empty);
    }

    /// <summary>
    /// A kind which is selected, but not declared by the file, is ignored.
    /// </summary>
    [Test]
    public async Task Import_IgnoresSelectedKindsWhichAreNotDeclaredAsync()
    {
        var source = this.CreateMap(5);
        this.AddSpawn(source, this.CreateMonster(1, NpcObjectKind.Monster));
        var json = this._service.BuildExport([source], new HashSet<MapObjectKind> { MapObjectKind.Monsters });

        var target = this.CreateMap(5);
        var existingNpcSpawn = this.AddSpawn(target, this.CreateMonster(4, NpcObjectKind.PassiveNpc), x1: 40);

        var preview = await this._service
            .PreviewAsync(json, [target], includeGates: false, this._context)
            .ConfigureAwait(false);
        await this._service
            .ApplyAsync(
                preview,
                new HashSet<MapObjectKind> { MapObjectKind.Monsters, MapObjectKind.Npcs },
                this._context)
            .ConfigureAwait(false);

        Assert.That(preview.DeclaredKinds, Is.EqualTo(new[] { MapObjectKind.Monsters }));
        Assert.That(target.MonsterSpawns, Has.Member(existingNpcSpawn));
        Assert.That(target.MonsterSpawns, Has.Count.EqualTo(2));
    }

    /// <summary>
    /// The preview counts the importable objects per kind, so the selection can show what will be imported.
    /// </summary>
    [Test]
    public async Task Preview_CountsObjectsPerKindAsync()
    {
        var source = this.CreateMap(1);
        var monster = this.CreateMonster(1, NpcObjectKind.Monster);
        this.AddSpawn(source, monster, x1: 1);
        this.AddSpawn(source, monster, x1: 2);
        this.AddSpawn(source, this.CreateMonster(2, NpcObjectKind.PassiveNpc), x1: 3);
        var exitGate = this.AddExitGate(source, x1: 10);
        this.AddEnterGate(source, exitGate, number: 7);
        var json = this._service.BuildExport(
            [source],
            new HashSet<MapObjectKind>(MapObjectKindExtensions.All));

        var preview = await this._service
            .PreviewAsync(json, [this.CreateMap(1), this.CreateMap(2)], includeGates: true, this._context)
            .ConfigureAwait(false);

        Assert.That(preview.GetCount(MapObjectKind.Monsters), Is.EqualTo(2));
        Assert.That(preview.GetCount(MapObjectKind.Npcs), Is.EqualTo(1));
        Assert.That(preview.GetCount(MapObjectKind.Others), Is.Zero);
        Assert.That(preview.GetCount(MapObjectKind.Gates), Is.EqualTo(2));
        Assert.That(preview.CountImported(new HashSet<MapObjectKind> { MapObjectKind.Monsters }), Is.EqualTo(2));
        Assert.That(
            preview.CountImported(new HashSet<MapObjectKind> { MapObjectKind.Monsters, MapObjectKind.Gates }),
            Is.EqualTo(4));
        Assert.That(preview.CountImported(new HashSet<MapObjectKind>()), Is.Zero);
    }

    /// <summary>
    /// Files of the former format version 1.0 are not supported anymore and have to be converted.
    /// </summary>
    [Test]
    public async Task Preview_RejectsFormatVersionOneAsync()
    {
        var target = this.CreateMap(0);
        var preview = await this._service
            .PreviewAsync("""{ "formatVersion": "1.0", "spawns": [ { "monsterNumber": 1, "quantity": 1 } ] }""", [target], includeGates: false, this._context)
            .ConfigureAwait(false);

        Assert.That(preview.Error, Is.EqualTo(MapImportError.UnsupportedFormatVersion));
        Assert.That(preview.Maps, Is.Empty);
    }

    /// <summary>
    /// Spawns without kind, and maps without number, are rejected instead of being guessed.
    /// </summary>
    [Test]
    public async Task Preview_RejectsMissingKindAndMapNumberAsync()
    {
        var target = this.CreateMap(0);

        var missingKind = await this._service
            .PreviewAsync("""{ "formatVersion": "2.0", "kinds": ["Monsters"], "maps": [ { "mapNumber": 0, "spawns": [ { "monsterNumber": 1 } ] } ] }""", [target], includeGates: false, this._context)
            .ConfigureAwait(false);
        Assert.That(missingKind.Error, Is.EqualTo(MapImportError.InvalidJson));

        var missingMapNumber = await this._service
            .PreviewAsync("""{ "formatVersion": "2.0", "kinds": ["Monsters"], "maps": [ { "spawns": [] } ] }""", [target], includeGates: false, this._context)
            .ConfigureAwait(false);
        Assert.That(missingMapNumber.Error, Is.EqualTo(MapImportError.InvalidJson));
    }

    /// <summary>
    /// A spawn of a kind which is not declared by the file is skipped and reported.
    /// </summary>
    [Test]
    public async Task Preview_SkipsSpawnsOfKindsWhichAreNotDeclaredAsync()
    {
        var source = this.CreateMap(0);
        this.AddSpawn(source, this.CreateMonster(1, NpcObjectKind.Monster));
        this.AddSpawn(source, this.CreateMonster(2, NpcObjectKind.PassiveNpc));
        var json = this._service.BuildExport(
            [source],
            new HashSet<MapObjectKind> { MapObjectKind.Monsters, MapObjectKind.Npcs });

        var file = JsonNode.Parse(json)!.AsObject();
        file["kinds"] = new JsonArray(nameof(MapObjectKind.Monsters));

        var preview = await this._service
            .PreviewAsync(file.ToJsonString(), [this.CreateMap(0)], includeGates: false, this._context)
            .ConfigureAwait(false);

        Assert.That(preview.Error, Is.Null);
        Assert.That(preview.GetCount(MapObjectKind.Monsters), Is.EqualTo(1));
        Assert.That(preview.GetCount(MapObjectKind.Npcs), Is.Zero);
        Assert.That(
            preview.Warnings,
            Has.Some.Matches<MapImportWarning>(warning =>
                warning.Kind == MapImportWarningKind.KindNotDeclared
                && warning.MonsterNumber == 2
                && warning.ActualKind == MapObjectKind.Npcs));
    }

    /// <summary>
    /// Maps can share their number (e.g. the Devil Squares). They must not crash the import,
    /// and they are paired by their order of occurrence.
    /// </summary>
    [Test]
    public async Task Import_AllMapsFile_PairsMapsWithTheSameNumberByOrderAsync()
    {
        var monster = this.CreateMonster(1, NpcObjectKind.Monster);
        var sourceFirst = this.CreateMap(9);
        var sourceSecond = this.CreateMap(9);
        this.AddSpawn(sourceFirst, monster, x1: 1);
        this.AddSpawn(sourceSecond, monster, x1: 2);
        var json = this._service.BuildExport(
            [sourceFirst, sourceSecond],
            new HashSet<MapObjectKind> { MapObjectKind.Monsters });

        var targetFirst = this.CreateMap(9);
        var targetSecond = this.CreateMap(9);
        var preview = await this._service
            .PreviewAsync(json, [targetFirst, targetSecond], includeGates: false, this._context)
            .ConfigureAwait(false);

        Assert.That(preview.Error, Is.Null);
        Assert.That(preview.Warnings, Is.Empty);
        Assert.That(preview.Maps.Select(map => map.Map), Is.EqualTo(new[] { targetFirst, targetSecond }));

        await this._service
            .ApplyAsync(preview, new HashSet<MapObjectKind> { MapObjectKind.Monsters }, this._context)
            .ConfigureAwait(false);

        Assert.That(targetFirst.MonsterSpawns.Single().X1, Is.EqualTo(1));
        Assert.That(targetSecond.MonsterSpawns.Single().X1, Is.EqualTo(2));
    }

    /// <summary>
    /// Importing an all-maps file into one map picks the entry which belongs to the selected map,
    /// even if several maps share its number.
    /// </summary>
    [Test]
    public async Task Import_AllMapsFile_IntoSingleMapWithSharedNumberAsync()
    {
        var monster = this.CreateMonster(1, NpcObjectKind.Monster);
        var sourceFirst = this.CreateMap(9);
        var sourceSecond = this.CreateMap(9);
        this.AddSpawn(sourceFirst, monster, x1: 1);
        this.AddSpawn(sourceSecond, monster, x1: 2);
        var json = this._service.BuildExport(
            [sourceFirst, sourceSecond],
            new HashSet<MapObjectKind> { MapObjectKind.Monsters });

        var targetFirst = this.CreateMap(9);
        var targetSecond = this.CreateMap(9);
        var preview = await this._service
            .PreviewAsync(json, [targetSecond], includeGates: false, this._context, [targetFirst, targetSecond])
            .ConfigureAwait(false);

        Assert.That(preview.Error, Is.Null);
        Assert.That(preview.Maps.Single().Map, Is.SameAs(targetSecond));

        await this._service
            .ApplyAsync(preview, new HashSet<MapObjectKind> { MapObjectKind.Monsters }, this._context)
            .ConfigureAwait(false);

        Assert.That(targetFirst.MonsterSpawns, Is.Empty);
        Assert.That(targetSecond.MonsterSpawns.Single().X1, Is.EqualTo(2));
    }

    /// <summary>
    /// Entries of the file without a counterpart of the same number in the target database are reported.
    /// </summary>
    [Test]
    public async Task Import_AllMapsFile_WarnsAboutSurplusMapsWithTheSameNumberAsync()
    {
        var monster = this.CreateMonster(1, NpcObjectKind.Monster);
        var sourceFirst = this.CreateMap(9);
        var sourceSecond = this.CreateMap(9);
        this.AddSpawn(sourceFirst, monster);
        this.AddSpawn(sourceSecond, monster);
        var json = this._service.BuildExport(
            [sourceFirst, sourceSecond],
            new HashSet<MapObjectKind> { MapObjectKind.Monsters });

        var targetNine = this.CreateMap(9);
        var preview = await this._service
            .PreviewAsync(json, [targetNine, this.CreateMap(1)], includeGates: false, this._context)
            .ConfigureAwait(false);

        Assert.That(preview.Error, Is.Null);
        Assert.That(preview.Maps.Select(map => map.Map), Is.EquivalentTo(new[] { targetNine }));
        Assert.That(
            preview.Warnings,
            Has.Exactly(1).Matches<MapImportWarning>(warning =>
                warning.Kind == MapImportWarningKind.UnknownMapNumber && warning.MapNumber == 9));
    }

    private BasicModel.GameMapDefinition CreateMap(short number)
    {
        var map = this._context.CreateNew<BasicModel.GameMapDefinition>();
        map.Number = number;
        return map;
    }

    private BasicModel.MonsterDefinition CreateMonster(short number, NpcObjectKind objectKind)
    {
        var monster = this._context.CreateNew<BasicModel.MonsterDefinition>();
        monster.Number = number;
        monster.ObjectKind = objectKind;
        return monster;
    }

    private BasicModel.MonsterSpawnArea AddSpawn(
        BasicModel.GameMapDefinition map,
        BasicModel.MonsterDefinition monster,
        byte x1 = 0)
    {
        var spawn = this._context.CreateNew<BasicModel.MonsterSpawnArea>();
        spawn.GameMap = map;
        spawn.MonsterDefinition = monster;
        spawn.X1 = x1;
        spawn.Y1 = 0;
        spawn.X2 = (byte)(x1 + 1);
        spawn.Y2 = 1;
        spawn.Quantity = 1;
        map.MonsterSpawns.Add(spawn);
        return spawn;
    }

    private BasicModel.ExitGate AddExitGate(BasicModel.GameMapDefinition map, byte x1)
    {
        var gate = this._context.CreateNew<BasicModel.ExitGate>();
        gate.Map = map;
        gate.X1 = x1;
        gate.Y1 = 0;
        gate.X2 = (byte)(x1 + 1);
        gate.Y2 = 1;
        gate.IsSpawnGate = true;
        map.ExitGates.Add(gate);
        return gate;
    }

    private BasicModel.EnterGate AddEnterGate(
        BasicModel.GameMapDefinition map,
        BasicModel.ExitGate targetGate,
        short number)
    {
        var gate = this._context.CreateNew<BasicModel.EnterGate>();
        gate.TargetGate = targetGate;
        gate.Number = number;
        gate.X1 = 5;
        gate.Y1 = 5;
        gate.X2 = 6;
        gate.Y2 = 6;
        map.EnterGates.Add(gate);
        return gate;
    }
}
