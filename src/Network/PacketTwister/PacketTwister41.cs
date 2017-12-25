// <copyright file="PacketTwister41.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister41 : IPacketTwister
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
                            data[10] ^= 0x1B;
                            var v11 = data[14];
                            data[14] = data[22];
                            data[22] = v11;
                            var v12 = (byte)(data[14] >> 1);
                            data[14] <<= 7;
                            data[14] |= v12;
                            var v14 = (byte)((data[19] >> 7) & 1);
                            if (((data[19] >> 5) & 1) != 0)
                            {
                                data[19] |= 0x80;
                            }
                            else
                            {
                                data[19] &= 0x7F;
                            }

                            if (v14 != 0)
                            {
                                data[19] |= 0x20;
                            }
                            else
                            {
                                data[19] &= 0xDF;
                            }

                            var v13 = data[27];
                            data[27] = data[16];
                            data[16] = v13;
                        }
                        else
                        {
                            var v18 = (byte)((data[15] >> 5) & 1);
                            if (((data[15] >> 7) & 1) != 0)
                            {
                                data[15] |= 0x20;
                            }
                            else
                            {
                                data[15] &= 0xDF;
                            }

                            if (v18 != 0)
                            {
                                data[15] |= 0x80;
                            }
                            else
                            {
                                data[15] &= 0x7F;
                            }

                            data[5] ^= 0x2C;
                            var v17 = (byte)((data[7] >> 2) & 1);
                            if (((data[7] >> 1) & 1) != 0)
                            {
                                data[7] |= 4;
                            }
                            else
                            {
                                data[7] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[7] |= 2;
                            }
                            else
                            {
                                data[7] &= 0xFD;
                            }

                            var v9 = data[3];
                            data[3] = data[12];
                            data[12] = v9;
                            var v16 = (byte)((data[13] >> 7) & 1);
                            if (((data[13] >> 6) & 1) != 0)
                            {
                                data[13] |= 0x80;
                            }
                            else
                            {
                                data[13] &= 0x7F;
                            }

                            if (v16 != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            var v15 = (byte)((data[3] >> 7) & 1);
                            if (((data[3] >> 4) & 1) != 0)
                            {
                                data[3] |= 0x80;
                            }
                            else
                            {
                                data[3] &= 0x7F;
                            }

                            if (v15 != 0)
                            {
                                data[3] |= 0x10;
                            }
                            else
                            {
                                data[3] &= 0xEF;
                            }

                            var v10 = (byte)(data[0] >> 5);
                            data[0] *= 8;
                            data[0] |= v10;
                        }
                    }
                    else
                    {
                        var v4 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v4;
                        var v5 = (byte)(data[2] >> 6);
                        data[2] *= 4;
                        data[2] |= v5;
                        var v20 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 5) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v20 != 0)
                        {
                            data[1] |= 0x20;
                        }
                        else
                        {
                            data[1] &= 0xDF;
                        }

                        var v6 = data[0];
                        data[0] = data[0];
                        data[0] = v6;
                        var v7 = (byte)(data[7] >> 7);
                        data[7] *= 2;
                        data[7] |= v7;
                        data[7] ^= 0x9B;
                        data[1] ^= 0x68;
                        var v19 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 1) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v19 != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        var v8 = (byte)(data[2] >> 5);
                        data[2] *= 8;
                        data[2] |= v8;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[1];
                    data[1] = v3;
                    data[1] ^= 0xED;
                    var v2 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 7) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
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
                            var v11 = data[27];
                            data[27] = data[16];
                            data[16] = v11;
                            var v14 = (byte)((data[19] >> 7) & 1);
                            if (((data[19] >> 5) & 1) != 0)
                            {
                                data[19] |= 0x80;
                            }
                            else
                            {
                                data[19] &= 0x7F;
                            }

                            if (v14 != 0)
                            {
                                data[19] |= 0x20;
                            }
                            else
                            {
                                data[19] &= 0xDF;
                            }

                            var v12 = (byte)(data[14] >> 7);
                            data[14] *= 2;
                            data[14] |= v12;
                            var v13 = data[14];
                            data[14] = data[22];
                            data[22] = v13;
                            data[10] ^= 0x1B;
                        }
                        else
                        {
                            var v9 = (byte)(data[0] >> 3);
                            data[0] *= 32;
                            data[0] |= v9;
                            var v18 = (byte)((data[3] >> 7) & 1);
                            if (((data[3] >> 4) & 1) != 0)
                            {
                                data[3] |= 0x80;
                            }
                            else
                            {
                                data[3] &= 0x7F;
                            }

                            if (v18 != 0)
                            {
                                data[3] |= 0x10;
                            }
                            else
                            {
                                data[3] &= 0xEF;
                            }

                            var v17 = (byte)((data[13] >> 7) & 1);
                            if (((data[13] >> 6) & 1) != 0)
                            {
                                data[13] |= 0x80;
                            }
                            else
                            {
                                data[13] &= 0x7F;
                            }

                            if (v17 != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            var v10 = data[3];
                            data[3] = data[12];
                            data[12] = v10;
                            var v16 = (byte)((data[7] >> 2) & 1);
                            if (((data[7] >> 1) & 1) != 0)
                            {
                                data[7] |= 4;
                            }
                            else
                            {
                                data[7] &= 0xFB;
                            }

                            if (v16 != 0)
                            {
                                data[7] |= 2;
                            }
                            else
                            {
                                data[7] &= 0xFD;
                            }

                            data[5] ^= 0x2C;
                            var v15 = (byte)((data[15] >> 5) & 1);
                            if (((data[15] >> 7) & 1) != 0)
                            {
                                data[15] |= 0x20;
                            }
                            else
                            {
                                data[15] &= 0xDF;
                            }

                            if (v15 != 0)
                            {
                                data[15] |= 0x80;
                            }
                            else
                            {
                                data[15] &= 0x7F;
                            }
                        }
                    }
                    else
                    {
                        var v4 = (byte)(data[2] >> 3);
                        data[2] *= 32;
                        data[2] |= v4;
                        var v20 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 1) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v20 != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        data[1] ^= 0x68;
                        data[7] ^= 0x9B;
                        var v5 = (byte)(data[7] >> 1);
                        data[7] <<= 7;
                        data[7] |= v5;
                        var v6 = data[0];
                        data[0] = data[0];
                        data[0] = v6;
                        var v19 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 5) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v19 != 0)
                        {
                            data[1] |= 0x20;
                        }
                        else
                        {
                            data[1] &= 0xDF;
                        }

                        var v7 = (byte)(data[2] >> 2);
                        data[2] <<= 6;
                        data[2] |= v7;
                        var v8 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v8;
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 7) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    data[1] ^= 0xED;
                    var v3 = data[2];
                    data[2] = data[1];
                    data[1] = v3;
                }
            }
        }
    }
}