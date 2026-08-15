// <copyright file="CastleSiegeStateMachineTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the Castle Siege weekly state machine.
/// </summary>
[TestFixture]
public class CastleSiegeStateMachineTests
{
    /// <summary>
    /// Verifies that the plug-in manager discovers and activates the Castle Siege periodic task.
    /// </summary>
    [Test]
    public void PlugInIsDiscoveredAndActivated()
    {
        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);

        manager.DiscoverAndRegisterPlugIns(typeof(CastleSiegePlugIn).Assembly);
        manager.ActivatePlugIn<CastleSiegePlugIn>();

        Assert.That(manager.GetKnownPlugInsOf<IPeriodicTaskPlugIn>(), Does.Contain(typeof(CastleSiegePlugIn)));
        Assert.That(manager.GetActivePlugInsOf<IPeriodicTaskPlugIn>(), Has.Some.InstanceOf<CastleSiegePlugIn>());
    }

    /// <summary>
    /// Verifies that the current state is calculated from the configured UTC week.
    /// </summary>
    /// <param name="utcTime">The current UTC time.</param>
    /// <param name="expectedState">The expected state.</param>
    [TestCase("2026-08-02T12:00:00Z", CastleSiegeState.Idle1)]
    [TestCase("2026-08-03T12:00:00Z", CastleSiegeState.RegisterGuild)]
    [TestCase("2026-08-08T19:00:00Z", CastleSiegeState.Ready)]
    [TestCase("2026-08-08T21:00:00Z", CastleSiegeState.Start)]
    [TestCase("2026-08-08T22:03:00Z", CastleSiegeState.End)]
    [TestCase("2026-08-08T23:00:00Z", CastleSiegeState.EndCycle)]
    public void CurrentStateIsCalculatedFromWeeklySchedule(string utcTime, CastleSiegeState expectedState)
    {
        var schedule = new CastleSiegeSchedule(CreateSchedule());

        var period = schedule.GetCurrentPeriod(DateTime.Parse(utcTime, null, System.Globalization.DateTimeStyles.AdjustToUniversal));

        Assert.That(period.State, Is.EqualTo(expectedState));
        Assert.That(period.StartUtc, Is.LessThanOrEqualTo(DateTime.Parse(utcTime, null, System.Globalization.DateTimeStyles.AdjustToUniversal)));
        Assert.That(period.EndUtc, Is.GreaterThan(DateTime.Parse(utcTime, null, System.Globalization.DateTimeStyles.AdjustToUniversal)));
    }

    /// <summary>
    /// Verifies that the early registration period survives a restart through the scheduled idle and registration states.
    /// </summary>
    /// <param name="utcTime">The restart time.</param>
    [TestCase("2026-08-09T12:00:00Z")]
    [TestCase("2026-08-10T12:00:00Z")]
    public void EarlyRegistrationPeriodIsRecoveredAcrossRestart(string utcTime)
    {
        var schedule = new CastleSiegeSchedule(CreateSchedule());
        var period = schedule.GetCurrentEventPeriod(
            DateTime.Parse(utcTime, null, System.Globalization.DateTimeStyles.AdjustToUniversal));

        Assert.Multiple(() =>
        {
            Assert.That(period.State, Is.EqualTo(CastleSiegeState.RegisterGuild));
            Assert.That(period.StartUtc, Is.EqualTo(new DateTime(2026, 8, 8, 22, 5, 0, DateTimeKind.Utc)));
            Assert.That(period.EndUtc, Is.EqualTo(new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc)));
        });
    }

    /// <summary>
    /// Verifies that the schedule wraps from the last state to Sunday.
    /// </summary>
    [Test]
    public void LastStateEndsAtStartOfNextWeek()
    {
        var schedule = new CastleSiegeSchedule(CreateSchedule());
        var utcNow = new DateTime(2026, 8, 8, 23, 0, 0, DateTimeKind.Utc);

        var period = schedule.GetCurrentPeriod(utcNow);

        Assert.That(period.State, Is.EqualTo(CastleSiegeState.EndCycle));
        Assert.That(period.EndUtc, Is.EqualTo(new DateTime(2026, 8, 9, 0, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Verifies that duplicate schedule states are rejected.
    /// </summary>
    [Test]
    public void DuplicateScheduleStateIsRejected()
    {
        var schedule = CreateSchedule().ToList();
        schedule.Add(new CastleSiegeStateScheduleEntry
        {
            State = CastleSiegeState.Start,
            DayOfWeek = DayOfWeek.Saturday,
            Hour = 23,
        });

        Assert.That(() => new CastleSiegeSchedule(schedule), Throws.InvalidOperationException);
    }

    /// <summary>
    /// Verifies that persistent state and registrations are loaded and state changes can be saved.
    /// </summary>
    [Test]
    public async ValueTask ContextLoadsAndSavesPersistentStateAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var context = new CastleSiegeContext(fixture.GameContext.Object, fixture.CastleSiegeConfiguration);

        await context.InitializeAsync(new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);

        Assert.That(context.SiegeData.TributeMoney, Is.EqualTo(42));
        Assert.That(context.RegisteredGuilds.ContainsKey(fixture.RegisteredGuildId), Is.True);

        context.SiegeData.TributeMoney = 1234;
        await context.SaveOwnerAsync().ConfigureAwait(false);

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeData),
            false,
            fixture.GameConfiguration);
        var savedData = (await persistenceContext.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
        Assert.That(savedData.TributeMoney, Is.EqualTo(1234));
    }

    /// <summary>
    /// Verifies that loading does not silently create a missing persistent state.
    /// </summary>
    [Test]
    public async ValueTask ContextLoadDoesNotCreateMissingPersistentStateAsync()
    {
        var fixture = await CreateFixtureAsync(false).ConfigureAwait(false);
        var context = new CastleSiegeContext(fixture.GameContext.Object, fixture.CastleSiegeConfiguration);

        Assert.ThrowsAsync<InvalidOperationException>(async () => await context.LoadAsync().ConfigureAwait(false));

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeData),
            false,
            fixture.GameConfiguration);
        Assert.That(await persistenceContext.GetAsync<CastleSiegeData>().ConfigureAwait(false), Is.Empty);
    }

    /// <summary>
    /// Verifies that initialization explicitly creates a missing persistent state.
    /// </summary>
    [Test]
    public async ValueTask ContextInitializationCreatesMissingPersistentStateAsync()
    {
        var fixture = await CreateFixtureAsync(false).ConfigureAwait(false);
        var context = new CastleSiegeContext(fixture.GameContext.Object, fixture.CastleSiegeConfiguration);

        await context.InitializeAsync(new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeData),
            false,
            fixture.GameConfiguration);
        var savedData = (await persistenceContext.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
        Assert.That(context.SiegeData.Id, Is.EqualTo(savedData.Id));
    }

    /// <summary>
    /// Verifies that all database-persisted NPC runtime states are stored, including destroyed and unspawned structures.
    /// </summary>
    [Test]
    public async ValueTask ContextSavesPersistentNpcStatesIncludingDestroyedAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var gateStateId = await AddNpcStateAsync(fixture, 277, 0, 250_000).ConfigureAwait(false);
        var statueStateId = await AddNpcStateAsync(fixture, 283, 0, 300_000).ConfigureAwait(false);
        var context = new CastleSiegeContext(fixture.GameContext.Object, fixture.CastleSiegeConfiguration);
        await context.InitializeAsync(new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);
        context.ActiveNpcs.Add(CreateNpcRuntime(277, 0, 125_000, true, true));
        context.ActiveNpcs.Add(CreateNpcRuntime(283, 0, 0, true, false));
        context.ActiveNpcs.Add(CreateNpcRuntime(216, 0, 1_000, false, true));

        await context.SaveNpcStatesAsync().ConfigureAwait(false);

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeData),
            false,
            fixture.GameConfiguration);
        var savedData = (await persistenceContext.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
        Assert.That(savedData.NpcStates, Has.Count.EqualTo(2));
        Assert.That(savedData.NpcStates.Select(state => state.MonsterNumber), Is.EquivalentTo(new short[] { 277, 283 }));
        Assert.Multiple(() =>
        {
            var gateState = savedData.NpcStates.Single(state => state.MonsterNumber == 277);
            var statueState = savedData.NpcStates.Single(state => state.MonsterNumber == 283);
            Assert.That(gateState.Id, Is.EqualTo(gateStateId));
            Assert.That(gateState.CurrentHp, Is.EqualTo(125_000));
            Assert.That(statueState.Id, Is.EqualTo(statueStateId));
            Assert.That(statueState.CurrentHp, Is.Zero);
        });
    }

    /// <summary>
    /// Verifies that an incomplete runtime snapshot does not delete persistent NPC states.
    /// </summary>
    [Test]
    public async ValueTask ContextPreservesNpcStatesForIncompleteRuntimeSnapshotAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var gateStateId = await AddNpcStateAsync(fixture, 277, 0, 250_000).ConfigureAwait(false);
        var context = new CastleSiegeContext(fixture.GameContext.Object, fixture.CastleSiegeConfiguration);
        await context.InitializeAsync(new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);
        context.ActiveNpcs.Add(new CastleSiegeNpcRuntime
        {
            Definition = new CastleSiegeNpcDefinition
            {
                MonsterDefinition = new MonsterDefinition { Number = 277 },
                InstanceId = 0,
                IsPersistedToDatabase = true,
            },
            IsAlive = true,
        });

        await context.SaveNpcStatesAsync().ConfigureAwait(false);

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeData),
            false,
            fixture.GameConfiguration);
        var savedState = (await persistenceContext.GetAsync<CastleSiegeData>().ConfigureAwait(false))
            .Single()
            .NpcStates
            .Single();
        Assert.Multiple(() =>
        {
            Assert.That(savedState.Id, Is.EqualTo(gateStateId));
            Assert.That(savedState.CurrentHp, Is.EqualTo(250_000));
        });
    }

    /// <summary>
    /// Verifies startup state calculation and an automatic state transition.
    /// </summary>
    [Test]
    public async ValueTask PlugInCalculatesAndAdvancesStateAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 8, 3, 12, 0, 0, TimeSpan.Zero));
        var plugIn = new CastleSiegePlugIn(timeProvider);

        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);
        var context = plugIn.GetContext(fixture.GameContext.Object);

        Assert.That(context, Is.Not.Null);
        Assert.That(context!.CurrentState, Is.EqualTo(CastleSiegeState.RegisterGuild));
        Assert.That(context.StateEndTimeUtc, Is.EqualTo(new DateTime(2026, 8, 4, 0, 0, 0, DateTimeKind.Utc)));

        timeProvider.Advance(TimeSpan.FromHours(12));
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);

        Assert.That(context.CurrentState, Is.EqualTo(CastleSiegeState.Idle2));
    }

    /// <summary>
    /// Verifies that plug-in initialization restores the early registration period after a restart.
    /// </summary>
    [Test]
    public async ValueTask PlugInRecoversEarlyRegistrationAfterRestartAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 8, 9, 12, 0, 0, TimeSpan.Zero));
        var plugIn = new CastleSiegePlugIn(timeProvider);

        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);
        var context = plugIn.GetContext(fixture.GameContext.Object);

        Assert.Multiple(() =>
        {
            Assert.That(context, Is.Not.Null);
            Assert.That(context!.CurrentState, Is.EqualTo(CastleSiegeState.RegisterGuild));
            Assert.That(context.StateStartTimeUtc, Is.EqualTo(new DateTime(2026, 8, 8, 22, 5, 0, DateTimeKind.Utc)));
            Assert.That(context.StateEndTimeUtc, Is.EqualTo(new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc)));
        });
    }

    /// <summary>
    /// Verifies that forcing the event starts the battle for its configured duration.
    /// </summary>
    [Test]
    public async ValueTask ForceStartEntersStartStateAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var utcNow = new DateTimeOffset(2026, 8, 3, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = new ManualTimeProvider(utcNow);
        var plugIn = new CastleSiegePlugIn(timeProvider);
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);

        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);
        var context = plugIn.GetContext(fixture.GameContext.Object)!;

        Assert.That(context.CurrentState, Is.EqualTo(CastleSiegeState.Start));
        Assert.That(context.IsEventRunning, Is.True);
        Assert.That(context.StateStartTimeUtc, Is.EqualTo(utcNow.UtcDateTime));
        Assert.That(context.StateEndTimeUtc, Is.EqualTo(utcNow.AddHours(2).UtcDateTime));
    }

    /// <summary>
    /// Verifies that the event-running flag is set exclusively for the battle state.
    /// </summary>
    [Test]
    public async ValueTask EventIsRunningOnlyDuringStartStateAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var context = new CastleSiegeContext(fixture.GameContext.Object, fixture.CastleSiegeConfiguration);
        await context.InitializeAsync(new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);

        foreach (var state in Enum.GetValues<CastleSiegeState>())
        {
            context.CurrentState = state;
            Assert.That(
                context.IsEventRunning,
                Is.EqualTo(state == CastleSiegeState.Start),
                $"Unexpected event-running value for state {state}.");
        }
    }

    /// <summary>
    /// Verifies that the periodic NPC snapshot is scheduled when the server starts outside the battle phase.
    /// </summary>
    [Test]
    public async ValueTask PeriodicNpcSaveIsScheduledOnStartupAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        await AddNpcStateAsync(fixture, 277, 1, 1_000).ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 8, 3, 12, 0, 0, TimeSpan.Zero));
        var plugIn = new CastleSiegePlugIn(timeProvider);
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);
        var context = plugIn.GetContext(fixture.GameContext.Object)!;
        context.ActiveNpcs.Add(CreateNpcRuntime(277, 1, 500, true, true));

        timeProvider.Advance(TimeSpan.FromMinutes(2));
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeData),
            false,
            fixture.GameConfiguration);
        var savedData = (await persistenceContext.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
        Assert.That(savedData.NpcStates.Single(state => state.MonsterNumber == 277).CurrentHp, Is.EqualTo(500));
    }

    /// <summary>
    /// Verifies that EndCycle clears registrations and immediately opens the next registration phase.
    /// </summary>
    [Test]
    public async ValueTask EndCycleTransitionsDirectlyToRegistrationAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 8, 8, 22, 4, 0, TimeSpan.Zero));
        var plugIn = new CastleSiegePlugIn(timeProvider);
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);

        timeProvider.Advance(TimeSpan.FromMinutes(2));
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);
        var context = plugIn.GetContext(fixture.GameContext.Object)!;

        Assert.That(context.CurrentState, Is.EqualTo(CastleSiegeState.RegisterGuild));
        Assert.That(context.StateStartTimeUtc, Is.EqualTo(new DateTime(2026, 8, 8, 22, 5, 0, DateTimeKind.Utc)));
        Assert.That(context.StateEndTimeUtc, Is.EqualTo(new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc)));
        Assert.That(context.RegisteredGuilds, Is.Empty);

        using var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeGuildRegistration),
            false,
            fixture.GameConfiguration);
        Assert.That(await persistenceContext.GetAsync<CastleSiegeGuildRegistration>().ConfigureAwait(false), Is.Empty);
    }

    /// <summary>
    /// Verifies that regular notifications are not repeated on every timer tick.
    /// </summary>
    [Test]
    public async ValueTask RegistrationNotificationUsesConfiguredIntervalAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 8, 3, 0, 15, 0, TimeSpan.Zero));
        var plugIn = new CastleSiegePlugIn(timeProvider);

        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);
        timeProvider.Advance(TimeSpan.FromMinutes(19));
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);
        timeProvider.Advance(TimeSpan.FromMinutes(1));
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);

        Assert.That(fixture.Notifications, Has.Count.EqualTo(2));
        Assert.That(fixture.Notifications, Has.All.Contains("registration"));
    }

    /// <summary>
    /// Verifies the Ready-state notification at 30 minutes and every minute during the final five minutes.
    /// </summary>
    [Test]
    public async ValueTask ReadyNotificationsUseCountdownIntervalsAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 8, 8, 19, 29, 0, TimeSpan.Zero));
        var plugIn = new CastleSiegePlugIn(timeProvider);
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);

        timeProvider.Advance(TimeSpan.FromMinutes(1));
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);
        timeProvider.Advance(TimeSpan.FromMinutes(24));
        await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);
        for (var minute = 0; minute < 5; minute++)
        {
            timeProvider.Advance(TimeSpan.FromMinutes(1));
            await plugIn.ExecuteTaskAsync(fixture.GameContext.Object).ConfigureAwait(false);
        }

        Assert.That(fixture.Notifications, Has.Count.EqualTo(6));
        Assert.That(fixture.Notifications, Has.All.Contains("starts in"));
    }

    /// <summary>
    /// Verifies that a disabled or missing configuration is handled without creating runtime state.
    /// </summary>
    [Test]
    public async ValueTask MissingConfigurationIsIgnoredAsync()
    {
        var gameConfiguration = new MUnique.OpenMU.Persistence.BasicModel.GameConfiguration();
        var gameContext = new Mock<IGameContext>();
        gameContext.SetupGet(context => context.Configuration).Returns(gameConfiguration);
        var plugIn = new CastleSiegePlugIn();

        await plugIn.ExecuteTaskAsync(gameContext.Object).ConfigureAwait(false);

        Assert.That(plugIn.GetContext(gameContext.Object), Is.Null);
    }

    private static IEnumerable<CastleSiegeStateScheduleEntry> CreateSchedule()
    {
        yield return CreateScheduleEntry(CastleSiegeState.Idle1, DayOfWeek.Sunday, 0, 0);
        yield return CreateScheduleEntry(CastleSiegeState.RegisterGuild, DayOfWeek.Monday, 0, 0);
        yield return CreateScheduleEntry(CastleSiegeState.Idle2, DayOfWeek.Tuesday, 0, 0);
        yield return CreateScheduleEntry(CastleSiegeState.RegisterMark, DayOfWeek.Wednesday, 0, 0);
        yield return CreateScheduleEntry(CastleSiegeState.Idle3, DayOfWeek.Thursday, 0, 0);
        yield return CreateScheduleEntry(CastleSiegeState.Notify, DayOfWeek.Friday, 0, 0);
        yield return CreateScheduleEntry(CastleSiegeState.Ready, DayOfWeek.Saturday, 18, 0);
        yield return CreateScheduleEntry(CastleSiegeState.Start, DayOfWeek.Saturday, 20, 0);
        yield return CreateScheduleEntry(CastleSiegeState.End, DayOfWeek.Saturday, 22, 0);
        yield return CreateScheduleEntry(CastleSiegeState.EndCycle, DayOfWeek.Saturday, 22, 5);
    }

    private static CastleSiegeStateScheduleEntry CreateScheduleEntry(CastleSiegeState state, DayOfWeek dayOfWeek, byte hour, byte minute)
    {
        return new CastleSiegeStateScheduleEntry
        {
            State = state,
            DayOfWeek = dayOfWeek,
            Hour = hour,
            Minute = minute,
        };
    }

    private static CastleSiegeNpcRuntime CreateNpcRuntime(short monsterNumber, byte instanceId, int currentHp, bool isPersisted, bool isAlive)
    {
        return new CastleSiegeNpcRuntime
        {
            Definition = new CastleSiegeNpcDefinition
            {
                MonsterDefinition = new MonsterDefinition { Number = monsterNumber },
                InstanceId = instanceId,
                IsPersistedToDatabase = isPersisted,
            },
            PersistedState = new CastleSiegeNpcState
            {
                MonsterNumber = monsterNumber,
                InstanceId = instanceId,
                CurrentHp = currentHp,
            },
            IsAlive = isAlive,
        };
    }

    private static async ValueTask<Guid> AddNpcStateAsync(
        TestFixture fixture,
        short monsterNumber,
        byte instanceId,
        int currentHp)
    {
        using var context = fixture.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegeData),
            false,
            fixture.GameConfiguration);
        var siegeData = (await context.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
        var npcState = context.CreateNew<CastleSiegeNpcState>();
        npcState.MonsterNumber = monsterNumber;
        npcState.InstanceId = instanceId;
        npcState.CurrentHp = currentHp;
        siegeData.NpcStates.Add(npcState);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return npcState.Id;
    }

    private static async ValueTask<TestFixture> CreateFixtureAsync(bool createSiegeData = true)
    {
        var persistenceContextProvider = new InMemoryPersistenceContextProvider();
        using var context = persistenceContextProvider.CreateNewContext();
        var gameConfiguration = context.CreateNew<MUnique.OpenMU.Persistence.BasicModel.GameConfiguration>();
        var castleSiegeConfiguration = context.CreateNew<MUnique.OpenMU.Persistence.BasicModel.CastleSiegeConfiguration>();
        castleSiegeConfiguration.Enabled = true;
        foreach (var scheduleEntry in CreateSchedule())
        {
            var persistentEntry = context.CreateNew<CastleSiegeStateScheduleEntry>();
            persistentEntry.State = scheduleEntry.State;
            persistentEntry.DayOfWeek = scheduleEntry.DayOfWeek;
            persistentEntry.Hour = scheduleEntry.Hour;
            persistentEntry.Minute = scheduleEntry.Minute;
            castleSiegeConfiguration.StateSchedule.Add(persistentEntry);
        }

        gameConfiguration.CastleSiegeConfiguration = castleSiegeConfiguration;

        if (createSiegeData)
        {
            var siegeData = context.CreateNew<CastleSiegeData>();
            siegeData.TributeMoney = 42;
        }

        var registeredGuildId = Guid.NewGuid();
        var registration = context.CreateNew<CastleSiegeGuildRegistration>();
        registration.GuildId = registeredGuildId;
        registration.GuildName = "Test";
        await context.SaveChangesAsync().ConfigureAwait(false);

        var notifications = new List<string>();
        var gameContext = new Mock<IGameContext>();
        gameContext.SetupGet(game => game.Configuration).Returns(gameConfiguration);
        gameContext.SetupGet(game => game.PersistenceContextProvider).Returns(persistenceContextProvider);
        gameContext.SetupGet(game => game.LoggerFactory).Returns(NullLoggerFactory.Instance);
        gameContext
            .Setup(game => game.SendGlobalNotificationAsync(It.IsAny<string>()))
            .Callback<string>(notifications.Add)
            .Returns(ValueTask.CompletedTask);

        return new(
            persistenceContextProvider,
            gameConfiguration,
            castleSiegeConfiguration,
            gameContext,
            registeredGuildId,
            notifications);
    }

    private sealed record TestFixture(
        InMemoryPersistenceContextProvider PersistenceContextProvider,
        GameConfiguration GameConfiguration,
        CastleSiegeConfiguration CastleSiegeConfiguration,
        Mock<IGameContext> GameContext,
        Guid RegisteredGuildId,
        List<string> Notifications);

    private sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public ManualTimeProvider(DateTimeOffset utcNow)
        {
            this._utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => this._utcNow;

        public void Advance(TimeSpan duration)
        {
            this._utcNow += duration;
        }
    }
}
