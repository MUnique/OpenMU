// <copyright file="ConnectionConfigurator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The database roles.
    /// </summary>
    public enum DatabaseRole
    {
        /// <summary>
        /// The admin role which can create databases and other roles.
        /// </summary>
        Admin,

        /// <summary>
        /// The account role which can load and save account data.
        /// </summary>
        Account,

        /// <summary>
        /// The role which can load configuration data.
        /// </summary>
        Configuration,

        /// <summary>
        /// The role which can load guild data.
        /// </summary>
        Guild,

        /// <summary>
        /// The role which can load friend data.
        /// </summary>
        Friend,
    }

    /// <summary>
    /// The database connection configurator which loads the configuration from a file.
    /// </summary>
    public static class ConnectionConfigurator
    {
        private const string DbHostVariableName = "DB_HOST";
        private const string DbAdminUserVariableName = "DB_ADMIN_USER";
        private const string DbAdminPasswordVariableName = "DB_ADMIN_PW";

        private static readonly IDictionary<Type, ConnectionSetting> Settings = LoadSettings();

        /// <summary>
        /// Gets the name of the role from the configured connection string.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>The name of the role from the configured connection string.</returns>
        public static string GetRoleName(DatabaseRole role)
        {
            var settings = Settings[GetContextTypeOfRole(role)];
            return Regex.Match(settings.ConnectionString, "User Id=([^;]+?);").Groups[1].Value;
        }

        /// <summary>
        /// Gets the password password of the role from the configured connection string.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>The password password of the role from the configured connection string.</returns>
        public static string GetRolePassword(DatabaseRole role)
        {
            var settings = Settings[GetContextTypeOfRole(role)];
            return Regex.Match(settings.ConnectionString, "Password=([^;]+?);").Groups[1].Value;
        }

        /// <summary>
        /// Configures the specified options builder.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="optionsBuilder">The options builder.</param>
        /// <exception cref="NotImplementedException">At the moment only Npgsql engine (PostgreSQL) is implemented.</exception>
        internal static void Configure(this DbContext context, DbContextOptionsBuilder optionsBuilder)
        {
            var type = context.GetType();
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            if (Settings.TryGetValue(type, out ConnectionSetting setting))
            {
                switch (setting.DatabaseEngine)
                {
                    case DatabaseEngine.Npgsql:
                        optionsBuilder.UseNpgsql(setting.ConnectionString);
                        return;
                    default:
                        throw new NotImplementedException("At the moment only Npgsql engine (PostgreSQL) is implemented.");
                }
            }

            throw new ArgumentException($"No configuration found for context type {context.GetType()}", nameof(context));
        }

        private static Type GetContextTypeOfRole(DatabaseRole role)
        {
            switch (role)
            {
                case DatabaseRole.Account:
                    return typeof(AccountContext);
                case DatabaseRole.Admin:
                    return typeof(EntityDataContext);
                case DatabaseRole.Configuration:
                    return typeof(ConfigurationContext);
                case DatabaseRole.Guild:
                    return typeof(GuildContext);
                case DatabaseRole.Friend:
                    return typeof(FriendContext);
                default: throw new ArgumentException($"Role {role} unknown.");
            }
        }

        private static IDictionary<Type, ConnectionSetting> LoadSettings()
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

            var configurationFilePath = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ConnectionConfigurator).Assembly.CodeBase).LocalPath), "ConnectionSettings.xml");
            using (var xmlReader = XmlReader.Create(File.OpenRead(configurationFilePath), settings))
            {
                var serializer = new XmlSerializer(typeof(ConnectionSettings));
                if (serializer.CanDeserialize(xmlReader))
                {
                    if (serializer.Deserialize(xmlReader) is ConnectionSettings xmlSettings)
                    {
                        foreach (var setting in xmlSettings.Connections)
                        {
                            Type contextType = Type.GetType(setting.ContextTypeName, true, true);
                            ApplyEnvironmentVariables(setting);

                            result.Add(contextType, setting);
                        }
                    }
                }
            }

            return result;
        }

        private static void ApplyEnvironmentVariables(ConnectionSetting setting)
        {
            if (Environment.GetEnvironmentVariable(DbHostVariableName) is string dbHost
                                            && !string.IsNullOrEmpty(dbHost))
            {
                setting.ConnectionString = setting.ConnectionString.Replace("Server=localhost;", $"Server={dbHost};");
            }

            if (setting.ConnectionString.Contains("User Id=postgres;"))
            {
                if (Environment.GetEnvironmentVariable(DbAdminUserVariableName) is string dbAdminUser
                    && !string.IsNullOrEmpty(dbAdminUser))
                {
                    setting.ConnectionString = setting.ConnectionString.Replace("User Id=postgres;", $"User Id={dbAdminUser};");
                }

                if (Environment.GetEnvironmentVariable(DbAdminPasswordVariableName) is string dbAdminPassword
                    && !string.IsNullOrEmpty(dbAdminPassword))
                {
                    setting.ConnectionString = setting.ConnectionString.Replace("Password=admin;", $"Password={dbAdminPassword};");
                }
            }
        }
    }
}
