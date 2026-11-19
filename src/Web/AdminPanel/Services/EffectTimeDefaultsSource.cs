// <copyright file="EffectTimeDefaultsSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>
/// Provides the <see cref="EffectTimeOperations"/> with the effect time defaults of the installed data.
/// </summary>
/// <param name="defaults">The defaults of the data initializations.</param>
/// <param name="logger">The logger.</param>
public sealed class EffectTimeDefaultsSource(EffectTimeDefaults defaults, ILogger<EffectTimeDefaultsSource> logger) : IEffectTimeDefaultsSource
{
    /// <inheritdoc />
    /// <remarks>
    /// The defaults are only a comparison, so the effects are listed without them when they can't be determined,
    /// e.g. for data of an unknown data initialization, or for one which can't be executed in memory.
    /// A failure of the reference configuration (<see cref="ReferenceConfigurationFailedException"/>) is remembered and logged once
    /// by the <see cref="ReferenceConfiguration"/>, so it's only logged as debug information here; it comes again on every request.
    /// Other failures, e.g. of reading the installed data or of an unknown client version, are logged as warning on every request,
    /// as they may be caused by the database.
    /// </remarks>
    public async ValueTask<EffectTimeDefaultValues?> GetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var (key, caption, sendsTime) = await defaults.GetForInstalledDataAsync(cancellationToken).ConfigureAwait(false);
            return new EffectTimeDefaultValues(key, caption, sendsTime);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.Log(ex is ReferenceConfigurationFailedException ? LogLevel.Debug : LogLevel.Warning, ex, "The effect time defaults of the installed data can't be determined: {Message}", ex.Message);
            return null;
        }
    }
}
