// <copyright file="EffectTimeDefaultsSourceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Tests the <see cref="EffectTimeDefaultsSource"/>, which provides the defaults of the installed data to the operations.
/// </summary>
[TestFixture]
public sealed class EffectTimeDefaultsSourceTests
{
    /// <summary>
    /// Defaults which can't be determined, e.g. for data of an unknown data initialization, come as <c>null</c>,
    /// so that the effects are listed without them.
    /// </summary>
    [Test]
    public async Task DefaultsWhichCantBeDeterminedAreNullAsync()
    {
        var source = new EffectTimeDefaultsSource(new FailingDefaults(new InvalidOperationException("Unknown client version.")), NullLogger<EffectTimeDefaultsSource>.Instance);

        var defaults = await source.GetAsync(default).ConfigureAwait(false);

        Assert.That(defaults, Is.Null);
    }

    /// <summary>
    /// A failure which the reference configuration remembered was logged as a warning when it happened, so the source,
    /// which gets it again on every request, only logs it as debug information.
    /// </summary>
    [Test]
    public async Task RememberedFailureIsNotLoggedAsWarningAgainAsync()
    {
        var logger = new Mock<ILogger<EffectTimeDefaultsSource>>();
        var failure = new ReferenceConfigurationFailedException("unknown", new InvalidOperationException("The data initialization 'unknown' is not available."));
        var source = new EffectTimeDefaultsSource(new FailingDefaults(failure), logger.Object);

        var defaults = await source.GetAsync(default).ConfigureAwait(false);

        Assert.That(defaults, Is.Null);
        logger.Verify(l => l.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
        logger.Verify(l => l.Log(LogLevel.Debug, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), failure, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    /// <summary>
    /// Defaults which fail with the given exception, e.g. like the data of an unknown client version.
    /// </summary>
    /// <param name="failure">The exception which the defaults throw.</param>
    private sealed class FailingDefaults(Exception failure) : EffectTimeDefaults(new PlugInManager(null, NullLoggerFactory.Instance, null, null), NullLoggerFactory.Instance, new InMemoryPersistenceContextProvider())
    {
        /// <inheritdoc />
        public override ValueTask<(string InitializationKey, string Caption, IReadOnlyDictionary<short, bool> SendsTime)> GetForInstalledDataAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            throw failure;
        }
    }
}
