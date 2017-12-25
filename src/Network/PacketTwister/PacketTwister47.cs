// <copyright file="PacketTwister47.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister47 : IPacketTwister
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
                            var v17 = (byte)((data[14] >> 2) & 1);
                            if (((data[14] >> 1) & 1) != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[14] |= 2;
                            }
                            else
                            {
                                data[14] &= 0xFD;
                            }

                            var v11 = data[9];
                            data[9] = data[20];
                            data[20] = v11;
                            var v12 = data[24];
                            data[24] = data[29];
                            data[29] = v12;
                            var v13 = (byte)(data[17] >> 6);
                            data[17] *= 4;
                            data[17] |= v13;
                            var v14 = (byte)(data[2] >> 2);
                            data[2] <<= 6;
                            data[2] |= v14;
                            data[17] ^= 0x2A;
                            var v16 = (byte)((data[16] >> 2) & 1);
                            if (((data[16] >> 3) & 1) != 0)
                            {
                                data[16] |= 4;
                            }
                            else
                            {
                                data[16] &= 0xFB;
                            }

                            if (v16 != 0)
                            {
                                data[16] |= 8;
                            }
                            else
                            {
                                data[16] &= 0xF7;
                            }

                            var v15 = (byte)((data[6] >> 4) & 1);
                            if (((data[6] >> 3) & 1) != 0)
                            {
                                data[6] |= 0x10;
                            }
                            else
                            {
                                data[6] &= 0xEF;
                            }

                            if (v15 != 0)
                            {
                                data[6] |= 8;
                            }
                            else
                            {
                                data[6] &= 0xF7;
                            }
                        }
                        else
                        {
                            var v8 = data[10];
                            data[10] = data[1];
                            data[1] = v8;
                            var v9 = (byte)(data[10] >> 1);
                            data[10] <<= 7;
                            data[10] |= v9;
                            var v10 = data[6];
                            data[6] = data[5];
                            data[5] = v10;
                        }
                    }
                    else
                    {
                        var v19 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v19 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        var v18 = (byte)((data[5] >> 3) & 1);
                        if (((data[5] >> 2) & 1) != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        if (v18 != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        data[7] ^= 0xFA;
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    var v3 = (byte)(data[1] >> 5);
                    data[1] *= 8;
                    data[1] |= v3;
                    var v4 = data[0];
                    data[0] = data[1];
                    data[1] = v4;
                    var v21 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v21 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v5 = data[0];
                    data[0] = data[0];
                    data[0] = v5;
                    var v6 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v6;
                    var v7 = data[3];
                    data[3] = data[0];
                    data[0] = v7;
                    data[2] ^= 0x20;
                    var v20 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 7) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v20 != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
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
                            var v17 = (byte)((data[6] >> 4) & 1);
                            if (((data[6] >> 3) & 1) != 0)
                            {
                                data[6] |= 0x10;
                            }
                            else
                            {
                                data[6] &= 0xEF;
                            }

                            if (v17 != 0)
                            {
                                data[6] |= 8;
                            }
                            else
                            {
                                data[6] &= 0xF7;
                            }

                            var v16 = (byte)((data[16] >> 2) & 1);
                            if (((data[16] >> 3) & 1) != 0)
                            {
                                data[16] |= 4;
                            }
                            else
                            {
                                data[16] &= 0xFB;
                            }

                            if (v16 != 0)
                            {
                                data[16] |= 8;
                            }
                            else
                            {
                                data[16] &= 0xF7;
                            }

                            data[17] ^= 0x2A;
                            var v11 = (byte)(data[2] >> 6);
                            data[2] *= 4;
                            data[2] |= v11;
                            var v12 = (byte)(data[17] >> 2);
                            data[17] <<= 6;
                            data[17] |= v12;
                            var v13 = data[24];
                            data[24] = data[29];
                            data[29] = v13;
                            var v14 = data[9];
                            data[9] = data[20];
                            data[20] = v14;
                            var v15 = (byte)((data[14] >> 2) & 1);
                            if (((data[14] >> 1) & 1) != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            if (v15 != 0)
                            {
                                data[14] |= 2;
                            }
                            else
                            {
                                data[14] &= 0xFD;
                            }
                        }
                        else
                        {
                            var v8 = data[6];
                            data[6] = data[5];
                            data[5] = v8;
                            var v9 = (byte)(data[10] >> 7);
                            data[10] *= 2;
                            data[10] |= v9;
                            var v10 = data[10];
                            data[10] = data[1];
                            data[1] = v10;
                        }
                    }
                    else
                    {
                        data[7] ^= 0xFA;
                        var v19 = (byte)((data[5] >> 3) & 1);
                        if (((data[5] >> 2) & 1) != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        if (v19 != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        var v18 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v18 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 7) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    data[2] ^= 0x20;
                    var v3 = data[3];
                    data[3] = data[0];
                    data[0] = v3;
                    var v4 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v4;
                    var v5 = data[0];
                    data[0] = data[0];
                    data[0] = v5;
                    var v21 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v21 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v6 = data[0];
                    data[0] = data[1];
                    data[1] = v6;
                    var v7 = (byte)(data[1] >> 3);
                    data[1] *= 32;
                    data[1] |= v7;
                    var v20 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v20 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }
                }
            }
        }
    }
}