// <copyright file="PacketTwister78.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister78 : IPacketTwister
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
                            var v17 = (byte)((data[2] >> 1) & 1);
                            if (((data[2] >> 2) & 1) != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            if (v17 != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            var v16 = (byte)((data[28] >> 4) & 1);
                            if (((data[28] >> 5) & 1) != 0)
                            {
                                data[28] |= 0x10;
                            }
                            else
                            {
                                data[28] &= 0xEF;
                            }

                            if (v16 != 0)
                            {
                                data[28] |= 0x20;
                            }
                            else
                            {
                                data[28] &= 0xDF;
                            }

                            var v13 = (byte)(data[17] >> 6);
                            data[17] *= 4;
                            data[17] |= v13;
                            var v15 = (byte)((data[23] >> 1) & 1);
                            if (((data[23] >> 5) & 1) != 0)
                            {
                                data[23] |= 2;
                            }
                            else
                            {
                                data[23] &= 0xFD;
                            }

                            if (v15 != 0)
                            {
                                data[23] |= 0x20;
                            }
                            else
                            {
                                data[23] &= 0xDF;
                            }

                            data[16] ^= 0x45;
                            var v14 = (byte)(data[27] >> 6);
                            data[27] *= 4;
                            data[27] |= v14;
                        }
                        else
                        {
                            var v10 = (byte)(data[11] >> 4);
                            data[11] *= 16;
                            data[11] |= v10;
                            var v19 = (byte)((data[5] >> 5) & 1);
                            if (((data[5] >> 3) & 1) != 0)
                            {
                                data[5] |= 0x20;
                            }
                            else
                            {
                                data[5] &= 0xDF;
                            }

                            if (v19 != 0)
                            {
                                data[5] |= 8;
                            }
                            else
                            {
                                data[5] &= 0xF7;
                            }

                            var v18 = (byte)((data[2] >> 7) & 1);
                            if (((data[2] >> 7) & 1) != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            if (v18 != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            var v11 = (byte)(data[10] >> 2);
                            data[10] <<= 6;
                            data[10] |= v11;
                            data[2] ^= 0x24;
                            var v12 = (byte)(data[7] >> 6);
                            data[7] *= 4;
                            data[7] |= v12;
                        }
                    }
                    else
                    {
                        data[4] ^= 0x17;
                        var v21 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v21 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        data[6] ^= 0xBE;
                        var v6 = (byte)(data[4] >> 2);
                        data[4] <<= 6;
                        data[4] |= v6;
                        var v7 = data[3];
                        data[3] = data[1];
                        data[1] = v7;
                        var v8 = (byte)(data[7] >> 6);
                        data[7] *= 4;
                        data[7] |= v8;
                        var v20 = (byte)((data[4] >> 7) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        if (v20 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        var v9 = data[2];
                        data[2] = data[3];
                        data[3] = v9;
                    }
                }
                else
                {
                    data[3] ^= 0xC8;
                    var v2 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    data[3] ^= 0xC0;
                    var v24 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v24 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    var v23 = (byte)((data[0] >> 5) & 1);
                    if (((data[0] >> 7) & 1) != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    if (v23 != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    var v3 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v3;
                    data[3] ^= 0x80;
                    var v4 = data[0];
                    data[0] = data[0];
                    data[0] = v4;
                    var v5 = data[2];
                    data[2] = data[1];
                    data[1] = v5;
                    var v22 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 7) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v22 != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    data[2] ^= 0x57;
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
                            var v13 = (byte)(data[27] >> 2);
                            data[27] <<= 6;
                            data[27] |= v13;
                            data[16] ^= 0x45;
                            var v17 = (byte)((data[23] >> 1) & 1);
                            if (((data[23] >> 5) & 1) != 0)
                            {
                                data[23] |= 2;
                            }
                            else
                            {
                                data[23] &= 0xFD;
                            }

                            if (v17 != 0)
                            {
                                data[23] |= 0x20;
                            }
                            else
                            {
                                data[23] &= 0xDF;
                            }

                            var v14 = (byte)(data[17] >> 2);
                            data[17] <<= 6;
                            data[17] |= v14;
                            var v16 = (byte)((data[28] >> 4) & 1);
                            if (((data[28] >> 5) & 1) != 0)
                            {
                                data[28] |= 0x10;
                            }
                            else
                            {
                                data[28] &= 0xEF;
                            }

                            if (v16 != 0)
                            {
                                data[28] |= 0x20;
                            }
                            else
                            {
                                data[28] &= 0xDF;
                            }

                            var v15 = (byte)((data[2] >> 1) & 1);
                            if (((data[2] >> 2) & 1) != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            if (v15 != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }
                        }
                        else
                        {
                            var v10 = (byte)(data[7] >> 2);
                            data[7] <<= 6;
                            data[7] |= v10;
                            data[2] ^= 0x24;
                            var v11 = (byte)(data[10] >> 6);
                            data[10] *= 4;
                            data[10] |= v11;
                            var v19 = (byte)((data[2] >> 7) & 1);
                            if (((data[2] >> 7) & 1) != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            if (v19 != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            var v18 = (byte)((data[5] >> 5) & 1);
                            if (((data[5] >> 3) & 1) != 0)
                            {
                                data[5] |= 0x20;
                            }
                            else
                            {
                                data[5] &= 0xDF;
                            }

                            if (v18 != 0)
                            {
                                data[5] |= 8;
                            }
                            else
                            {
                                data[5] &= 0xF7;
                            }

                            var v12 = (byte)(data[11] >> 4);
                            data[11] *= 16;
                            data[11] |= v12;
                        }
                    }
                    else
                    {
                        var v6 = data[2];
                        data[2] = data[3];
                        data[3] = v6;
                        var v21 = (byte)((data[4] >> 7) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        if (v21 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        var v7 = (byte)(data[7] >> 2);
                        data[7] <<= 6;
                        data[7] |= v7;
                        var v8 = data[3];
                        data[3] = data[1];
                        data[1] = v8;
                        var v9 = (byte)(data[4] >> 6);
                        data[4] *= 4;
                        data[4] |= v9;
                        data[6] ^= 0xBE;
                        var v20 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v20 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        data[4] ^= 0x17;
                    }
                }
                else
                {
                    data[2] ^= 0x57;
                    var v2 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 7) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    var v3 = data[2];
                    data[2] = data[1];
                    data[1] = v3;
                    var v4 = data[0];
                    data[0] = data[0];
                    data[0] = v4;
                    data[3] ^= 0x80;
                    var v5 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v5;
                    var v24 = (byte)((data[0] >> 5) & 1);
                    if (((data[0] >> 7) & 1) != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    if (v24 != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    var v23 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v23 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    data[3] ^= 0xC0;
                    var v22 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v22 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    data[3] ^= 0xC8;
                }
            }
        }
    }
}