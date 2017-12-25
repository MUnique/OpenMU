// <copyright file="PacketTwisterOfFriendAddRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'FriendAddRequest' type.
    /// </summary>
    internal class PacketTwisterOfFriendAddRequest : IPacketTwister
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
                            var v15 = (byte)(data[23] >> 1);
                            data[23] <<= 7;
                            data[23] |= v15;
                            var v16 = data[1];
                            data[1] = data[17];
                            data[17] = v16;
                            var v17 = data[4];
                            data[4] = data[21];
                            data[21] = v17;
                            data[15] ^= 0x82;
                            var v21 = (byte)((data[2] >> 2) & 1);
                            if (((data[2] >> 1) & 1) != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            if (v21 != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            var v20 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 1) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            var v18 = (byte)(data[26] >> 4);
                            data[26] *= 16;
                            data[26] |= v18;
                            var v19 = (byte)((data[17] >> 5) & 1);
                            if (((data[17] >> 4) & 1) != 0)
                            {
                                data[17] |= 0x20;
                            }
                            else
                            {
                                data[17] &= 0xDF;
                            }

                            if (v19 != 0)
                            {
                                data[17] |= 0x10;
                            }
                            else
                            {
                                data[17] &= 0xEF;
                            }

                            data[25] ^= 0x9F;
                            data[1] ^= 0x14;
                        }
                        else
                        {
                            var v13 = data[15];
                            data[15] = data[14];
                            data[14] = v13;
                            var v22 = (byte)((data[1] >> 1) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            if (v22 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            var v14 = data[12];
                            data[12] = data[8];
                            data[8] = v14;
                        }
                    }
                    else
                    {
                        var v9 = data[2];
                        data[2] = data[6];
                        data[6] = v9;
                        var v10 = (byte)(data[7] >> 5);
                        data[7] *= 8;
                        data[7] |= v10;
                        var v11 = data[6];
                        data[6] = data[6];
                        data[6] = v11;
                        data[1] ^= 0x5C;
                        var v12 = data[2];
                        data[2] = data[1];
                        data[1] = v12;
                        var v23 = (byte)((data[6] >> 7) & 1);
                        if (((data[6] >> 5) & 1) != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }

                        if (v23 != 0)
                        {
                            data[6] |= 0x20;
                        }
                        else
                        {
                            data[6] &= 0xDF;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[0] >> 5) & 1);
                    if (((data[0] >> 2) & 1) != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    var v3 = data[2];
                    data[2] = data[2];
                    data[2] = v3;
                    var v4 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v4;
                    var v26 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v26 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    var v5 = (byte)(data[2] >> 7);
                    data[2] *= 2;
                    data[2] |= v5;
                    var v6 = data[1];
                    data[1] = data[0];
                    data[0] = v6;
                    var v7 = (byte)(data[2] >> 3);
                    data[2] *= 32;
                    data[2] |= v7;
                    var v25 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 6) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v25 != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    data[1] ^= 0x9A;
                    data[1] ^= 0x51;
                    var v24 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 7) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v24 != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    var v8 = data[0];
                    data[0] = data[1];
                    data[1] = v8;
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
                            data[1] ^= 0x14;
                            data[25] ^= 0x9F;
                            var v21 = (byte)((data[17] >> 5) & 1);
                            if (((data[17] >> 4) & 1) != 0)
                            {
                                data[17] |= 0x20;
                            }
                            else
                            {
                                data[17] &= 0xDF;
                            }

                            if (v21 != 0)
                            {
                                data[17] |= 0x10;
                            }
                            else
                            {
                                data[17] &= 0xEF;
                            }

                            var v15 = (byte)(data[26] >> 4);
                            data[26] *= 16;
                            data[26] |= v15;
                            var v20 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 1) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            var v19 = (byte)((data[2] >> 2) & 1);
                            if (((data[2] >> 1) & 1) != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            if (v19 != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            data[15] ^= 0x82;
                            var v16 = data[4];
                            data[4] = data[21];
                            data[21] = v16;
                            var v17 = data[1];
                            data[1] = data[17];
                            data[17] = v17;
                            var v18 = (byte)(data[23] >> 7);
                            data[23] *= 2;
                            data[23] |= v18;
                        }
                        else
                        {
                            var v13 = data[12];
                            data[12] = data[8];
                            data[8] = v13;
                            var v22 = (byte)((data[1] >> 1) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            if (v22 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            var v14 = data[15];
                            data[15] = data[14];
                            data[14] = v14;
                        }
                    }
                    else
                    {
                        var v23 = (byte)((data[6] >> 7) & 1);
                        if (((data[6] >> 5) & 1) != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }

                        if (v23 != 0)
                        {
                            data[6] |= 0x20;
                        }
                        else
                        {
                            data[6] &= 0xDF;
                        }

                        var v9 = data[2];
                        data[2] = data[1];
                        data[1] = v9;
                        data[1] ^= 0x5C;
                        var v10 = data[6];
                        data[6] = data[6];
                        data[6] = v10;
                        var v11 = (byte)(data[7] >> 3);
                        data[7] *= 32;
                        data[7] |= v11;
                        var v12 = data[2];
                        data[2] = data[6];
                        data[6] = v12;
                    }
                }
                else
                {
                    var v3 = data[0];
                    data[0] = data[1];
                    data[1] = v3;
                    var v2 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 7) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    data[1] ^= 0x51;
                    data[1] ^= 0x9A;
                    var v26 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 6) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v26 != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    var v4 = (byte)(data[2] >> 5);
                    data[2] *= 8;
                    data[2] |= v4;
                    var v5 = data[1];
                    data[1] = data[0];
                    data[0] = v5;
                    var v6 = (byte)(data[2] >> 1);
                    data[2] <<= 7;
                    data[2] |= v6;
                    var v25 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v25 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    var v7 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v7;
                    var v8 = data[2];
                    data[2] = data[2];
                    data[2] = v8;
                    var v24 = (byte)((data[0] >> 5) & 1);
                    if (((data[0] >> 2) & 1) != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    if (v24 != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }
                }
            }
        }
    }
}