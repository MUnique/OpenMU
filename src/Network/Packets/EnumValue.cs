// <copyright file="EnumValue.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets;

using System.Xml.Serialization;

/// <summary>
/// A xml serializable definition of an enum value.
/// </summary>
[XmlType(Namespace = PacketDefinitions.XmlNamespace)]
[Serializable]
public class EnumValue
{
    /// <summary>
    /// Gets or sets the name of the enum value.
    /// </summary>
    [XmlElement(DataType = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the description of this enum value.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the numerical value of this enum value.
    /// </summary>
    public int Value { get; set; }
}