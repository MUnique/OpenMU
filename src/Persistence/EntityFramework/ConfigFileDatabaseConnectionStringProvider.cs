// <copyright file="ConfigFileDatabaseConnectionStringProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementation of <see cref="IDatabaseConnectionSettingProvider"/> which takes the connection strings out of
/// a configuration file, usually <c>ConnectionSettings.xml</c>.
/// The settings can be influenced by the environment variables <c>DB_HOST</c>, <c>DB_ADMIN_USER</c> and <c>DB_ADMIN_PW</c>.
/// </summary>
public class ConfigFileDatabaseConnectionStringProvider : IDatabaseConnectionSettingProvider
{
    private const string DbHostVariableName = "DB_HOST";
    private const string DbAdminUserVariableName = "DB_ADMIN_USER";
    private const string DbAdminPasswordVariableName = "DB_ADMIN_PW";

    private readonly string _fileName;

    private IDictionary<Type, ConnectionSetting>? _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigFileDatabaseConnectionStringProvider"/> class.
    /// </summary>
    public ConfigFileDatabaseConnectionStringProvider()
        : this("ConnectionSettings.xml")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigFileDatabaseConnectionStringProvider"/> class.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    public ConfigFileDatabaseConnectionStringProvider(string fileName)
    {
        this._fileName = fileName;
    }

    /// <inheritdoc />
    public Task? Initialization { get; private set; }

    private IDictionary<Type, ConnectionSetting> Settings => this._settings ??= this.LoadSettings();

    /// <inheritdoc />
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        this.Initialization = Task.Run(() => this._settings = this.LoadSettings(), cancellationToken);
        await this.Initialization.ConfigureAwait(false);
        ConnectionConfigurator.Initialize(this);
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
        if (this.Settings.TryGetValue(contextType, out var result))
        {
            return result;
        }

        throw new ArgumentException("DB Configuration not found for type {0}", contextType.FullName);
    }

    private IDictionary<Type, ConnectionSetting> LoadSettings()
    {
        var settings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
            DtdProcessing = DtdProcessing.Ignore,
            CloseInput = true,
            XmlResolver = null,
        };
        var result = new Dictionary<Type, ConnectionSetting>();

        var configurationFilePath = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ConnectionConfigurator).Assembly.Location!).LocalPath)!, this._fileName);
        using var xmlReader = XmlReader.Create(File.OpenRead(configurationFilePath), settings);
        var serializer = new XmlSerializer(typeof(ConnectionSettings));
        if (serializer.CanDeserialize(xmlReader))
        {
            if (serializer.Deserialize(xmlReader) is ConnectionSettings xmlSettings)
            {
                foreach (var setting in xmlSettings.Connections)
                {
                    if (setting.ContextTypeName is null)
                    {
                        throw new InvalidDataException("ContextTypeName is null.");
                    }

                    if (setting.ConnectionString is null)
                    {
                        throw new InvalidDataException("ConnectionString is null.");
                    }

                    if (Type.GetType(setting.ContextTypeName, false, true) is { } contextType)
                    {
                        this.ApplyEnvironmentVariables(setting);
                        result.Add(contextType, setting);
                    }
                    else if (setting.ContextTypeName.EndsWith($".{nameof(TypedContext)}") || setting.ContextTypeName.EndsWith($".{nameof(TypedContext)}^1"))
                    {
                        this.ApplyEnvironmentVariables(setting);
                        result.Add(typeof(TypedContext), setting);
                    }
                    else
                    {
                        throw new InvalidDataException($"Unknown context type: {setting.ContextTypeName}");
                    }
                }
            }
        }

        return result;
    }

    private void ApplyEnvironmentVariables(ConnectionSetting setting)
    {
        if (Environment.GetEnvironmentVariable(DbHostVariableName) is { } dbHost
            && !string.IsNullOrEmpty(dbHost))
        {
            setting.ConnectionString = setting.ConnectionString!.Replace("Server=localhost;", $"Server={dbHost};");
        }

        if (setting.ConnectionString!.Contains("User Id=postgres;"))
        {
            if (Environment.GetEnvironmentVariable(DbAdminUserVariableName) is { } dbAdminUser
                && !string.IsNullOrEmpty(dbAdminUser))
            {
                setting.ConnectionString = setting.ConnectionString.Replace("User Id=postgres;", $"User Id={dbAdminUser};");
            }

            if (Environment.GetEnvironmentVariable(DbAdminPasswordVariableName) is { } dbAdminPassword
                && !string.IsNullOrEmpty(dbAdminPassword))
            {
                setting.ConnectionString = setting.ConnectionString.Replace("Password=admin;", $"Password={dbAdminPassword};");
            }
        }
    }
}