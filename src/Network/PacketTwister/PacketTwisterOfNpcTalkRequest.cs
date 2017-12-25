// <copyright file="PacketTwisterOfNpcTalkRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'NpcTalkRequest' type.
    /// </summary>
    internal class PacketTwisterOfNpcTalkRequest : IPacketTwister
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
                            var v17 = data[30];
                            data[30] = data[8];
                            data[8] = v17;
                            var v18 = data[18];
                            data[18] = data[18];
                            data[18] = v18;
                            var v19 = data[13];
                            data[13] = data[9];
                            data[9] = v19;
                            var v20 = data[8];
                            data[8] = data[29];
                            data[29] = v20;
                            var v22 = (byte)((data[5] >> 3) & 1);
                            if (((data[5] >> 7) & 1) != 0)
                            {
                                data[5] |= 8;
                            }
                            else
                            {
                                data[5] &= 0xF7;
                            }

                            if (v22 != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            var v21 = (byte)((data[10] >> 6) & 1);
                            if (((data[10] >> 1) & 1) != 0)
                            {
                                data[10] |= 0x40;
                            }
                            else
                            {
                                data[10] &= 0xBF;
                            }

                            if (v21 != 0)
                            {
                                data[10] |= 2;
                            }
                            else
                            {
                                data[10] &= 0xFD;
                            }
                        }
                        else
                        {
                            var v12 = (byte)(data[6] >> 4);
                            data[6] *= 16;
                            data[6] |= v12;
                            var v13 = (byte)(data[12] >> 6);
                            data[12] *= 4;
                            data[12] |= v13;
                            data[15] ^= 0xE9;
                            data[9] ^= 0x9E;
                            var v14 = (byte)(data[13] >> 3);
                            data[13] *= 32;
                            data[13] |= v14;
                            var v15 = (byte)(data[2] >> 3);
                            data[2] *= 32;
                            data[2] |= v15;
                            var v24 = (byte)((data[0] >> 5) & 1);
                            if (((data[0] >> 5) & 1) != 0)
                            {
                                data[0] |= 0x20;
                            }
                            else
                            {
                                data[0] &= 0xDF;
                            }

                            if (v24 != 0)
                            {
                                data[0] |= 0x20;
                            }
                            else
                            {
                                data[0] &= 0xDF;
                            }

                            var v16 = data[4];
                            data[4] = data[8];
                            data[8] = v16;
                            var v23 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 1) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[3] |= 2;
                            }
                            else
                            {
                                data[3] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        var v27 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 3) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v27 != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        var v7 = data[3];
                        data[3] = data[0];
                        data[0] = v7;
                        data[0] ^= 0xF;
                        var v26 = (byte)((data[3] >> 4) & 1);
                        if (((data[3] >> 6) & 1) != 0)
                        {
                            data[3] |= 0x10;
                        }
                        else
                        {
                            data[3] &= 0xEF;
                        }

                        if (v26 != 0)
                        {
                            data[3] |= 0x40;
                        }
                        else
                        {
                            data[3] &= 0xBF;
                        }

                        var v8 = data[7];
                        data[7] = data[4];
                        data[4] = v8;
                        data[2] ^= 0x50;
                        var v9 = data[1];
                        data[1] = data[7];
                        data[7] = v9;
                        var v10 = data[4];
                        data[4] = data[4];
                        data[4] = v10;
                        var v11 = data[2];
                        data[2] = data[0];
                        data[0] = v11;
                        data[2] ^= 0xB8;
                        var v25 = (byte)((data[3] >> 5) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 0x20;
                        }
                        else
                        {
                            data[3] &= 0xDF;
                        }

                        if (v25 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    var v2 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v4 = data[1];
                    data[1] = data[2];
                    data[2] = v4;
                    data[3] ^= 0xDF;
                    var v5 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v5;
                    data[2] = data[2];
                    var v6 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v6;
                    var v28 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 4) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v28 != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
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
                            var v22 = (byte)((data[10] >> 6) & 1);
                            if (((data[10] >> 1) & 1) != 0)
                            {
                                data[10] |= 0x40;
                            }
                            else
                            {
                                data[10] &= 0xBF;
                            }

                            if (v22 != 0)
                            {
                                data[10] |= 2;
                            }
                            else
                            {
                                data[10] &= 0xFD;
                            }

                            var v21 = (byte)((data[5] >> 3) & 1);
                            if (((data[5] >> 7) & 1) != 0)
                            {
                                data[5] |= 8;
                            }
                            else
                            {
                                data[5] &= 0xF7;
                            }

                            if (v21 != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            var v17 = data[8];
                            data[8] = data[29];
                            data[29] = v17;
                            var v18 = data[13];
                            data[13] = data[9];
                            data[9] = v18;
                            var v19 = data[18];
                            data[18] = data[18];
                            data[18] = v19;
                            var v20 = data[30];
                            data[30] = data[8];
                            data[8] = v20;
                        }
                        else
                        {
                            var v24 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 1) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[3] |= 2;
                            }
                            else
                            {
                                data[3] &= 0xFD;
                            }

                            var v12 = data[4];
                            data[4] = data[8];
                            data[8] = v12;
                            var v23 = (byte)((data[0] >> 5) & 1);
                            if (((data[0] >> 5) & 1) != 0)
                            {
                                data[0] |= 0x20;
                            }
                            else
                            {
                                data[0] &= 0xDF;
                            }

                            if (v23 != 0)
                            {
                                data[0] |= 0x20;
                            }
                            else
                            {
                                data[0] &= 0xDF;
                            }

                            var v13 = (byte)(data[2] >> 5);
                            data[2] *= 8;
                            data[2] |= v13;
                            var v14 = (byte)(data[13] >> 5);
                            data[13] *= 8;
                            data[13] |= v14;
                            data[9] ^= 0x9E;
                            data[15] ^= 0xE9;
                            var v15 = (byte)(data[12] >> 2);
                            data[12] <<= 6;
                            data[12] |= v15;
                            var v16 = (byte)(data[6] >> 4);
                            data[6] *= 16;
                            data[6] |= v16;
                        }
                    }
                    else
                    {
                        var v27 = (byte)((data[3] >> 5) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 0x20;
                        }
                        else
                        {
                            data[3] &= 0xDF;
                        }

                        if (v27 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        data[2] ^= 0xB8;
                        var v7 = data[2];
                        data[2] = data[0];
                        data[0] = v7;
                        var v8 = data[4];
                        data[4] = data[4];
                        data[4] = v8;
                        var v9 = data[1];
                        data[1] = data[7];
                        data[7] = v9;
                        data[2] ^= 0x50;
                        var v10 = data[7];
                        data[7] = data[4];
                        data[4] = v10;
                        var v26 = (byte)((data[3] >> 4) & 1);
                        if (((data[3] >> 6) & 1) != 0)
                        {
                            data[3] |= 0x10;
                        }
                        else
                        {
                            data[3] &= 0xEF;
                        }

                        if (v26 != 0)
                        {
                            data[3] |= 0x40;
                        }
                        else
                        {
                            data[3] &= 0xBF;
                        }

                        data[0] ^= 0xF;
                        var v11 = data[3];
                        data[3] = data[0];
                        data[0] = v11;
                        var v25 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 3) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v25 != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }
                    }
                }
                else
                {
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

                    var v3 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v3;
                    data[2] = data[2];
                    var v4 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v4;
                    data[3] ^= 0xDF;
                    var v5 = data[1];
                    data[1] = data[2];
                    data[2] = v5;
                    var v28 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v28 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v6 = data[1];
                    data[1] = data[2];
                    data[2] = v6;
                }
            }
        }
    }
}