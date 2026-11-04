// <copyright file="MoneyDropped.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Extends the <see cref="MoneyDropped"/>.
/// </summary>
public readonly partial struct MoneyDropped
{
    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    public uint Amount
    {
        get
        {
            var data = this._data.Span;
            return (uint)(data[10] << 16 | data[11] << 8 | data[13]);
        }
        set
        {
            var data = this._data.Span;
            data[10] = (byte)(value >> 16 & 0xFF);
            data[11] = (byte)(value >> 8 & 0xFF);
            data[13] = (byte)(value & 0xFF);
        }
    }
}

/// <summary>
/// Extends the <see cref="MoneyDropped075"/>.
/// </summary>
public readonly partial struct MoneyDropped075
{
    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    public uint Amount
    {
        get
        {
            var data = this._data.Span;
            return (uint)(data[10] << 16 | data[11] << 8 | data[12]);
        }
        set
        {
            var data = this._data.Span;
            data[10] = (byte)(value >> 16 & 0xFF);
            data[11] = (byte)(value >> 8 & 0xFF);
            data[12] = (byte)(value & 0xFF);
        }
    }
}

/// <summary>
/// Extends the <see cref="MoneyDroppedRef"/>.
/// </summary>
public readonly ref partial struct MoneyDroppedRef
{
    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    public uint Amount
    {
        get => (uint)(this._data[10] << 16 | this._data[11] << 8 | this._data[13]);
        set
        {
            this._data[10] = (byte)(value >> 16 & 0xFF);
            this._data[11] = (byte)(value >> 8 & 0xFF);
            this._data[13] = (byte)(value & 0xFF);
        }
    }
}

/// <summary>
/// Extends the <see cref="MoneyDropped075Ref"/>.
/// </summary>
public readonly ref partial struct MoneyDropped075Ref
{
    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    public uint Amount
    {
        get => (uint)(this._data[10] << 16 | this._data[11] << 8 | this._data[12]);
        set
        {
            this._data[10] = (byte)(value >> 16 & 0xFF);
            this._data[11] = (byte)(value >> 8 & 0xFF);
            this._data[12] = (byte)(value & 0xFF);
        }
    }
}