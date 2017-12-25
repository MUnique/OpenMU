// <copyright file="PacketTwister29.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister29 : IPacketTwister
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
                            var v12 = (byte)((data[1] >> 6) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 0x40;
                            }
                            else
                            {
                                data[1] &= 0xBF;
                            }

                            if (v12 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            var v10 = data[6];
                            data[6] = data[28];
                            data[28] = v10;
                            var v11 = (byte)((data[31] >> 2) & 1);
                            if (((data[31] >> 2) & 1) != 0)
                            {
                                data[31] |= 4;
                            }
                            else
                            {
                                data[31] &= 0xFB;
                            }

                            if (v11 != 0)
                            {
                                data[31] |= 4;
                            }
                            else
                            {
                                data[31] &= 0xFB;
                            }
                        }
                        else
                        {
                            var v14 = (byte)((data[0] >> 2) & 1);
                            if (((data[0] >> 5) & 1) != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            if (v14 != 0)
                            {
                                data[0] |= 0x20;
                            }
                            else
                            {
                                data[0] &= 0xDF;
                            }

                            data[14] ^= 0x46;
                            var v8 = (byte)(data[4] >> 3);
                            data[4] *= 32;
                            data[4] |= v8;
                            var v13 = (byte)((data[13] >> 7) & 1);
                            if (((data[13] >> 5) & 1) != 0)
                            {
                                data[13] |= 0x80;
                            }
                            else
                            {
                                data[13] &= 0x7F;
                            }

                            if (v13 != 0)
                            {
                                data[13] |= 0x20;
                            }
                            else
                            {
                                data[13] &= 0xDF;
                            }

                            var v9 = (byte)(data[10] >> 3);
                            data[10] *= 32;
                            data[10] |= v9;
                            data[1] ^= 0xEF;
                            data[12] ^= 0x9E;
                        }
                    }
                    else
                    {
                        var v5 = data[6];
                        data[6] = data[7];
                        data[7] = v5;
                        var v6 = data[3];
                        data[3] = data[4];
                        data[4] = v6;
                        data[7] ^= 0x85;
                        var v15 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 2) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v15 != 0)
                        {
                            data[7] |= 4;
                        }
                        else
                        {
                            data[7] &= 0xFB;
                        }

                        var v7 = data[2];
                        data[2] = data[1];
                        data[1] = v7;
                        data[4] ^= 0x1F;
                    }
                }
                else
                {
                    data[3] ^= 0x49;
                    data[1] ^= 0xD5;
                    var v2 = (byte)((data[0] >> 1) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v3 = (byte)(data[0] >> 1);
                    data[0] <<= 7;
                    data[0] |= v3;
                    var v4 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v4;
                    var v17 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v17 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v16 = (byte)((data[0] >> 6) & 1);
                    if (((data[0] >> 2) & 1) != 0)
                    {
                        data[0] |= 0x40;
                    }
                    else
                    {
                        data[0] &= 0xBF;
                    }

                    if (v16 != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    data[3] ^= 0xC0;
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
                            var v12 = (byte)((data[31] >> 2) & 1);
                            if (((data[31] >> 2) & 1) != 0)
                            {
                                data[31] |= 4;
                            }
                            else
                            {
                                data[31] &= 0xFB;
                            }

                            if (v12 != 0)
                            {
                                data[31] |= 4;
                            }
                            else
                            {
                                data[31] &= 0xFB;
                            }

                            var v10 = data[6];
                            data[6] = data[28];
                            data[28] = v10;
                            var v11 = (byte)((data[1] >> 6) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 0x40;
                            }
                            else
                            {
                                data[1] &= 0xBF;
                            }

                            if (v11 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }
                        }
                        else
                        {
                            data[12] ^= 0x9E;
                            data[1] ^= 0xEF;
                            var v8 = (byte)(data[10] >> 5);
                            data[10] *= 8;
                            data[10] |= v8;
                            var v14 = (byte)((data[13] >> 7) & 1);
                            if (((data[13] >> 5) & 1) != 0)
                            {
                                data[13] |= 0x80;
                            }
                            else
                            {
                                data[13] &= 0x7F;
                            }

                            if (v14 != 0)
                            {
                                data[13] |= 0x20;
                            }
                            else
                            {
                                data[13] &= 0xDF;
                            }

                            var v9 = (byte)(data[4] >> 5);
                            data[4] *= 8;
                            data[4] |= v9;
                            data[14] ^= 0x46;
                            var v13 = (byte)((data[0] >> 2) & 1);
                            if (((data[0] >> 5) & 1) != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            if (v13 != 0)
                            {
                                data[0] |= 0x20;
                            }
                            else
                            {
                                data[0] &= 0xDF;
                            }
                        }
                    }
                    else
                    {
                        data[4] ^= 0x1F;
                        var v5 = data[2];
                        data[2] = data[1];
                        data[1] = v5;
                        var v15 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 2) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v15 != 0)
                        {
                            data[7] |= 4;
                        }
                        else
                        {
                            data[7] &= 0xFB;
                        }

                        data[7] ^= 0x85;
                        var v6 = data[3];
                        data[3] = data[4];
                        data[4] = v6;
                        var v7 = data[6];
                        data[6] = data[7];
                        data[7] = v7;
                    }
                }
                else
                {
                    data[3] ^= 0xC0;
                    var v2 = (byte)((data[0] >> 6) & 1);
                    if (((data[0] >> 2) & 1) != 0)
                    {
                        data[0] |= 0x40;
                    }
                    else
                    {
                        data[0] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    var v17 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v17 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v3 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v3;
                    var v4 = (byte)(data[0] >> 7);
                    data[0] *= 2;
                    data[0] |= v4;
                    var v16 = (byte)((data[0] >> 1) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    if (v16 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    data[1] ^= 0xD5;
                    data[3] ^= 0x49;
                }
            }
        }
    }
}