// <copyright file="ConnectionSetting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The configured database engine.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://www.munique.net/ConnectionSettings")]
    public enum DatabaseEngine
    {
        /// <summary>
        /// The NPGSQL engine (PostgreSQL).
        /// </summary>
        Npgsql,

        /// <summary>
        /// The MSSQL server engine.
        /// </summary>
        SqlServer,

        /// <summary>
        /// The in memory engine (could be used for testing).
        /// </summary>
        InMemory,
    }

    /// <summary>
    /// A database connection setting for the specified role.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://www.munique.net/ConnectionSettings")]
    public class ConnectionSetting
    {
        /// <summary>
        /// Gets or sets the name of the type of the <see cref="DbContext"/>.
        /// </summary>
        public string? ContextTypeName { get; set; }

        /// <summary>
        /// Gets or sets the connection string which should be used for the DbContext.
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the database engine.
        /// </summary>
        [DefaultValue(DatabaseEngine.Npgsql)]
        public DatabaseEngine DatabaseEngine { get; set; }
    }
}