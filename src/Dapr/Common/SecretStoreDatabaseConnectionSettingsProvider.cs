// <copyright file="SecretStoreDatabaseConnectionSettingsProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Threading;
using global::Dapr;
using global::Dapr.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Implementation of <see cref="IDatabaseConnectionSettingProvider"/> which retrieves the settings from the
/// configured Dapr secret storage.
/// </summary>
public class SecretStoreDatabaseConnectionSettingsProvider : IDatabaseConnectionSettingProvider
{
    private const string SecretStoreName = "secrets";
    private readonly DaprClient _daprClient;
    private readonly ILogger<SecretStoreDatabaseConnectionSettingsProvider> _logger;
    private readonly Dictionary<string, ConnectionSetting> _connectionSettings = new(StringComparer.InvariantCultureIgnoreCase);
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretStoreDatabaseConnectionSettingsProvider" /> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public SecretStoreDatabaseConnectionSettingsProvider(DaprClient daprClient, ILogger<SecretStoreDatabaseConnectionSettingsProvider> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;
    }

    /// <inheritdoc />
    public Task? Initialization { get; private set; }

    /// <inheritdoc />
    public Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (this._isInitialized)
        {
            return Task.CompletedTask;
        }

        this.Initialization = Task.Run(
            async () =>
            {
                this._isInitialized = false;
                while (!this._isInitialized && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        Console.WriteLine("trying to get secrets ...");
                        var secrets = await this._daprClient.GetBulkSecretAsync(SecretStoreName, cancellationToken: cancellationToken).ConfigureAwait(false);
                        foreach (var secret in secrets.Where(kvp => string.Equals(kvp.Key.Split(':')[0], "connectionStrings", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var contextTypeName = secret.Value.Keys.First().Split(':').Last();
                            var setting = new ConnectionSetting
                            {
                                ContextTypeName = contextTypeName,
                                ConnectionString = secret.Value.Values.First()!,
                                DatabaseEngine = DatabaseEngine.Npgsql,
                            };

                            this._connectionSettings.Add(contextTypeName, setting);
                        }

                        Console.WriteLine("secrects retrieved :)");

                        this._isInitialized = true;
                    }
                    catch (DaprException ex)
                    {
                        // This should never happen - however, it may happen when we are using a Dapr secret store.
                        // It may not be started yet, and the implementation to get it does retrieve it in the constructor already.
                        this._logger.LogWarning(ex, "Error occurred when retrieving the connection strings from the secrets store. Trying again in 3 seconds...");
                        Console.WriteLine("Error occurred when retrieving the connection strings from the secrets store. Trying again in 3 seconds...");
                        await Task.Delay(3000, cancellationToken).ConfigureAwait(false);
                    }
                }
            });
        ConnectionConfigurator.Initialize(this);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public ConnectionSetting GetConnectionSetting<TContextType>()
        where TContextType : DbContext
    {
        return this.GetConnectionSetting(typeof(TContextType));
    }

    /// <inheritdoc />
    public ConnectionSetting GetConnectionSetting(Type contextType)
    {
        if (this._connectionSettings.TryGetValue(contextType.FullName ?? contextType.Name, out var result))
        {
            return result;
        }

        throw new InvalidOperationException($"No connection string for type '{contextType.FullName}' stored in the secret store.");
    }
}