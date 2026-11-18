// <copyright file="ReferenceConfigurationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the <see cref="ReferenceConfiguration"/>, which derives values from the data of a data initialization once per process.
/// The derived values are kept for the process, so each test uses its own data initialization with its own key.
/// </summary>
[TestFixture]
internal class ReferenceConfigurationTest
{
    /// <summary>The maximum level of the configurations which the test data initializations create.</summary>
    private const short MaximumLevel = 123;

    /// <summary>
    /// Gets the executions of the test data initializations, by their key.
    /// </summary>
    private static ConcurrentDictionary<string, int> Executions { get; } = new();

    /// <summary>
    /// Two values of the same type but different names are both derived, from one execution of the data initialization;
    /// a second request of a name returns the value of the first, without deriving again.
    /// </summary>
    [Test]
    public async Task ValuesAreDerivedOncePerNameFromOneInitializationAsync()
    {
        var plugInManager = CreatePlugInManager(new DerivingTwiceInitialization(new InMemoryPersistenceContextProvider(), NullLoggerFactory.Instance));
        var key = DerivingTwiceInitialization.TestKey;

        var first = await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "first", c => new[] { c.MaximumLevel }).ConfigureAwait(false);
        var second = await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "second", c => new[] { c.MaximumLevel + 1 }).ConfigureAwait(false);
        var firstAgain = await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "first", _ => new short[] { -1 }).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(first, Is.EqualTo(new[] { MaximumLevel }));
            Assert.That(second, Is.EqualTo(new[] { MaximumLevel + 1 }));
            Assert.That(firstAgain, Is.SameAs(first));
            Assert.That(Executions[key], Is.LessThanOrEqualTo(2), "the configuration is only kept weakly, so a collection in between may create it again");
        });
    }

    /// <summary>
    /// A failing derivation keeps nothing and is remembered, as it can't heal: a later request of the name fails without
    /// deriving again, with a new exception which contains the original one, while another name of the same data still works.
    /// </summary>
    [Test]
    public async Task FailingDerivationIsRememberedAsync()
    {
        var plugInManager = CreatePlugInManager(new FailingDerivationInitialization(new InMemoryPersistenceContextProvider(), NullLoggerFactory.Instance));
        var key = FailingDerivationInitialization.TestKey;
        var derivations = 0;

        var first = Assert.ThrowsAsync<ReferenceConfigurationFailedException>(async () => await ReferenceConfiguration.GetDerivedAsync<int[]>(plugInManager, NullLoggerFactory.Instance, key, "failing", _ => throw new ArgumentException("broken")).ConfigureAwait(false));
        var second = Assert.ThrowsAsync<ReferenceConfigurationFailedException>(async () => await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "failing", _ => new[] { ++derivations }).ConfigureAwait(false));
        var other = await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "other", c => new[] { c.MaximumLevel }).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(derivations, Is.Zero);
            Assert.That(other, Is.EqualTo(new[] { MaximumLevel }));
            Assert.That(second, Is.Not.SameAs(first));
            Assert.That(second!.InnerException, Is.InstanceOf<ArgumentException>().And.SameAs(first!.InnerException));
        });
    }

    /// <summary>
    /// A derivation which runs out of memory may work on the next request, so its failure isn't remembered.
    /// </summary>
    [Test]
    public async Task DerivationOutOfMemoryIsTriedAgainAsync()
    {
        var plugInManager = CreatePlugInManager(new OutOfMemoryDerivationInitialization(new InMemoryPersistenceContextProvider(), NullLoggerFactory.Instance));
        var key = OutOfMemoryDerivationInitialization.TestKey;

        Assert.ThrowsAsync<OutOfMemoryException>(async () => await ReferenceConfiguration.GetDerivedAsync<int[]>(plugInManager, NullLoggerFactory.Instance, key, "derived", _ => throw new OutOfMemoryException()).ConfigureAwait(false));
        var value = await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "derived", c => new[] { c.MaximumLevel }).ConfigureAwait(false);

        Assert.That(value, Is.EqualTo(new[] { MaximumLevel }));
    }

    /// <summary>
    /// A data initialization which fails while it creates its data, e.g. because the server runs out of memory, may work
    /// on the next request, so its failure isn't remembered and the next request executes it again.
    /// </summary>
    [Test]
    public void FailingInitializationIsTriedAgain()
    {
        var plugInManager = CreatePlugInManager(new FailingInitialization(new InMemoryPersistenceContextProvider(), NullLoggerFactory.Instance));
        var key = FailingInitialization.TestKey;

        Assert.ThrowsAsync<InvalidOperationException>(async () => await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "first", c => new[] { c.MaximumLevel }).ConfigureAwait(false));
        Assert.ThrowsAsync<InvalidOperationException>(async () => await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "first", c => new[] { c.MaximumLevel }).ConfigureAwait(false));

        Assert.That(Executions[key], Is.EqualTo(2));
    }

    /// <summary>
    /// A data initialization which isn't available can't become available in the running process, so this failure is remembered:
    /// a later request fails at once, even with a plug in manager which knows it.
    /// </summary>
    [Test]
    public void UnavailableInitializationIsRemembered()
    {
        var key = LateInitialization.TestKey;
        var withoutInitialization = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        var withInitialization = CreatePlugInManager(new LateInitialization(new InMemoryPersistenceContextProvider(), NullLoggerFactory.Instance));

        var first = Assert.ThrowsAsync<ReferenceConfigurationFailedException>(async () => await ReferenceConfiguration.GetDerivedAsync(withoutInitialization, NullLoggerFactory.Instance, key, "first", c => new[] { c.MaximumLevel }).ConfigureAwait(false));
        Assert.ThrowsAsync<ReferenceConfigurationFailedException>(async () => await ReferenceConfiguration.GetDerivedAsync(withInitialization, NullLoggerFactory.Instance, key, "second", c => new[] { c.MaximumLevel }).ConfigureAwait(false));

        Assert.Multiple(() =>
        {
            Assert.That(first!.InitializationKey, Is.EqualTo(key));
            Assert.That(first.Message, Does.Contain("not available"));
            Assert.That(Executions.ContainsKey(key), Is.False);
        });
    }

    /// <summary>
    /// A data initialization which can't be executed in memory, as it has no constructor with the context provider and the logger factory,
    /// can't heal, so its failure is remembered: the inner exception is the original failure, not the exception which marks it.
    /// </summary>
    [Test]
    public void InitializationWithoutInMemoryConstructorIsRemembered()
    {
        var plugInManager = CreatePlugInManager(new WithoutInMemoryConstructorInitialization(new InMemoryPersistenceContextProvider()));
        var key = WithoutInMemoryConstructorInitialization.TestKey;

        var first = Assert.ThrowsAsync<ReferenceConfigurationFailedException>(async () => await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "first", c => new[] { c.MaximumLevel }).ConfigureAwait(false));
        var second = Assert.ThrowsAsync<ReferenceConfigurationFailedException>(async () => await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "second", c => new[] { c.MaximumLevel }).ConfigureAwait(false));

        Assert.Multiple(() =>
        {
            Assert.That(first!.Message, Does.Contain("can't be executed in memory"));
            Assert.That(first.InnerException, Is.Not.InstanceOf<ReferenceConfigurationFailedException>());
            Assert.That(first.InnerException!.InnerException, Is.InstanceOf<MissingMethodException>());
            Assert.That(second!.InnerException, Is.SameAs(first.InnerException));
            Assert.That(Executions.ContainsKey(key), Is.False);
        });
    }

    /// <summary>
    /// A data initialization which creates no game configuration can't heal, so its failure is remembered and it's executed only once.
    /// </summary>
    [Test]
    public void InitializationWithoutConfigurationIsRemembered()
    {
        var plugInManager = CreatePlugInManager(new WithoutConfigurationInitialization(new InMemoryPersistenceContextProvider(), NullLoggerFactory.Instance));
        var key = WithoutConfigurationInitialization.TestKey;

        var first = Assert.ThrowsAsync<ReferenceConfigurationFailedException>(async () => await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "first", c => new[] { c.MaximumLevel }).ConfigureAwait(false));
        Assert.ThrowsAsync<ReferenceConfigurationFailedException>(async () => await ReferenceConfiguration.GetDerivedAsync(plugInManager, NullLoggerFactory.Instance, key, "second", c => new[] { c.MaximumLevel }).ConfigureAwait(false));

        Assert.Multiple(() =>
        {
            Assert.That(first!.Message, Does.Contain("created no game configuration"));
            Assert.That(first.InnerException, Is.Not.InstanceOf<ReferenceConfigurationFailedException>());
            Assert.That(Executions[key], Is.EqualTo(1));
        });
    }

    /// <summary>
    /// A request which waits for another creation can be cancelled; the wait ends at once, and the creation lock
    /// works as before, so the running creation and later requests still succeed.
    /// </summary>
    [Test]
    public async Task WaitForAnotherCreationCanBeCancelledAsync()
    {
        var blockingManager = CreatePlugInManager(new BlockingInitialization(new InMemoryPersistenceContextProvider(), NullLoggerFactory.Instance));
        var waitingManager = CreatePlugInManager(new CancelledWaitInitialization(new InMemoryPersistenceContextProvider(), NullLoggerFactory.Instance));

        var blocked = ReferenceConfiguration.GetDerivedAsync(blockingManager, NullLoggerFactory.Instance, BlockingInitialization.TestKey, "blocked", c => new[] { c.MaximumLevel }).AsTask();
        try
        {
            await BlockingInitialization.Started.Task.WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

            using var cancellation = new CancellationTokenSource();
            var waiting = ReferenceConfiguration.GetDerivedAsync(waitingManager, NullLoggerFactory.Instance, CancelledWaitInitialization.TestKey, "waiting", c => new[] { c.MaximumLevel }, cancellation.Token).AsTask();
            Assert.That(waiting.IsCompleted, Is.False, "the request waits for the running creation");

            await cancellation.CancelAsync().ConfigureAwait(false);
            Assert.That(async () => await waiting.ConfigureAwait(false), Throws.InstanceOf<OperationCanceledException>());
        }
        finally
        {
            // The creation holds the lock which all tests share, so it's released also when an assertion fails.
            BlockingInitialization.Release.TrySetResult();
        }

        var blockedValue = await blocked.WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        var later = await ReferenceConfiguration.GetDerivedAsync(waitingManager, NullLoggerFactory.Instance, CancelledWaitInitialization.TestKey, "waiting", c => new[] { c.MaximumLevel }).AsTask().WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(blockedValue, Is.EqualTo(new[] { MaximumLevel }));
            Assert.That(later, Is.EqualTo(new[] { MaximumLevel }));
            Assert.That(Executions[CancelledWaitInitialization.TestKey], Is.EqualTo(1), "the cancelled request didn't execute the initialization");
        });
    }

    private static PlugInManager CreatePlugInManager(IDataInitializationPlugIn initialization)
    {
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        plugInManager.RegisterPlugInAtPlugInPoint(initialization);
        return plugInManager;
    }

    /// <summary>
    /// A data initialization which only creates a game configuration and counts its executions by its key.
    /// The reference configuration executes a new instance of the registered type, so each test has its own type.
    /// </summary>
    private abstract class TestInitializationBase : IDataInitializationPlugIn
    {
        private readonly IPersistenceContextProvider _contextProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestInitializationBase"/> class.
        /// </summary>
        /// <param name="contextProvider">The context provider.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        protected TestInitializationBase(IPersistenceContextProvider contextProvider, ILoggerFactory loggerFactory)
        {
            this._contextProvider = contextProvider;
            this._logger = loggerFactory.CreateLogger(this.GetType());
        }

        /// <inheritdoc />
        public abstract string Key { get; }

        /// <inheritdoc />
        public string Caption => "Test";

        /// <summary>
        /// Gets a value indicating whether the initialization fails.
        /// </summary>
        protected virtual bool Fails => false;

        /// <summary>
        /// Gets a value indicating whether the initialization creates a game configuration.
        /// </summary>
        protected virtual bool CreatesConfiguration => true;

        /// <summary>
        /// Waits before the initialization creates its data, e.g. to hold the creation lock.
        /// </summary>
        /// <returns>The task.</returns>
        protected virtual Task BeforeCreationAsync() => Task.CompletedTask;

        /// <inheritdoc />
        public async Task CreateInitialDataAsync(byte numberOfGameServers, bool createTestAccounts)
        {
            Executions.AddOrUpdate(this.Key, 1, (_, count) => count + 1);
            this._logger.LogDebug("Test initialization {Key} executed for {Servers} servers, test accounts: {TestAccounts}.", this.Key, numberOfGameServers, createTestAccounts);
            if (this.Fails)
            {
                throw new InvalidOperationException("The test initialization fails.");
            }

            await this.BeforeCreationAsync().ConfigureAwait(false);
            if (!this.CreatesConfiguration)
            {
                return;
            }

            using var context = this._contextProvider.CreateNewContext();
            var configuration = context.CreateNew<GameConfiguration>();
            configuration.MaximumLevel = MaximumLevel;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The data initialization of <see cref="ValuesAreDerivedOncePerNameFromOneInitializationAsync"/>.</summary>
    [Guid("2D7C1F6E-9B0A-4E6B-8F3C-5A1D2E4B7C90")]
    private sealed class DerivingTwiceInitialization(IPersistenceContextProvider contextProvider, ILoggerFactory loggerFactory) : TestInitializationBase(contextProvider, loggerFactory)
    {
        /// <summary>The key of the initialization.</summary>
        public const string TestKey = "reference-test-deriving-twice";

        /// <inheritdoc />
        public override string Key => TestKey;
    }

    /// <summary>The data initialization of <see cref="FailingDerivationIsRememberedAsync"/>.</summary>
    [Guid("6F1B2A47-3C8D-4E59-A0B6-7D2E9C4F1A83")]
    private sealed class FailingDerivationInitialization(IPersistenceContextProvider contextProvider, ILoggerFactory loggerFactory) : TestInitializationBase(contextProvider, loggerFactory)
    {
        /// <summary>The key of the initialization.</summary>
        public const string TestKey = "reference-test-failing-derivation";

        /// <inheritdoc />
        public override string Key => TestKey;
    }

    /// <summary>The data initialization of <see cref="FailingInitializationIsTriedAgain"/>, which fails.</summary>
    [Guid("A4C93E15-8B27-4D6F-9E01-5B3F7A2C8D46")]
    private sealed class FailingInitialization(IPersistenceContextProvider contextProvider, ILoggerFactory loggerFactory) : TestInitializationBase(contextProvider, loggerFactory)
    {
        /// <summary>The key of the initialization.</summary>
        public const string TestKey = "reference-test-failing-initialization";

        /// <inheritdoc />
        public override string Key => TestKey;

        /// <inheritdoc />
        protected override bool Fails => true;
    }

    /// <summary>The data initialization of <see cref="DerivationOutOfMemoryIsTriedAgainAsync"/>.</summary>
    [Guid("E1A47C2D-3B58-4F96-8D0A-6C9E2F1B7A54")]
    private sealed class OutOfMemoryDerivationInitialization(IPersistenceContextProvider contextProvider, ILoggerFactory loggerFactory) : TestInitializationBase(contextProvider, loggerFactory)
    {
        /// <summary>The key of the initialization.</summary>
        public const string TestKey = "reference-test-derivation-out-of-memory";

        /// <inheritdoc />
        public override string Key => TestKey;
    }

    /// <summary>The data initialization of <see cref="InitializationWithoutInMemoryConstructorIsRemembered"/>, which has no constructor with a logger factory.</summary>
    [Guid("47B0D9E3-2C6A-4E18-B5F7-8A3D1C6E9B02")]
    private sealed class WithoutInMemoryConstructorInitialization(IPersistenceContextProvider contextProvider) : TestInitializationBase(contextProvider, NullLoggerFactory.Instance)
    {
        /// <summary>The key of the initialization.</summary>
        public const string TestKey = "reference-test-without-in-memory-constructor";

        /// <inheritdoc />
        public override string Key => TestKey;
    }

    /// <summary>The data initialization of <see cref="InitializationWithoutConfigurationIsRemembered"/>, which creates no game configuration.</summary>
    [Guid("8D2C6F41-9E07-4B3A-A1D5-3F8B0E7C2A96")]
    private sealed class WithoutConfigurationInitialization(IPersistenceContextProvider contextProvider, ILoggerFactory loggerFactory) : TestInitializationBase(contextProvider, loggerFactory)
    {
        /// <summary>The key of the initialization.</summary>
        public const string TestKey = "reference-test-without-configuration";

        /// <inheritdoc />
        public override string Key => TestKey;

        /// <inheritdoc />
        protected override bool CreatesConfiguration => false;
    }

    /// <summary>The data initialization of <see cref="UnavailableInitializationIsRemembered"/>, which is registered after the first request.</summary>
    [Guid("C3D9E2A7-6B1F-4A85-9C04-7E2B5D8F1A63")]
    private sealed class LateInitialization(IPersistenceContextProvider contextProvider, ILoggerFactory loggerFactory) : TestInitializationBase(contextProvider, loggerFactory)
    {
        /// <summary>The key of the initialization.</summary>
        public const string TestKey = "reference-test-unavailable";

        /// <inheritdoc />
        public override string Key => TestKey;
    }

    /// <summary>The data initialization of <see cref="WaitForAnotherCreationCanBeCancelledAsync"/> which holds the creation lock until it's released.</summary>
    [Guid("5E8A0C3B-71D4-4F2A-B96E-0C4D8F1A2B37")]
    private sealed class BlockingInitialization(IPersistenceContextProvider contextProvider, ILoggerFactory loggerFactory) : TestInitializationBase(contextProvider, loggerFactory)
    {
        /// <summary>The key of the initialization.</summary>
        public const string TestKey = "reference-test-blocking";

        /// <summary>Gets the task which completes when the initialization runs.</summary>
        public static TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <summary>Gets the task which the initialization waits for.</summary>
        public static TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <inheritdoc />
        public override string Key => TestKey;

        /// <inheritdoc />
        protected override Task BeforeCreationAsync()
        {
            Started.TrySetResult();
            return Release.Task;
        }
    }

    /// <summary>The data initialization of <see cref="WaitForAnotherCreationCanBeCancelledAsync"/> whose first request is cancelled.</summary>
    [Guid("9B2F6D18-4A3E-4C70-8E5B-D1F07A6C3E29")]
    private sealed class CancelledWaitInitialization(IPersistenceContextProvider contextProvider, ILoggerFactory loggerFactory) : TestInitializationBase(contextProvider, loggerFactory)
    {
        /// <summary>The key of the initialization.</summary>
        public const string TestKey = "reference-test-cancelled-wait";

        /// <inheritdoc />
        public override string Key => TestKey;
    }
}
