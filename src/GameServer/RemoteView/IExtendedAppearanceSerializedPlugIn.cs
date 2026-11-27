// <copyright file="IExtendedAppearanceSerializedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which is called after the <see cref="AppearanceSerializerExtended"/> serialized the appearance of a
/// character, so that it can change the data, e.g. for a customized game client.
/// </summary>
[Guid("6E1F3C84-2B7A-4C59-8D0E-71A4F9C2B6D3")]
[PlugInPoint("Extended appearance serialized", "Called after the appearance of a character has been serialized with the extended format.")]
public interface IExtendedAppearanceSerializedPlugIn
{
    /// <summary>
    /// Is called after an appearance has been serialized.
    /// </summary>
    /// <param name="data">The serialized appearance, with a size of <see cref="IAppearanceSerializer.NeededSpace"/>.</param>
    /// <param name="appearance">The appearance data.</param>
    /// <remarks>
    /// The result may be cached and shared between all viewers, until <see cref="IAppearanceData.AppearanceChanged"/>
    /// is raised. So it must not depend on the viewer.
    /// </remarks>
    void AppearanceSerialized(Span<byte> data, IAppearanceData appearance);
}
