// <copyright file="IExtendedAppearanceChangingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which is called before the <see cref="AppearanceChangedExtendedPlugIn"/> sends a changed item of a
/// player's appearance, so that it can change the sent values.
/// </summary>
[Guid("B4D8A2F7-93C1-4E6B-A5F0-8C27D1E94A35")]
[PlugInPoint("Extended appearance changing", "Called before a changed item of the appearance is sent with the extended format.")]
public interface IExtendedAppearanceChangingPlugIn
{
    /// <summary>
    /// Is called before the changed appearance is sent.
    /// </summary>
    /// <param name="changedPlayer">The player whose appearance changed.</param>
    /// <param name="changedItem">The changed item.</param>
    /// <param name="isShown">If set to <c>true</c>, the item is shown in the appearance; Otherwise, it's removed from it.</param>
    /// <param name="change">The values which are going to be sent.</param>
    void AppearanceChanging(Player changedPlayer, Item changedItem, bool isShown, ExtendedAppearanceChange change);
}

/// <summary>
/// The values of a changed item, as they're sent by the <see cref="AppearanceChangedExtendedPlugIn"/>.
/// </summary>
public class ExtendedAppearanceChange
{
    /// <summary>Gets or sets the item group; 0xFF if the item isn't shown.</summary>
    public byte ItemGroup { get; set; }

    /// <summary>Gets or sets the item number.</summary>
    public ushort ItemNumber { get; set; }

    /// <summary>Gets or sets the item level.</summary>
    public byte ItemLevel { get; set; }

    /// <summary>Gets or sets the excellent flags.</summary>
    public byte ExcellentFlags { get; set; }

    /// <summary>Gets or sets the discriminator of the ancient set.</summary>
    public byte AncientDiscriminator { get; set; }
}
