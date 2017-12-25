// <copyright file="PacketTwisterOfFriendStateChange.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'FriendStateChange' type.
    /// </summary>
    internal class PacketTwisterOfFriendStateChange : IPacketTwister
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
                            var v20 = data[6];
                            data[6] = data[17];
                            data[17] = v20;
                            data[12] ^= 0x39;
                            var v24 = (byte)((data[26] >> 6) & 1);
                            if (((data[26] >> 4) & 1) != 0)
                            {
                                data[26] |= 0x40;
                            }
                            else
                            {
                                data[26] &= 0xBF;
                            }

                            if (v24 != 0)
                            {
                                data[26] |= 0x10;
                            }
                            else
                            {
                                data[26] &= 0xEF;
                            }

                            var v21 = (byte)(data[0] >> 2);
                            data[0] <<= 6;
                            data[0] |= v21;
                            var v22 = data[16];
                            data[16] = data[10];
                            data[10] = v22;
                            var v23 = (byte)((data[13] >> 2) & 1);
                            if (((data[13] >> 1) & 1) != 0)
                            {
                                data[13] |= 4;
                            }
                            else
                            {
                                data[13] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[13] |= 2;
                            }
                            else
                            {
                                data[13] &= 0xFD;
                            }
                        }
                        else
                        {
                            data[12] ^= 0xEA;
                            var v15 = (byte)(data[8] >> 3);
                            data[8] *= 32;
                            data[8] |= v15;
                            var v26 = (byte)((data[8] >> 2) & 1);
                            if (((data[8] >> 2) & 1) != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            if (v26 != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            data[7] ^= 0x8D;
                            var v25 = (byte)((data[12] >> 1) & 1);
                            if (((data[12] >> 1) & 1) != 0)
                            {
                                data[12] |= 2;
                            }
                            else
                            {
                                data[12] &= 0xFD;
                            }

                            if (v25 != 0)
                            {
                                data[12] |= 2;
                            }
                            else
                            {
                                data[12] &= 0xFD;
                            }

                            var v16 = (byte)(data[0] >> 1);
                            data[0] <<= 7;
                            data[0] |= v16;
                            var v17 = data[14];
                            data[14] = data[13];
                            data[13] = v17;
                            var v18 = data[4];
                            data[4] = data[8];
                            data[8] = v18;
                            var v19 = (byte)(data[5] >> 4);
                            data[5] *= 16;
                            data[5] |= v19;
                        }
                    }
                    else
                    {
                        var v10 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v10;
                        var v11 = data[2];
                        data[2] = data[2];
                        data[2] = v11;
                        data[2] ^= 0x74;
                        data[4] ^= 0x44;
                        var v12 = (byte)(data[3] >> 5);
                        data[3] *= 8;
                        data[3] |= v12;
                        var v13 = data[3];
                        data[3] = data[6];
                        data[6] = v13;
                        data[3] ^= 0x3F;
                        var v14 = data[6];
                        data[6] = data[4];
                        data[4] = v14;
                        data[7] ^= 0xF8;
                        var v27 = (byte)((data[2] >> 6) & 1);
                        if (((data[2] >> 2) & 1) != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        if (v27 != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        data[1] ^= 0x17;
                    }
                }
                else
                {
                    var v3 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v3;
                    var v4 = data[1];
                    data[1] = data[3];
                    data[3] = v4;
                    var v5 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v5;
                    var v6 = (byte)(data[0] >> 7);
                    data[0] *= 2;
                    data[0] |= v6;
                    data[3] ^= 0xC2;
                    var v7 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v7;
                    var v2 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 5) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x20;
                    }
                    else
                    {
                        data[1] &= 0xDF;
                    }

                    var v8 = data[0];
                    data[0] = data[1];
                    data[1] = v8;
                    data[3] ^= 0x8C;
                    var v9 = data[3];
                    data[3] = data[2];
                    data[2] = v9;
                    data[0] ^= 0xB6;
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
                            var v24 = (byte)((data[13] >> 2) & 1);
                            if (((data[13] >> 1) & 1) != 0)
                            {
                                data[13] |= 4;
                            }
                            else
                            {
                                data[13] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[13] |= 2;
                            }
                            else
                            {
                                data[13] &= 0xFD;
                            }

                            var v20 = data[16];
                            data[16] = data[10];
                            data[10] = v20;
                            var v21 = (byte)(data[0] >> 6);
                            data[0] *= 4;
                            data[0] |= v21;
                            var v23 = (byte)((data[26] >> 6) & 1);
                            if (((data[26] >> 4) & 1) != 0)
                            {
                                data[26] |= 0x40;
                            }
                            else
                            {
                                data[26] &= 0xBF;
                            }

                            if (v23 != 0)
                            {
                                data[26] |= 0x10;
                            }
                            else
                            {
                                data[26] &= 0xEF;
                            }

                            data[12] ^= 0x39;
                            var v22 = data[6];
                            data[6] = data[17];
                            data[17] = v22;
                        }
                        else
                        {
                            var v15 = (byte)(data[5] >> 4);
                            data[5] *= 16;
                            data[5] |= v15;
                            var v16 = data[4];
                            data[4] = data[8];
                            data[8] = v16;
                            var v17 = data[14];
                            data[14] = data[13];
                            data[13] = v17;
                            var v18 = (byte)(data[0] >> 7);
                            data[0] *= 2;
                            data[0] |= v18;
                            var v26 = (byte)((data[12] >> 1) & 1);
                            if (((data[12] >> 1) & 1) != 0)
                            {
                                data[12] |= 2;
                            }
                            else
                            {
                                data[12] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[12] |= 2;
                            }
                            else
                            {
                                data[12] &= 0xFD;
                            }

                            data[7] ^= 0x8D;
                            var v25 = (byte)((data[8] >> 2) & 1);
                            if (((data[8] >> 2) & 1) != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            if (v25 != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            var v19 = (byte)(data[8] >> 5);
                            data[8] *= 8;
                            data[8] |= v19;
                            data[12] ^= 0xEA;
                        }
                    }
                    else
                    {
                        data[1] ^= 0x17;
                        var v27 = (byte)((data[2] >> 6) & 1);
                        if (((data[2] >> 2) & 1) != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        if (v27 != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        data[7] ^= 0xF8;
                        var v10 = data[6];
                        data[6] = data[4];
                        data[4] = v10;
                        data[3] ^= 0x3F;
                        var v11 = data[3];
                        data[3] = data[6];
                        data[6] = v11;
                        var v12 = (byte)(data[3] >> 3);
                        data[3] *= 32;
                        data[3] |= v12;
                        data[4] ^= 0x44;
                        data[2] ^= 0x74;
                        var v13 = data[2];
                        data[2] = data[2];
                        data[2] = v13;
                        var v14 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v14;
                    }
                }
                else
                {
                    data[0] ^= 0xB6;
                    var v3 = data[3];
                    data[3] = data[2];
                    data[2] = v3;
                    data[3] ^= 0x8C;
                    var v4 = data[0];
                    data[0] = data[1];
                    data[1] = v4;
                    var v2 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 5) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x20;
                    }
                    else
                    {
                        data[1] &= 0xDF;
                    }

                    var v5 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v5;
                    data[3] ^= 0xC2;
                    var v6 = (byte)(data[0] >> 1);
                    data[0] <<= 7;
                    data[0] |= v6;
                    var v7 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v7;
                    var v8 = data[1];
                    data[1] = data[3];
                    data[3] = v8;
                    var v9 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v9;
                }
            }
        }
    }
}