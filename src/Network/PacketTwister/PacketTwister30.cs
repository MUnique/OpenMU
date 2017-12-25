// <copyright file="PacketTwister30.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister30 : IPacketTwister
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
                            var v16 = data[16];
                            data[16] = data[9];
                            data[9] = v16;
                            data[18] ^= 0x8F;
                            var v17 = (byte)(data[7] >> 3);
                            data[7] *= 32;
                            data[7] |= v17;
                            var v21 = (byte)((data[22] >> 1) & 1);
                            if (((data[22] >> 4) & 1) != 0)
                            {
                                data[22] |= 2;
                            }
                            else
                            {
                                data[22] &= 0xFD;
                            }

                            if (v21 != 0)
                            {
                                data[22] |= 0x10;
                            }
                            else
                            {
                                data[22] &= 0xEF;
                            }

                            var v18 = data[23];
                            data[23] = data[24];
                            data[24] = v18;
                            var v19 = data[18];
                            data[18] = data[21];
                            data[21] = v19;
                            var v20 = data[8];
                            data[8] = data[14];
                            data[14] = v20;
                        }
                        else
                        {
                            var v13 = (byte)(data[13] >> 4);
                            data[13] *= 16;
                            data[13] |= v13;
                            var v14 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v14;
                            data[1] ^= 0xA3;
                            var v23 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 7) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[15] |= 0x80;
                            }
                            else
                            {
                                data[15] &= 0x7F;
                            }

                            var v22 = (byte)((data[8] >> 2) & 1);
                            if (((data[8] >> 1) & 1) != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[8] |= 2;
                            }
                            else
                            {
                                data[8] &= 0xFD;
                            }

                            var v15 = data[2];
                            data[2] = data[8];
                            data[8] = v15;
                        }
                    }
                    else
                    {
                        var v7 = data[0];
                        data[0] = data[4];
                        data[4] = v7;
                        var v8 = (byte)(data[5] >> 5);
                        data[5] *= 8;
                        data[5] |= v8;
                        var v26 = (byte)((data[1] >> 3) & 1);
                        if (((data[1] >> 3) & 1) != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        if (v26 != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        var v9 = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= v9;
                        var v10 = data[3];
                        data[3] = data[5];
                        data[5] = v10;
                        var v25 = (byte)((data[2] >> 4) & 1);
                        if (((data[2] >> 1) & 1) != 0)
                        {
                            data[2] |= 0x10;
                        }
                        else
                        {
                            data[2] &= 0xEF;
                        }

                        if (v25 != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        var v11 = data[6];
                        data[6] = data[3];
                        data[3] = v11;
                        var v12 = (byte)(data[0] >> 7);
                        data[0] *= 2;
                        data[0] |= v12;
                        var v24 = (byte)((data[5] >> 1) & 1);
                        if (((data[5] >> 2) & 1) != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        if (v24 != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[0];
                    data[0] = v3;
                    var v2 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 7) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    data[0] ^= 0x18;
                    var v4 = (byte)(data[0] >> 7);
                    data[0] *= 2;
                    data[0] |= v4;
                    data[0] ^= 0x8E;
                    data[0] ^= 0xE7;
                    var v5 = (byte)(data[1] >> 3);
                    data[1] *= 32;
                    data[1] |= v5;
                    var v6 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v6;
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
                            var v16 = data[8];
                            data[8] = data[14];
                            data[14] = v16;
                            var v17 = data[18];
                            data[18] = data[21];
                            data[21] = v17;
                            var v18 = data[23];
                            data[23] = data[24];
                            data[24] = v18;
                            var v21 = (byte)((data[22] >> 1) & 1);
                            if (((data[22] >> 4) & 1) != 0)
                            {
                                data[22] |= 2;
                            }
                            else
                            {
                                data[22] &= 0xFD;
                            }

                            if (v21 != 0)
                            {
                                data[22] |= 0x10;
                            }
                            else
                            {
                                data[22] &= 0xEF;
                            }

                            var v19 = (byte)(data[7] >> 5);
                            data[7] *= 8;
                            data[7] |= v19;
                            data[18] ^= 0x8F;
                            var v20 = data[16];
                            data[16] = data[9];
                            data[9] = v20;
                        }
                        else
                        {
                            var v13 = data[2];
                            data[2] = data[8];
                            data[8] = v13;
                            var v23 = (byte)((data[8] >> 2) & 1);
                            if (((data[8] >> 1) & 1) != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[8] |= 2;
                            }
                            else
                            {
                                data[8] &= 0xFD;
                            }

                            var v22 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 7) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[15] |= 0x80;
                            }
                            else
                            {
                                data[15] &= 0x7F;
                            }

                            data[1] ^= 0xA3;
                            var v14 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v14;
                            var v15 = (byte)(data[13] >> 4);
                            data[13] *= 16;
                            data[13] |= v15;
                        }
                    }
                    else
                    {
                        var v26 = (byte)((data[5] >> 1) & 1);
                        if (((data[5] >> 2) & 1) != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        if (v26 != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        var v7 = (byte)(data[0] >> 1);
                        data[0] <<= 7;
                        data[0] |= v7;
                        var v8 = data[6];
                        data[6] = data[3];
                        data[3] = v8;
                        var v25 = (byte)((data[2] >> 4) & 1);
                        if (((data[2] >> 1) & 1) != 0)
                        {
                            data[2] |= 0x10;
                        }
                        else
                        {
                            data[2] &= 0xEF;
                        }

                        if (v25 != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        var v9 = data[3];
                        data[3] = data[5];
                        data[5] = v9;
                        var v10 = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= v10;
                        var v24 = (byte)((data[1] >> 3) & 1);
                        if (((data[1] >> 3) & 1) != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        if (v24 != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        var v11 = (byte)(data[5] >> 3);
                        data[5] *= 32;
                        data[5] |= v11;
                        var v12 = data[0];
                        data[0] = data[4];
                        data[4] = v12;
                    }
                }
                else
                {
                    var v3 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v3;
                    var v4 = (byte)(data[1] >> 5);
                    data[1] *= 8;
                    data[1] |= v4;
                    data[0] ^= 0xE7;
                    data[0] ^= 0x8E;
                    var v5 = (byte)(data[0] >> 1);
                    data[0] <<= 7;
                    data[0] |= v5;
                    data[0] ^= 0x18;
                    var v2 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 7) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    var v6 = data[3];
                    data[3] = data[0];
                    data[0] = v6;
                }
            }
        }
    }
}