// <copyright file="AttributeValueExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

/// <summary>
/// Extensions to convert attribute values for packets.
/// </summary>
public static class AttributeValueExtensions
{
    /// <summary>
    /// Converts the attribute value to an <see cref="ushort"/>, clamped to its range.
    /// </summary>
    /// <param name="value">The attribute value.</param>
    /// <returns>The clamped value.</returns>
    /// <remarks>
    /// A plain cast wraps around for values above <see cref="ushort.MaxValue"/>, so e.g. a maximum health
    /// of 290505 would be sent as 28361. Older clients can't show more than <see cref="ushort.MaxValue"/> anyway.
    /// </remarks>
    public static ushort ToUInt16Clamped(this float value)
    {
        return (ushort)Math.Clamp(value, ushort.MinValue, ushort.MaxValue);
    }
}
