// <copyright file="ConnectionSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Runtime.Serialization;
using System.Xml.Serialization;

/// <summary>
/// The database connection settings xml serialization class.
/// </summary>
[Serializable]
[XmlType(AnonymousType = true, Namespace = "http://www.munique.net/ConnectionSettings")]
[XmlRoot(Namespace = "http://www.munique.net/ConnectionSettings", IsNullable = false)]
public class ConnectionSettings : IDeserializationCallback
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionSettings"/> class.
    /// </summary>
    public ConnectionSettings()
    {
        this.Connections = new ConnectionSetting[0];
    }

    /// <summary>
    /// Gets or sets the database connections.
    /// </summary>
    [XmlArray(IsNullable = false)]
    [XmlArrayItem("Connection", IsNullable = false)]
    public ConnectionSetting[] Connections { get; set; }

    /// <summary>
    /// Runs when the entire object graph has been deserialized.
    /// </summary>
    /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
    public void OnDeserialization(object? sender)
    {
        this.Connections ??= new ConnectionSetting[0];
    }
}