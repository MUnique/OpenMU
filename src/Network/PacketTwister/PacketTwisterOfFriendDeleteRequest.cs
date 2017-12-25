// <copyright file="PacketTwisterOfFriendDeleteRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'FriendDeleteRequest' type.
    /// </summary>
    internal class PacketTwisterOfFriendDeleteRequest : IPacketTwister
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
                            var v16 = data[27];
                            data[27] = data[1];
                            data[1] = v16;
                            var v17 = (byte)(data[28] >> 1);
                            data[28] <<= 7;
                            data[28] |= v17;
                            var v20 = (byte)((data[23] >> 6) & 1);
                            if (((data[23] >> 6) & 1) != 0)
                            {
                                data[23] |= 0x40;
                            }
                            else
                            {
                                data[23] &= 0xBF;
                            }

                            if (v20 != 0)
                            {
                                data[23] |= 0x40;
                            }
                            else
                            {
                                data[23] &= 0xBF;
                            }

                            var v18 = (byte)(data[5] >> 4);
                            data[5] *= 16;
                            data[5] |= v18;
                            var v19 = (byte)(data[4] >> 5);
                            data[4] *= 8;
                            data[4] |= v19;
                        }
                        else
                        {
                            var v12 = data[6];
                            data[6] = data[7];
                            data[7] = v12;
                            data[12] ^= 0x36;
                            data[1] ^= 0x86;
                            var v26 = (byte)((data[5] >> 1) & 1);
                            if (((data[5] >> 5) & 1) != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[5] |= 0x20;
                            }
                            else
                            {
                                data[5] &= 0xDF;
                            }

                            var v13 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v13;
                            data[9] ^= 0x95;
                            var v25 = (byte)((data[2] >> 6) & 1);
                            if (((data[2] >> 1) & 1) != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            if (v25 != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            var v24 = (byte)((data[6] >> 7) & 1);
                            if (((data[6] >> 2) & 1) != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            if (v24 != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            var v23 = (byte)((data[0] >> 2) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            var v14 = (byte)(data[13] >> 7);
                            data[13] *= 2;
                            data[13] |= v14;
                            var v22 = (byte)((data[6] >> 7) & 1);
                            if (((data[6] >> 3) & 1) != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            if (v22 != 0)
                            {
                                data[6] |= 8;
                            }
                            else
                            {
                                data[6] &= 0xF7;
                            }

                            var v21 = (byte)((data[11] >> 5) & 1);
                            if (((data[11] >> 2) & 1) != 0)
                            {
                                data[11] |= 0x20;
                            }
                            else
                            {
                                data[11] &= 0xDF;
                            }

                            if (v21 != 0)
                            {
                                data[11] |= 4;
                            }
                            else
                            {
                                data[11] &= 0xFB;
                            }

                            var v15 = (byte)(data[1] >> 3);
                            data[1] *= 32;
                            data[1] |= v15;
                        }
                    }
                    else
                    {
                        data[3] ^= 0x45;
                        data[3] ^= 0x4A;
                        var v11 = data[6];
                        data[6] = data[5];
                        data[5] = v11;
                        data[4] ^= 0xB2;
                        data[0] ^= 0x19;
                        data[0] ^= 0x23;
                        data[2] ^= 0xB7;
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[1];
                    data[1] = v3;
                    var v4 = data[0];
                    data[0] = data[0];
                    data[0] = v4;
                    data[2] ^= 0xE6;
                    var v5 = (byte)(data[0] >> 1);
                    data[0] <<= 7;
                    data[0] |= v5;
                    var v2 = (byte)((data[2] >> 7) & 1);
                    if (((data[2] >> 3) & 1) != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 8;
                    }
                    else
                    {
                        data[2] &= 0xF7;
                    }

                    var v6 = data[2];
                    data[2] = data[1];
                    data[1] = v6;
                    var v7 = data[0];
                    data[0] = data[2];
                    data[2] = v7;
                    data[0] ^= 0x6D;
                    var v8 = data[0];
                    data[0] = data[1];
                    data[1] = v8;
                    var v9 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v9;
                    var v10 = data[0];
                    data[0] = data[1];
                    data[1] = v10;
                    var v27 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v27 != 0)
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
                            var v16 = (byte)(data[4] >> 3);
                            data[4] *= 32;
                            data[4] |= v16;
                            var v17 = (byte)(data[5] >> 4);
                            data[5] *= 16;
                            data[5] |= v17;
                            var v20 = (byte)((data[23] >> 6) & 1);
                            if (((data[23] >> 6) & 1) != 0)
                            {
                                data[23] |= 0x40;
                            }
                            else
                            {
                                data[23] &= 0xBF;
                            }

                            if (v20 != 0)
                            {
                                data[23] |= 0x40;
                            }
                            else
                            {
                                data[23] &= 0xBF;
                            }

                            var v18 = (byte)(data[28] >> 7);
                            data[28] *= 2;
                            data[28] |= v18;
                            var v19 = data[27];
                            data[27] = data[1];
                            data[1] = v19;
                        }
                        else
                        {
                            var v12 = (byte)(data[1] >> 5);
                            data[1] *= 8;
                            data[1] |= v12;
                            var v26 = (byte)((data[11] >> 5) & 1);
                            if (((data[11] >> 2) & 1) != 0)
                            {
                                data[11] |= 0x20;
                            }
                            else
                            {
                                data[11] &= 0xDF;
                            }

                            if (v26 != 0)
                            {
                                data[11] |= 4;
                            }
                            else
                            {
                                data[11] &= 0xFB;
                            }

                            var v25 = (byte)((data[6] >> 7) & 1);
                            if (((data[6] >> 3) & 1) != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            if (v25 != 0)
                            {
                                data[6] |= 8;
                            }
                            else
                            {
                                data[6] &= 0xF7;
                            }

                            var v13 = (byte)(data[13] >> 1);
                            data[13] <<= 7;
                            data[13] |= v13;
                            var v24 = (byte)((data[0] >> 2) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            var v23 = (byte)((data[6] >> 7) & 1);
                            if (((data[6] >> 2) & 1) != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            if (v23 != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            var v22 = (byte)((data[2] >> 6) & 1);
                            if (((data[2] >> 1) & 1) != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            if (v22 != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            data[9] ^= 0x95;
                            var v14 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v14;
                            var v21 = (byte)((data[5] >> 1) & 1);
                            if (((data[5] >> 5) & 1) != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            if (v21 != 0)
                            {
                                data[5] |= 0x20;
                            }
                            else
                            {
                                data[5] &= 0xDF;
                            }

                            data[1] ^= 0x86;
                            data[12] ^= 0x36;
                            var v15 = data[6];
                            data[6] = data[7];
                            data[7] = v15;
                        }
                    }
                    else
                    {
                        data[2] ^= 0xB7;
                        data[0] ^= 0x23;
                        data[0] ^= 0x19;
                        data[4] ^= 0xB2;
                        var v11 = data[6];
                        data[6] = data[5];
                        data[5] = v11;
                        data[3] ^= 0x4A;
                        data[3] ^= 0x45;
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
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
                    data[0] = data[1];
                    data[1] = v3;
                    var v4 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v4;
                    var v5 = data[0];
                    data[0] = data[1];
                    data[1] = v5;
                    data[0] ^= 0x6D;
                    var v6 = data[0];
                    data[0] = data[2];
                    data[2] = v6;
                    var v7 = data[2];
                    data[2] = data[1];
                    data[1] = v7;
                    var v27 = (byte)((data[2] >> 7) & 1);
                    if (((data[2] >> 3) & 1) != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    if (v27 != 0)
                    {
                        data[2] |= 8;
                    }
                    else
                    {
                        data[2] &= 0xF7;
                    }

                    var v8 = (byte)(data[0] >> 7);
                    data[0] *= 2;
                    data[0] |= v8;
                    data[2] ^= 0xE6;
                    var v9 = data[0];
                    data[0] = data[0];
                    data[0] = v9;
                    var v10 = data[3];
                    data[3] = data[1];
                    data[1] = v10;
                }
            }
        }
    }
}