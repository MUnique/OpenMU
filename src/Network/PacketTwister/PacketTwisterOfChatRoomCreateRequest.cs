// <copyright file="PacketTwisterOfChatRoomCreateRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'ChatRoomCreateRequest' type.
    /// </summary>
    internal class PacketTwisterOfChatRoomCreateRequest : IPacketTwister
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
                            var v18 = (byte)(data[2] >> 5);
                            data[2] *= 8;
                            data[2] |= v18;
                            data[18] ^= 0xE;
                            var v24 = (byte)((data[20] >> 2) & 1);
                            if (((data[20] >> 7) & 1) != 0)
                            {
                                data[20] |= 4;
                            }
                            else
                            {
                                data[20] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[20] |= 0x80;
                            }
                            else
                            {
                                data[20] &= 0x7F;
                            }

                            var v23 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 2) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v23 != 0)
                            {
                                data[4] |= 4;
                            }
                            else
                            {
                                data[4] &= 0xFB;
                            }

                            data[30] ^= 0x61;
                            var v19 = (byte)(data[24] >> 7);
                            data[24] *= 2;
                            data[24] |= v19;
                            data[8] ^= 0x68;
                            var v20 = (byte)(data[17] >> 4);
                            data[17] *= 16;
                            data[17] |= v20;
                            var v21 = data[11];
                            data[11] = data[24];
                            data[24] = v21;
                            var v22 = data[30];
                            data[30] = data[24];
                            data[24] = v22;
                        }
                        else
                        {
                            var v14 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v14;
                            data[3] ^= 0xCE;
                            data[12] ^= 0xFA;
                            data[6] ^= 0x52;
                            data[5] ^= 0xF3;
                            var v15 = (byte)(data[6] >> 5);
                            data[6] *= 8;
                            data[6] |= v15;
                            var v26 = (byte)((data[0] >> 6) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 0x40;
                            }
                            else
                            {
                                data[0] &= 0xBF;
                            }

                            if (v26 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            data[10] ^= 0xF0;
                            var v25 = (byte)((data[2] >> 6) & 1);
                            if (((data[2] >> 3) & 1) != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            if (v25 != 0)
                            {
                                data[2] |= 8;
                            }
                            else
                            {
                                data[2] &= 0xF7;
                            }

                            var v16 = (byte)(data[0] >> 6);
                            data[0] *= 4;
                            data[0] |= v16;
                            var v17 = data[12];
                            data[12] = data[13];
                            data[13] = v17;
                        }
                    }
                    else
                    {
                        var v8 = (byte)(data[3] >> 2);
                        data[3] <<= 6;
                        data[3] |= v8;
                        var v27 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 7) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v27 != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        data[6] ^= 0x73;
                        var v9 = data[4];
                        data[4] = data[4];
                        data[4] = v9;
                        var v10 = data[5];
                        data[5] = data[0];
                        data[0] = v10;
                        var v11 = (byte)(data[3] >> 6);
                        data[3] *= 4;
                        data[3] |= v11;
                        data[1] ^= 0x9D;
                        var v12 = (byte)(data[1] >> 7);
                        data[1] *= 2;
                        data[1] |= v12;
                        var v13 = (byte)(data[7] >> 1);
                        data[7] <<= 7;
                        data[7] |= v13;
                        data[3] ^= 0x48;
                        data[1] ^= 0x60;
                    }
                }
                else
                {
                    var v3 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v3;
                    var v4 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v4;
                    var v5 = data[0];
                    data[0] = data[1];
                    data[1] = v5;
                    var v6 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v6;
                    var v2 = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 4) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    var v7 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v7;
                    var v29 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 7) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v29 != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    var v28 = (byte)((data[1] >> 6) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    if (v28 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
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
                            var v18 = data[30];
                            data[30] = data[24];
                            data[24] = v18;
                            var v19 = data[11];
                            data[11] = data[24];
                            data[24] = v19;
                            var v20 = (byte)(data[17] >> 4);
                            data[17] *= 16;
                            data[17] |= v20;
                            data[8] ^= 0x68;
                            var v21 = (byte)(data[24] >> 1);
                            data[24] <<= 7;
                            data[24] |= v21;
                            data[30] ^= 0x61;
                            var v24 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 2) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v24 != 0)
                            {
                                data[4] |= 4;
                            }
                            else
                            {
                                data[4] &= 0xFB;
                            }

                            var v23 = (byte)((data[20] >> 2) & 1);
                            if (((data[20] >> 7) & 1) != 0)
                            {
                                data[20] |= 4;
                            }
                            else
                            {
                                data[20] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[20] |= 0x80;
                            }
                            else
                            {
                                data[20] &= 0x7F;
                            }

                            data[18] ^= 0xE;
                            var v22 = (byte)(data[2] >> 3);
                            data[2] *= 32;
                            data[2] |= v22;
                        }
                        else
                        {
                            var v14 = data[12];
                            data[12] = data[13];
                            data[13] = v14;
                            var v15 = (byte)(data[0] >> 2);
                            data[0] <<= 6;
                            data[0] |= v15;
                            var v26 = (byte)((data[2] >> 6) & 1);
                            if (((data[2] >> 3) & 1) != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            if (v26 != 0)
                            {
                                data[2] |= 8;
                            }
                            else
                            {
                                data[2] &= 0xF7;
                            }

                            data[10] ^= 0xF0;
                            var v25 = (byte)((data[0] >> 6) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 0x40;
                            }
                            else
                            {
                                data[0] &= 0xBF;
                            }

                            if (v25 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            var v16 = (byte)(data[6] >> 3);
                            data[6] *= 32;
                            data[6] |= v16;
                            data[5] ^= 0xF3;
                            data[6] ^= 0x52;
                            data[12] ^= 0xFA;
                            data[3] ^= 0xCE;
                            var v17 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v17;
                        }
                    }
                    else
                    {
                        data[1] ^= 0x60;
                        data[3] ^= 0x48;
                        var v8 = (byte)(data[7] >> 7);
                        data[7] *= 2;
                        data[7] |= v8;
                        var v9 = (byte)(data[1] >> 1);
                        data[1] <<= 7;
                        data[1] |= v9;
                        data[1] ^= 0x9D;
                        var v10 = (byte)(data[3] >> 2);
                        data[3] <<= 6;
                        data[3] |= v10;
                        var v11 = data[5];
                        data[5] = data[0];
                        data[0] = v11;
                        var v12 = data[4];
                        data[4] = data[4];
                        data[4] = v12;
                        data[6] ^= 0x73;
                        var v27 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 7) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v27 != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        var v13 = (byte)(data[3] >> 6);
                        data[3] *= 4;
                        data[3] |= v13;
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 6) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v29 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 7) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v29 != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    var v3 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v3;
                    var v28 = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 4) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (v28 != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    var v4 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v4;
                    var v5 = data[0];
                    data[0] = data[1];
                    data[1] = v5;
                    var v6 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v6;
                    var v7 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v7;
                }
            }
        }
    }
}