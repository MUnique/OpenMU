// <copyright file="PacketTwister48.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister48 : IPacketTwister
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
                            data[27] = data[27];
                            var v13 = (byte)((data[13] >> 7) & 1);
                            if (((data[13] >> 6) & 1) != 0)
                            {
                                data[13] |= 0x80;
                            }
                            else
                            {
                                data[13] &= 0x7F;
                            }

                            if (v13 != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            data[11] ^= 0xD3;
                            var v11 = (byte)(data[1] >> 1);
                            data[1] <<= 7;
                            data[1] |= v11;
                            var v12 = (byte)((data[2] >> 6) & 1);
                            if (((data[2] >> 7) & 1) != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            if (v12 != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }
                        }
                        else
                        {
                            var v8 = (byte)(data[2] >> 5);
                            data[2] *= 8;
                            data[2] |= v8;
                            data[7] ^= 0x3C;
                            var v9 = data[3];
                            data[3] = data[11];
                            data[11] = v9;
                            data[12] ^= 0xF9;
                            var v14 = (byte)((data[2] >> 6) & 1);
                            if (((data[2] >> 2) & 1) != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            if (v14 != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            var v10 = data[15];
                            data[15] = data[7];
                            data[7] = v10;
                            data[11] ^= 0xB;
                            data[0] ^= 0x2A;
                        }
                    }
                    else
                    {
                        data[5] ^= 0xC1;
                        var v7 = data[0];
                        data[0] = data[2];
                        data[2] = v7;
                        data[7] ^= 0x34;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[3];
                    data[3] = v3;
                    data[2] ^= 0x95;
                    data[0] ^= 0xDA;
                    var v4 = data[0];
                    data[0] = data[0];
                    data[0] = v4;
                    var v5 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v5;
                    var v6 = data[3];
                    data[3] = data[3];
                    data[3] = v6;
                    var v2 = (byte)((data[0] >> 5) & 1);
                    if (((data[0] >> 7) & 1) != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    data[1] ^= 0x89;
                    var v16 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 4) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v16 != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    var v15 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v15 != 0)
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
                            var v13 = (byte)((data[2] >> 6) & 1);
                            if (((data[2] >> 7) & 1) != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            if (v13 != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            var v11 = (byte)(data[1] >> 7);
                            data[1] *= 2;
                            data[1] |= v11;
                            data[11] ^= 0xD3;
                            var v12 = (byte)((data[13] >> 7) & 1);
                            if (((data[13] >> 6) & 1) != 0)
                            {
                                data[13] |= 0x80;
                            }
                            else
                            {
                                data[13] &= 0x7F;
                            }

                            if (v12 != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            data[27] = data[27];
                        }
                        else
                        {
                            data[0] ^= 0x2A;
                            data[11] ^= 0xB;
                            var v8 = data[15];
                            data[15] = data[7];
                            data[7] = v8;
                            var v14 = (byte)((data[2] >> 6) & 1);
                            if (((data[2] >> 2) & 1) != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            if (v14 != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            data[12] ^= 0xF9;
                            var v9 = data[3];
                            data[3] = data[11];
                            data[11] = v9;
                            data[7] ^= 0x3C;
                            var v10 = (byte)(data[2] >> 3);
                            data[2] *= 32;
                            data[2] |= v10;
                        }
                    }
                    else
                    {
                        data[7] ^= 0x34;
                        var v7 = data[0];
                        data[0] = data[2];
                        data[2] = v7;
                        data[5] ^= 0xC1;
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v16 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 4) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v16 != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    data[1] ^= 0x89;
                    var v15 = (byte)((data[0] >> 5) & 1);
                    if (((data[0] >> 7) & 1) != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    if (v15 != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    var v3 = data[3];
                    data[3] = data[3];
                    data[3] = v3;
                    var v4 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v4;
                    var v5 = data[0];
                    data[0] = data[0];
                    data[0] = v5;
                    data[0] ^= 0xDA;
                    data[2] ^= 0x95;
                    var v6 = data[2];
                    data[2] = data[3];
                    data[3] = v6;
                }
            }
        }
    }
}