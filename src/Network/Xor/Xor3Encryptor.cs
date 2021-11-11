﻿// <copyright file="Xor3Encryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Xor;

/// <summary>
/// An encryptor which XOR-encrypts data using a 3-byte key.
/// </summary>
public class Xor3Encryptor : ISpanEncryptor
{
    private readonly byte[] _xor3Keys;

    private readonly int _startOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="Xor3Encryptor"/> class.
    /// </summary>
    /// <param name="startOffset">The start offset.</param>
    public Xor3Encryptor(int startOffset)
    {
        this._startOffset = startOffset;
        this._xor3Keys = DefaultKeys.Xor3Keys;
    }

    /// <inheritdoc/>
    public void Encrypt(Span<byte> data)
    {
        this.InternalEncrypt(data.Slice(this._startOffset));
    }

    /// <summary>
    /// Internal encrypt function. XORs each byte with one byte of the 3-byte key.
    /// </summary>
    /// <param name="data">The data.</param>
    protected void InternalEncrypt(Span<byte> data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            data[i] ^= this._xor3Keys[i % 3];
        }
    }
}