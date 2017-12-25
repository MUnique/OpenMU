// <copyright file="PacketTwisterOfGuildJoinRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'GuildJoinRequest' type.
    /// </summary>
    internal class PacketTwisterOfGuildJoinRequest : IPacketTwister
    {
        /// <inheritdoc/>
        public void Twist(IList<byte> data)
        {
            if (data.Count >= 4)
            {
                if (data.Count >= 8)
                {
                    if (data.Count >= 16)
                    {
                        if (data.Count >= 32)
                        {
                            var v14 = data[26];
                            data[26] = data[22];
                            data[22] = v14;
                            var v15 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 4) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v15 != 0)
                            {
                                data[15] |= 0x10;
                            }
                            else
                            {
                                data[15] &= 0xEF;
                            }

                            data[11] ^= 0x6A;
                        }
                        else
                        {
                            var v13 = data[10];
                            data[10] = data[12];
                            data[12] = v13;
                            data[9] ^= 0x46;
                            data[10] ^= 0xC3;
                        }
                    }
                    else
                    {
                        var v6 = data[3];
                        data[3] = data[5];
                        data[5] = v6;
                        data[0] ^= 0x2B;
                        var v18 = (byte)((data[6] >> 1) & 1);
                        if (((data[6] >> 6) & 1) != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        if (v18 != 0)
                        {
                            data[6] |= 0x40;
                        }
                        else
                        {
                            data[6] &= 0xBF;
                        }

                        var v17 = (byte)((data[0] >> 1) & 1);
                        if (((data[0] >> 2) & 1) != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        if (v17 != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        var v7 = (byte)(data[6] >> 6);
                        data[6] *= 4;
                        data[6] |= v7;
                        var v8 = data[5];
                        data[5] = data[0];
                        data[0] = v8;
                        var v9 = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= v9;
                        var v10 = data[3];
                        data[3] = data[4];
                        data[4] = v10;
                        var v11 = (byte)(data[7] >> 1);
                        data[7] <<= 7;
                        data[7] |= v11;
                        var v16 = (byte)((data[7] >> 4) & 1);
                        if (((data[7] >> 4) & 1) != 0)
                        {
                            data[7] |= 0x10;
                        }
                        else
                        {
                            data[7] &= 0xEF;
                        }

                        if (v16 != 0)
                        {
                            data[7] |= 0x10;
                        }
                        else
                        {
                            data[7] &= 0xEF;
                        }

                        var v12 = (byte)(data[2] >> 2);
                        data[2] <<= 6;
                        data[2] |= v12;
                    }
                }
                else
                {
                    data[3] ^= 0x15;
                    data[0] ^= 0xDB;
                    var v2 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v19 = (byte)((data[0] >> 1) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    if (v19 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v3 = data[3];
                    data[3] = data[1];
                    data[1] = v3;
                    data[0] ^= 0x6A;
                    var v4 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v4;
                    data[2] ^= 0xC1;
                    var v5 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v5;
                }
            }
        }

        /// <inheritdoc/>
        public void Correct(IList<byte> data)
        {
            if (data.Count >= 4)
            {
                if (data.Count >= 8)
                {
                    if (data.Count >= 16)
                    {
                        if (data.Count >= 32)
                        {
                            data[11] ^= 0x6A;
                            var v15 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 4) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v15 != 0)
                            {
                                data[15] |= 0x10;
                            }
                            else
                            {
                                data[15] &= 0xEF;
                            }

                            var v14 = data[26];
                            data[26] = data[22];
                            data[22] = v14;
                        }
                        else
                        {
                            data[10] ^= 0xC3;
                            data[9] ^= 0x46;
                            var v13 = data[10];
                            data[10] = data[12];
                            data[12] = v13;
                        }
                    }
                    else
                    {
                        var v6 = (byte)(data[2] >> 6);
                        data[2] *= 4;
                        data[2] |= v6;
                        var v18 = (byte)((data[7] >> 4) & 1);
                        if (((data[7] >> 4) & 1) != 0)
                        {
                            data[7] |= 0x10;
                        }
                        else
                        {
                            data[7] &= 0xEF;
                        }

                        if (v18 != 0)
                        {
                            data[7] |= 0x10;
                        }
                        else
                        {
                            data[7] &= 0xEF;
                        }

                        var v7 = (byte)(data[7] >> 7);
                        data[7] *= 2;
                        data[7] |= v7;
                        var v8 = data[3];
                        data[3] = data[4];
                        data[4] = v8;
                        var v9 = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= v9;
                        var v10 = data[5];
                        data[5] = data[0];
                        data[0] = v10;
                        var v11 = (byte)(data[6] >> 2);
                        data[6] <<= 6;
                        data[6] |= v11;
                        var v17 = (byte)((data[0] >> 1) & 1);
                        if (((data[0] >> 2) & 1) != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        if (v17 != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        var v16 = (byte)((data[6] >> 1) & 1);
                        if (((data[6] >> 6) & 1) != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        if (v16 != 0)
                        {
                            data[6] |= 0x40;
                        }
                        else
                        {
                            data[6] &= 0xBF;
                        }

                        data[0] ^= 0x2B;
                        var v12 = data[3];
                        data[3] = data[5];
                        data[5] = v12;
                    }
                }
                else
                {
                    var v3 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v3;
                    data[2] ^= 0xC1;
                    var v4 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v4;
                    data[0] ^= 0x6A;
                    var v5 = data[3];
                    data[3] = data[1];
                    data[1] = v5;
                    var v2 = (byte)((data[0] >> 1) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v19 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v19 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    data[0] ^= 0xDB;
                    data[3] ^= 0x15;
                }
            }
        }
    }
}