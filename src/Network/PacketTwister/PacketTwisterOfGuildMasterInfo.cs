// <copyright file="PacketTwisterOfGuildMasterInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister;

/// <summary>
/// PacketTwister implementation for packets of 'GuildMasterInfo' type.
/// </summary>
internal class PacketTwisterOfGuildMasterInfo : IPacketTwister
{
    /// <inheritdoc/>
    public void Twist(Span<byte> data)
    {
        if (data.Length >= 4)
        {
            if (data.Length >= 8)
            {
                if (data.Length >= 16)
                {
                    if (data.Length >= 32)
                    {
                        var v13 = (byte)(data[15] >> 2);
                        data[15] <<= 6;
                        data[15] |= v13;
                        var v16 = (byte)((data[9] >> 2) & 1);
                        if (((data[9] >> 1) & 1) != 0)
                        {
                            data[9] |= 4;
                        }
                        else
                        {
                            data[9] &= 0xFB;
                        }

                        if (v16 != 0)
                        {
                            data[9] |= 2;
                        }
                        else
                        {
                            data[9] &= 0xFD;
                        }

                        var v14 = (byte)(data[5] >> 4);
                        data[5] *= 16;
                        data[5] |= v14;
                        var v15 = (byte)(data[26] >> 3);
                        data[26] *= 32;
                        data[26] |= v15;
                    }
                    else
                    {
                        var v10 = data[9];
                        data[9] = data[0];
                        data[0] = v10;
                        var v17 = (byte)((data[14] >> 3) & 1);
                        if (((data[14] >> 6) & 1) != 0)
                        {
                            data[14] |= 8;
                        }
                        else
                        {
                            data[14] &= 0xF7;
                        }

                        if (v17 != 0)
                        {
                            data[14] |= 0x40;
                        }
                        else
                        {
                            data[14] &= 0xBF;
                        }

                        var v11 = data[11];
                        data[11] = data[14];
                        data[14] = v11;
                        var v12 = data[13];
                        data[13] = data[11];
                        data[11] = v12;
                        data[6] ^= 0xDC;
                        data[12] ^= 0x59;
                    }
                }
                else
                {
                    var v20 = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (v20 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    data[3] ^= 0xFB;
                    var v19 = (byte)((data[7] >> 2) & 1);
                    if (((data[7] >> 3) & 1) != 0)
                    {
                        data[7] |= 4;
                    }
                    else
                    {
                        data[7] &= 0xFB;
                    }

                    if (v19 != 0)
                    {
                        data[7] |= 8;
                    }
                    else
                    {
                        data[7] &= 0xF7;
                    }

                    data[7] ^= 0xAB;
                    var v8 = data[1];
                    data[1] = data[6];
                    data[6] = v8;
                    var v18 = (byte)((data[2] >> 4) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    if (v18 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v9 = data[4];
                    data[4] = data[3];
                    data[3] = v9;
                }
            }
            else
            {
                var v3 = data[1];
                data[1] = data[1];
                data[1] = v3;
                var v4 = data[3];
                data[3] = data[0];
                data[0] = v4;
                var v5 = (byte)(data[1] >> 6);
                data[1] *= 4;
                data[1] |= v5;
                data[0] ^= 0xD6;
                var v2 = (byte)((data[1] >> 1) & 1);
                if (((data[1] >> 1) & 1) != 0)
                {
                    data[1] |= 2;
                }
                else
                {
                    data[1] &= 0xFD;
                }

                if (v2 != 0)
                {
                    data[1] |= 2;
                }
                else
                {
                    data[1] &= 0xFD;
                }

                var v6 = (byte)(data[0] >> 3);
                data[0] *= 32;
                data[0] |= v6;
                var v7 = data[2];
                data[2] = data[0];
                data[0] = v7;
                data[2] ^= 0x63;
                data[1] ^= 0x9A;
                var v21 = (byte)((data[0] >> 2) & 1);
                if (((data[0] >> 4) & 1) != 0)
                {
                    data[0] |= 4;
                }
                else
                {
                    data[0] &= 0xFB;
                }

                if (v21 != 0)
                {
                    data[0] |= 0x10;
                }
                else
                {
                    data[0] &= 0xEF;
                }

                data[2] = data[2];
            }
        }
    }

    /// <inheritdoc/>
    public void Correct(Span<byte> data)
    {
        if (data.Length >= 4)
        {
            if (data.Length >= 8)
            {
                if (data.Length >= 16)
                {
                    if (data.Length >= 32)
                    {
                        var v13 = (byte)(data[26] >> 5);
                        data[26] *= 8;
                        data[26] |= v13;
                        var v14 = (byte)(data[5] >> 4);
                        data[5] *= 16;
                        data[5] |= v14;
                        var v16 = (byte)((data[9] >> 2) & 1);
                        if (((data[9] >> 1) & 1) != 0)
                        {
                            data[9] |= 4;
                        }
                        else
                        {
                            data[9] &= 0xFB;
                        }

                        if (v16 != 0)
                        {
                            data[9] |= 2;
                        }
                        else
                        {
                            data[9] &= 0xFD;
                        }

                        var v15 = (byte)(data[15] >> 6);
                        data[15] *= 4;
                        data[15] |= v15;
                    }
                    else
                    {
                        data[12] ^= 0x59;
                        data[6] ^= 0xDC;
                        var v10 = data[13];
                        data[13] = data[11];
                        data[11] = v10;
                        var v11 = data[11];
                        data[11] = data[14];
                        data[14] = v11;
                        var v17 = (byte)((data[14] >> 3) & 1);
                        if (((data[14] >> 6) & 1) != 0)
                        {
                            data[14] |= 8;
                        }
                        else
                        {
                            data[14] &= 0xF7;
                        }

                        if (v17 != 0)
                        {
                            data[14] |= 0x40;
                        }
                        else
                        {
                            data[14] &= 0xBF;
                        }

                        var v12 = data[9];
                        data[9] = data[0];
                        data[0] = v12;
                    }
                }
                else
                {
                    var v8 = data[4];
                    data[4] = data[3];
                    data[3] = v8;
                    var v20 = (byte)((data[2] >> 4) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    if (v20 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v9 = data[1];
                    data[1] = data[6];
                    data[6] = v9;
                    data[7] ^= 0xAB;
                    var v19 = (byte)((data[7] >> 2) & 1);
                    if (((data[7] >> 3) & 1) != 0)
                    {
                        data[7] |= 4;
                    }
                    else
                    {
                        data[7] &= 0xFB;
                    }

                    if (v19 != 0)
                    {
                        data[7] |= 8;
                    }
                    else
                    {
                        data[7] &= 0xF7;
                    }

                    data[3] ^= 0xFB;
                    var v18 = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (v18 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }
                }
            }
            else
            {
                data[2] = data[2];
                var v2 = (byte)((data[0] >> 2) & 1);
                if (((data[0] >> 4) & 1) != 0)
                {
                    data[0] |= 4;
                }
                else
                {
                    data[0] &= 0xFB;
                }

                if (v2 != 0)
                {
                    data[0] |= 0x10;
                }
                else
                {
                    data[0] &= 0xEF;
                }

                data[1] ^= 0x9A;
                data[2] ^= 0x63;
                var v3 = data[2];
                data[2] = data[0];
                data[0] = v3;
                var v4 = (byte)(data[0] >> 5);
                data[0] *= 8;
                data[0] |= v4;
                var v21 = (byte)((data[1] >> 1) & 1);
                if (((data[1] >> 1) & 1) != 0)
                {
                    data[1] |= 2;
                }
                else
                {
                    data[1] &= 0xFD;
                }

                if (v21 != 0)
                {
                    data[1] |= 2;
                }
                else
                {
                    data[1] &= 0xFD;
                }

                data[0] ^= 0xD6;
                var v5 = (byte)(data[1] >> 2);
                data[1] <<= 6;
                data[1] |= v5;
                var v6 = data[3];
                data[3] = data[0];
                data[0] = v6;
                var v7 = data[1];
                data[1] = data[1];
                data[1] = v7;
            }
        }
    }
}