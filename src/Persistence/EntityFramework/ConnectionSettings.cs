// <copyright file="ConnectionSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The database connection settings xml serialization class.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.munique.net/ConnectionSettings")]
    [XmlRoot(Namespace = "http://www.munique.net/ConnectionSettings", IsNullable = false)]
    public class ConnectionSettings
    {
        /// <summary>
        /// Gets or sets the database connections.
        /// </summary>
        [XmlArray(IsNullable = false)]
        [XmlArrayItem("Connection", IsNullable = false)]
        public ConnectionSetting[] Connections { get; set; }
    }
}
