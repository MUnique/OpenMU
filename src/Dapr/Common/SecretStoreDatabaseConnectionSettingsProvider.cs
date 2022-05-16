// <copyright file="SecretStoreDatabaseConnectionSettingsProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using global::Dapr.Client;
using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Implementation of <see cref="IDatabaseConnectionSettingProvider"/> which retrieves the settings from the
/// configured Dapr secret storage.
/// </summary>
public class SecretStoreDatabaseConnectionSettingsProvider : IDatabaseConnectionSettingProvider
{
    private const string SecretStoreName = "secrets";
    private readonly DaprClient _daprClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretStoreDatabaseConnectionSettingsProvider"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    public SecretStoreDatabaseConnectionSettingsProvider(DaprClient daprClient)
    {
        this._daprClient = daprClient;
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
        // not optimal ... how about loading them all at once
        var secret = this._daprClient.GetSecretAsync(SecretStoreName, $"connectionStrings:{contextType.FullName}").GetAwaiter().GetResult();
        var result = new ConnectionSetting
        {
            ContextTypeName = contextType.FullName,
            ConnectionString = secret.Values.First(),
            DatabaseEngine = DatabaseEngine.Npgsql,
        };

        return result;
    }
}