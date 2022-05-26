// <copyright file="SecretStoreDatabaseConnectionSettingsProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using global::Dapr.Client;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx.Synchronous;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Implementation of <see cref="IDatabaseConnectionSettingProvider"/> which retrieves the settings from the
/// configured Dapr secret storage.
/// </summary>
public class SecretStoreDatabaseConnectionSettingsProvider : IDatabaseConnectionSettingProvider
{
    private const string SecretStoreName = "secrets";
    private readonly DaprClient _daprClient;
    private readonly Dictionary<string, ConnectionSetting> _connectionSettings = new(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretStoreDatabaseConnectionSettingsProvider"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    public SecretStoreDatabaseConnectionSettingsProvider(DaprClient daprClient)
    {
        this._daprClient = daprClient;

        var secrets = this._daprClient.GetBulkSecretAsync(SecretStoreName).WaitAndUnwrapException();
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