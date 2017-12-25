// <copyright file="PacketTwisterOfFriendList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'FriendList' type.
    /// </summary>
    internal class PacketTwisterOfFriendList : IPacketTwister
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
                            var v13 = data[27];
                            data[27] = data[18];
                            data[18] = v13;
                            data[30] ^= 0x33;
                            var v14 = data[3];
                            data[3] = data[15];
                            data[15] = v14;
                            var v15 = data[3];
                            data[3] = data[3];
                            data[3] = v15;
                            var v16 = data[16];
                            data[16] = data[16];
                            data[16] = v16;
                            var v17 = (byte)((data[25] >> 3) & 1);
                            if (((data[25] >> 4) & 1) != 0)
                            {
                                data[25] |= 8;
                            }
                            else
                            {
                                data[25] &= 0xF7;
                            }

                            if (v17 != 0)
                            {
                                data[25] |= 0x10;
                            }
                            else
                            {
                                data[25] &= 0xEF;
                            }
                        }
                        else
                        {
                            var v9 = (byte)(data[9] >> 7);
                            data[9] *= 2;
                            data[9] |= v9;
                            data[7] ^= 0x88;
                            var v10 = data[15];
                            data[15] = data[11];
                            data[11] = v10;
                            data[4] ^= 0x3B;
                            var v11 = (byte)(data[4] >> 1);
                            data[4] <<= 7;
                            data[4] |= v11;
                            var v18 = (byte)((data[15] >> 4) & 1);
                            if (((data[15] >> 2) & 1) != 0)
                            {
                                data[15] |= 0x10;
                            }
                            else
                            {
                                data[15] &= 0xEF;
                            }

                            if (v18 != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            var v12 = (byte)(data[9] >> 5);
                            data[9] *= 8;
                            data[9] |= v12;
                            data[11] ^= 0x18;
                            data[6] ^= 0x26;
                            data[14] ^= 0x20;
                        }
                    }
                    else
                    {
                        data[0] ^= 0xCC;
                        var v6 = (byte)(data[2] >> 7);
                        data[2] *= 2;
                        data[2] |= v6;
                        data[2] ^= 0x8D;
                        var v22 = (byte)((data[6] >> 3) & 1);
                        if (((data[6] >> 3) & 1) != 0)
                        {
                            data[6] |= 8;
                        }
                        else
                        {
                            data[6] &= 0xF7;
                        }

                        if (v22 != 0)
                        {
                            data[6] |= 8;
                        }
                        else
                        {
                            data[6] &= 0xF7;
                        }

                        data[0] ^= 0x10;
                        var v21 = (byte)((data[1] >> 3) & 1);
                        if (((data[1] >> 3) & 1) != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        if (v21 != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        var v7 = (byte)(data[1] >> 1);
                        data[1] <<= 7;
                        data[1] |= v7;
                        var v8 = data[6];
                        data[6] = data[1];
                        data[1] = v8;
                        var v20 = (byte)((data[2] >> 4) & 1);
                        if (((data[2] >> 2) & 1) != 0)
                        {
                            data[2] |= 0x10;
                        }
                        else
                        {
                            data[2] &= 0xEF;
                        }

                        if (v20 != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        var v19 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v19 != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }
                    }
                }
                else
                {
                    var v3 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v3;
                    data[0] ^= 0x39;
                    data[3] ^= 0x63;
                    var v2 = (byte)((data[2] >> 6) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    data[1] ^= 0x41;
                    var v4 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v4;
                    var v5 = data[0];
                    data[0] = data[3];
                    data[3] = v5;
                    var v23 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v23 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }
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
                            var v17 = (byte)((data[25] >> 3) & 1);
                            if (((data[25] >> 4) & 1) != 0)
                            {
                                data[25] |= 8;
                            }
                            else
                            {
                                data[25] &= 0xF7;
                            }

                            if (v17 != 0)
                            {
                                data[25] |= 0x10;
                            }
                            else
                            {
                                data[25] &= 0xEF;
                            }

                            var v13 = data[16];
                            data[16] = data[16];
                            data[16] = v13;
                            var v14 = data[3];
                            data[3] = data[3];
                            data[3] = v14;
                            var v15 = data[3];
                            data[3] = data[15];
                            data[15] = v15;
                            data[30] ^= 0x33;
                            var v16 = data[27];
                            data[27] = data[18];
                            data[18] = v16;
                        }
                        else
                        {
                            data[14] ^= 0x20;
                            data[6] ^= 0x26;
                            data[11] ^= 0x18;
                            var v9 = (byte)(data[9] >> 3);
                            data[9] *= 32;
                            data[9] |= v9;
                            var v18 = (byte)((data[15] >> 4) & 1);
                            if (((data[15] >> 2) & 1) != 0)
                            {
                                data[15] |= 0x10;
                            }
                            else
                            {
                                data[15] &= 0xEF;
                            }

                            if (v18 != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            var v10 = (byte)(data[4] >> 7);
                            data[4] *= 2;
                            data[4] |= v10;
                            data[4] ^= 0x3B;
                            var v11 = data[15];
                            data[15] = data[11];
                            data[11] = v11;
                            data[7] ^= 0x88;
                            var v12 = (byte)(data[9] >> 1);
                            data[9] <<= 7;
                            data[9] |= v12;
                        }
                    }
                    else
                    {
                        var v22 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v22 != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        var v21 = (byte)((data[2] >> 4) & 1);
                        if (((data[2] >> 2) & 1) != 0)
                        {
                            data[2] |= 0x10;
                        }
                        else
                        {
                            data[2] &= 0xEF;
                        }

                        if (v21 != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        var v6 = data[6];
                        data[6] = data[1];
                        data[1] = v6;
                        var v7 = (byte)(data[1] >> 7);
                        data[1] *= 2;
                        data[1] |= v7;
                        var v20 = (byte)((data[1] >> 3) & 1);
                        if (((data[1] >> 3) & 1) != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        if (v20 != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        data[0] ^= 0x10;
                        var v19 = (byte)((data[6] >> 3) & 1);
                        if (((data[6] >> 3) & 1) != 0)
                        {
                            data[6] |= 8;
                        }
                        else
                        {
                            data[6] &= 0xF7;
                        }

                        if (v19 != 0)
                        {
                            data[6] |= 8;
                        }
                        else
                        {
                            data[6] &= 0xF7;
                        }

                        data[2] ^= 0x8D;
                        var v8 = (byte)(data[2] >> 1);
                        data[2] <<= 7;
                        data[2] |= v8;
                        data[0] ^= 0xCC;
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v3 = data[0];
                    data[0] = data[3];
                    data[3] = v3;
                    var v4 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v4;
                    data[1] ^= 0x41;
                    var v23 = (byte)((data[2] >> 6) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    if (v23 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    data[3] ^= 0x63;
                    data[0] ^= 0x39;
                    var v5 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v5;
                }
            }
        }
    }
}