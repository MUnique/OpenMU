// <copyright file="PacketTwisterOfGuildList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'GuildList' type.
    /// </summary>
    internal class PacketTwisterOfGuildList : IPacketTwister
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
                            var v12 = (byte)(data[9] >> 5);
                            data[9] *= 8;
                            data[9] |= v12;
                            var v13 = (byte)(data[22] >> 3);
                            data[22] *= 32;
                            data[22] |= v13;
                            data[25] ^= 0xC3;
                            var v14 = (byte)(data[3] >> 3);
                            data[3] *= 32;
                            data[3] |= v14;
                            var v15 = data[17];
                            data[17] = data[26];
                            data[26] = v15;
                            data[20] ^= 0xD9;
                            var v16 = data[7];
                            data[7] = data[3];
                            data[3] = v16;
                            var v17 = (byte)(data[11] >> 1);
                            data[11] <<= 7;
                            data[11] |= v17;
                        }
                        else
                        {
                            var v19 = (byte)((data[6] >> 2) & 1);
                            if (((data[6] >> 6) & 1) != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            if (v19 != 0)
                            {
                                data[6] |= 0x40;
                            }
                            else
                            {
                                data[6] &= 0xBF;
                            }

                            var v10 = (byte)(data[12] >> 5);
                            data[12] *= 8;
                            data[12] |= v10;
                            var v18 = (byte)((data[6] >> 5) & 1);
                            if (((data[6] >> 1) & 1) != 0)
                            {
                                data[6] |= 0x20;
                            }
                            else
                            {
                                data[6] &= 0xDF;
                            }

                            if (v18 != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            var v11 = (byte)(data[10] >> 7);
                            data[10] *= 2;
                            data[10] |= v11;
                            data[3] ^= 0xF4;
                        }
                    }
                    else
                    {
                        data[1] ^= 0xD8;
                        var v21 = (byte)((data[4] >> 1) & 1);
                        if (((data[4] >> 7) & 1) != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        if (v21 != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        var v8 = data[6];
                        data[6] = data[5];
                        data[5] = v8;
                        data[5] ^= 0x19;
                        data[6] ^= 0x51;
                        data[0] ^= 0x5B;
                        data[0] ^= 0x65;
                        var v9 = data[1];
                        data[1] = data[2];
                        data[2] = v9;
                        var v20 = (byte)((data[6] >> 6) & 1);
                        if (((data[6] >> 1) & 1) != 0)
                        {
                            data[6] |= 0x40;
                        }
                        else
                        {
                            data[6] &= 0xBF;
                        }

                        if (v20 != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        data[6] ^= 0xB4;
                    }
                }
                else
                {
                    data[0] ^= 0xA9;
                    var v3 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v3;
                    data[1] ^= 0x4D;
                    var v4 = data[1];
                    data[1] = data[1];
                    data[1] = v4;
                    data[0] ^= 0x48;
                    data[0] ^= 0x50;
                    data[3] ^= 0x32;
                    var v5 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v5;
                    var v6 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v6;
                    var v2 = (byte)((data[2] >> 1) & 1);
                    if (((data[2] >> 6) & 1) != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    var v7 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v7;
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
                            var v12 = (byte)(data[11] >> 7);
                            data[11] *= 2;
                            data[11] |= v12;
                            var v13 = data[7];
                            data[7] = data[3];
                            data[3] = v13;
                            data[20] ^= 0xD9;
                            var v14 = data[17];
                            data[17] = data[26];
                            data[26] = v14;
                            var v15 = (byte)(data[3] >> 5);
                            data[3] *= 8;
                            data[3] |= v15;
                            data[25] ^= 0xC3;
                            var v16 = (byte)(data[22] >> 5);
                            data[22] *= 8;
                            data[22] |= v16;
                            var v17 = (byte)(data[9] >> 3);
                            data[9] *= 32;
                            data[9] |= v17;
                        }
                        else
                        {
                            data[3] ^= 0xF4;
                            var v10 = (byte)(data[10] >> 1);
                            data[10] <<= 7;
                            data[10] |= v10;
                            var v19 = (byte)((data[6] >> 5) & 1);
                            if (((data[6] >> 1) & 1) != 0)
                            {
                                data[6] |= 0x20;
                            }
                            else
                            {
                                data[6] &= 0xDF;
                            }

                            if (v19 != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            var v11 = (byte)(data[12] >> 3);
                            data[12] *= 32;
                            data[12] |= v11;
                            var v18 = (byte)((data[6] >> 2) & 1);
                            if (((data[6] >> 6) & 1) != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[6] |= 0x40;
                            }
                            else
                            {
                                data[6] &= 0xBF;
                            }
                        }
                    }
                    else
                    {
                        data[6] ^= 0xB4;
                        var v21 = (byte)((data[6] >> 6) & 1);
                        if (((data[6] >> 1) & 1) != 0)
                        {
                            data[6] |= 0x40;
                        }
                        else
                        {
                            data[6] &= 0xBF;
                        }

                        if (v21 != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        var v8 = data[1];
                        data[1] = data[2];
                        data[2] = v8;
                        data[0] ^= 0x65;
                        data[0] ^= 0x5B;
                        data[6] ^= 0x51;
                        data[5] ^= 0x19;
                        var v9 = data[6];
                        data[6] = data[5];
                        data[5] = v9;
                        var v20 = (byte)((data[4] >> 1) & 1);
                        if (((data[4] >> 7) & 1) != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        if (v20 != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        data[1] ^= 0xD8;
                    }
                }
                else
                {
                    var v3 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v3;
                    var v2 = (byte)((data[2] >> 1) & 1);
                    if (((data[2] >> 6) & 1) != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    var v4 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v4;
                    var v5 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v5;
                    data[3] ^= 0x32;
                    data[0] ^= 0x50;
                    data[0] ^= 0x48;
                    var v6 = data[1];
                    data[1] = data[1];
                    data[1] = v6;
                    data[1] ^= 0x4D;
                    var v7 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v7;
                    data[0] ^= 0xA9;
                }
            }
        }
    }
}