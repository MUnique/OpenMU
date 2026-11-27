// <copyright file="IExtendedItemSerializedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which is called after the <see cref="ItemSerializerExtended"/> serialized an item, so that it can
/// change or extend the data, e.g. to add information which a customized game client understands.
/// </summary>
[Guid("0C6A0B5D-7F0E-4E0F-9C53-2B9B1F3D5A21")]
[PlugInPoint("Extended item serialized", "Called after an item has been serialized with the extended format.")]
public interface IExtendedItemSerializedPlugIn
{
    /// <summary>
    /// Is called after an item has been serialized.
    /// </summary>
    /// <param name="data">The whole target buffer of the item. The item uses the first <see cref="ExtendedItemSerializedArgs.Length"/> bytes.</param>
    /// <param name="item">The serialized item.</param>
    /// <param name="args">The arguments, which hold the length of the serialized item.</param>
    void ItemSerialized(Span<byte> data, Item item, ExtendedItemSerializedArgs args);
}

/// <summary>
/// The arguments of <see cref="IExtendedItemSerializedPlugIn"/>.
/// </summary>
public class ExtendedItemSerializedArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedItemSerializedArgs"/> class.
    /// </summary>
    /// <param name="length">The length of the serialized item.</param>
    public ExtendedItemSerializedArgs(int length)
    {
        this.Length = length;
    }

    /// <summary>
    /// Gets or sets the length of the serialized item. A plugin which appends data has to increase it; it must not
    /// exceed <see cref="IItemSerializer.NeededSpace"/>, and the game client has to be able to derive it from the data.
    /// </summary>
    public int Length { get; set; }
}
