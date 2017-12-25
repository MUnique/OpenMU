// <copyright file="PacketTwisterOfNpcDialogClose.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'NpcDialogClose' type.
    /// </summary>
    internal class PacketTwisterOfNpcDialogClose : IPacketTwister
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
                            var v16 = data[12];
                            data[12] = data[27];
                            data[27] = v16;
                            var v17 = data[25];
                            data[25] = data[29];
                            data[29] = v17;
                            var v18 = (byte)((data[20] >> 2) & 1);
                            if (((data[20] >> 2) & 1) != 0)
                            {
                                data[20] |= 4;
                            }
                            else
                            {
                                data[20] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[20] |= 4;
                            }
                            else
                            {
                                data[20] &= 0xFB;
                            }

                            data[4] ^= 0x50;
                        }
                        else
                        {
                            var v12 = data[8];
                            data[8] = data[14];
                            data[14] = v12;
                            data[5] ^= 0xE7;
                            data[11] ^= 0x5E;
                            var v19 = (byte)((data[12] >> 3) & 1);
                            if (((data[12] >> 4) & 1) != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            if (v19 != 0)
                            {
                                data[12] |= 0x10;
                            }
                            else
                            {
                                data[12] &= 0xEF;
                            }

                            var v13 = data[10];
                            data[10] = data[1];
                            data[1] = v13;
                            var v14 = (byte)(data[2] >> 2);
                            data[2] <<= 6;
                            data[2] |= v14;
                            var v15 = (byte)(data[12] >> 3);
                            data[12] *= 32;
                            data[12] |= v15;
                        }
                    }
                    else
                    {
                        data[5] ^= 4;
                        var v7 = data[7];
                        data[7] = data[0];
                        data[0] = v7;
                        data[4] ^= 0x13;
                        var v8 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v8;
                        data[1] ^= 0x6B;
                        var v21 = (byte)((data[5] >> 2) & 1);
                        if (((data[5] >> 5) & 1) != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        if (v21 != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        data[5] ^= 0xDF;
                        var v9 = (byte)(data[4] >> 6);
                        data[4] *= 4;
                        data[4] |= v9;
                        var v10 = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= v10;
                        var v20 = (byte)((data[3] >> 6) & 1);
                        if (((data[3] >> 3) & 1) != 0)
                        {
                            data[3] |= 0x40;
                        }
                        else
                        {
                            data[3] &= 0xBF;
                        }

                        if (v20 != 0)
                        {
                            data[3] |= 8;
                        }
                        else
                        {
                            data[3] &= 0xF7;
                        }

                        var v11 = (byte)(data[4] >> 3);
                        data[4] *= 32;
                        data[4] |= v11;
                    }
                }
                else
                {
                    var v3 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v3;
                    var v4 = data[3];
                    data[3] = data[0];
                    data[0] = v4;
                    data[0] ^= 0xE3;
                    var v5 = data[1];
                    data[1] = data[2];
                    data[2] = v5;
                    var v6 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v6;
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
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
                            data[4] ^= 0x50;
                            var v18 = (byte)((data[20] >> 2) & 1);
                            if (((data[20] >> 2) & 1) != 0)
                            {
                                data[20] |= 4;
                            }
                            else
                            {
                                data[20] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[20] |= 4;
                            }
                            else
                            {
                                data[20] &= 0xFB;
                            }

                            var v16 = data[25];
                            data[25] = data[29];
                            data[29] = v16;
                            var v17 = data[12];
                            data[12] = data[27];
                            data[27] = v17;
                        }
                        else
                        {
                            var v12 = (byte)(data[12] >> 5);
                            data[12] *= 8;
                            data[12] |= v12;
                            var v13 = (byte)(data[2] >> 6);
                            data[2] *= 4;
                            data[2] |= v13;
                            var v14 = data[10];
                            data[10] = data[1];
                            data[1] = v14;
                            var v19 = (byte)((data[12] >> 3) & 1);
                            if (((data[12] >> 4) & 1) != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            if (v19 != 0)
                            {
                                data[12] |= 0x10;
                            }
                            else
                            {
                                data[12] &= 0xEF;
                            }

                            data[11] ^= 0x5E;
                            data[5] ^= 0xE7;
                            var v15 = data[8];
                            data[8] = data[14];
                            data[14] = v15;
                        }
                    }
                    else
                    {
                        var v7 = (byte)(data[4] >> 5);
                        data[4] *= 8;
                        data[4] |= v7;
                        var v21 = (byte)((data[3] >> 6) & 1);
                        if (((data[3] >> 3) & 1) != 0)
                        {
                            data[3] |= 0x40;
                        }
                        else
                        {
                            data[3] &= 0xBF;
                        }

                        if (v21 != 0)
                        {
                            data[3] |= 8;
                        }
                        else
                        {
                            data[3] &= 0xF7;
                        }

                        var v8 = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= v8;
                        var v9 = (byte)(data[4] >> 2);
                        data[4] <<= 6;
                        data[4] |= v9;
                        data[5] ^= 0xDF;
                        var v20 = (byte)((data[5] >> 2) & 1);
                        if (((data[5] >> 5) & 1) != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        if (v20 != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        data[1] ^= 0x6B;
                        var v10 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v10;
                        data[4] ^= 0x13;
                        var v11 = data[7];
                        data[7] = data[0];
                        data[0] = v11;
                        data[5] ^= 4;
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v3 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v3;
                    var v4 = data[1];
                    data[1] = data[2];
                    data[2] = v4;
                    data[0] ^= 0xE3;
                    var v5 = data[3];
                    data[3] = data[0];
                    data[0] = v5;
                    var v6 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v6;
                }
            }
        }
    }
}