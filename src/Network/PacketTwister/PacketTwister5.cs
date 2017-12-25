// <copyright file="PacketTwister5.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister5 : IPacketTwister
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
                            var v12 = (byte)(data[13] >> 7);
                            data[13] *= 2;
                            data[13] |= v12;
                            var v13 = (byte)(data[8] >> 7);
                            data[8] *= 2;
                            data[8] |= v13;
                            var v14 = data[0];
                            data[0] = data[30];
                            data[30] = v14;
                            var v21 = (byte)((data[12] >> 3) & 1);
                            if (((data[12] >> 1) & 1) != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            if (v21 != 0)
                            {
                                data[12] |= 2;
                            }
                            else
                            {
                                data[12] &= 0xFD;
                            }

                            var v20 = (byte)((data[24] >> 3) & 1);
                            if (((data[24] >> 6) & 1) != 0)
                            {
                                data[24] |= 8;
                            }
                            else
                            {
                                data[24] &= 0xF7;
                            }

                            if (v20 != 0)
                            {
                                data[24] |= 0x40;
                            }
                            else
                            {
                                data[24] &= 0xBF;
                            }

                            var v15 = (byte)(data[20] >> 4);
                            data[20] *= 16;
                            data[20] |= v15;
                            var v19 = (byte)((data[22] >> 5) & 1);
                            if (((data[22] >> 1) & 1) != 0)
                            {
                                data[22] |= 0x20;
                            }
                            else
                            {
                                data[22] &= 0xDF;
                            }

                            if (v19 != 0)
                            {
                                data[22] |= 2;
                            }
                            else
                            {
                                data[22] &= 0xFD;
                            }

                            var v16 = data[22];
                            data[22] = data[17];
                            data[17] = v16;
                            data[5] ^= 0xF9;
                            var v17 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v17;
                            var v18 = (byte)(data[29] >> 1);
                            data[29] <<= 7;
                            data[29] |= v18;
                        }
                        else
                        {
                            var v22 = (byte)((data[13] >> 1) & 1);
                            if (((data[13] >> 3) & 1) != 0)
                            {
                                data[13] |= 2;
                            }
                            else
                            {
                                data[13] &= 0xFD;
                            }

                            if (v22 != 0)
                            {
                                data[13] |= 8;
                            }
                            else
                            {
                                data[13] &= 0xF7;
                            }

                            var v10 = data[14];
                            data[14] = data[12];
                            data[12] = v10;
                            var v11 = (byte)(data[5] >> 2);
                            data[5] <<= 6;
                            data[5] |= v11;
                        }
                    }
                    else
                    {
                        data[1] ^= 0xA;
                        data[4] ^= 0xA0;
                        var v5 = data[2];
                        data[2] = data[3];
                        data[3] = v5;
                        var v2 = (byte)((data[2] >> 1) & 1);
                        if (((data[2] >> 5) & 1) != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        if (v2 != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        var v6 = data[1];
                        data[1] = data[7];
                        data[7] = v6;
                        var v7 = data[1];
                        data[1] = data[4];
                        data[4] = v7;
                        var v8 = (byte)(data[3] >> 7);
                        data[3] *= 2;
                        data[3] |= v8;
                        var v23 = (byte)((data[3] >> 2) & 1);
                        if (((data[3] >> 2) & 1) != 0)
                        {
                            data[3] |= 4;
                        }
                        else
                        {
                            data[3] &= 0xFB;
                        }

                        if (v23 != 0)
                        {
                            data[3] |= 4;
                        }
                        else
                        {
                            data[3] &= 0xFB;
                        }

                        var v9 = (byte)(data[5] >> 2);
                        data[5] <<= 6;
                        data[5] |= v9;
                    }
                }
                else
                {
                    data[1] ^= 0x51;
                    var v3 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v3;
                    data[2] ^= 0xED;
                    var v4 = (byte)(data[0] >> 4);
                    data[0] *= 16;
                    data[0] |= v4;
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
                            var v12 = (byte)(data[29] >> 7);
                            data[29] *= 2;
                            data[29] |= v12;
                            var v13 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v13;
                            data[5] ^= 0xF9;
                            var v14 = data[22];
                            data[22] = data[17];
                            data[17] = v14;
                            var v21 = (byte)((data[22] >> 5) & 1);
                            if (((data[22] >> 1) & 1) != 0)
                            {
                                data[22] |= 0x20;
                            }
                            else
                            {
                                data[22] &= 0xDF;
                            }

                            if (v21 != 0)
                            {
                                data[22] |= 2;
                            }
                            else
                            {
                                data[22] &= 0xFD;
                            }

                            var v15 = (byte)(data[20] >> 4);
                            data[20] *= 16;
                            data[20] |= v15;
                            var v20 = (byte)((data[24] >> 3) & 1);
                            if (((data[24] >> 6) & 1) != 0)
                            {
                                data[24] |= 8;
                            }
                            else
                            {
                                data[24] &= 0xF7;
                            }

                            if (v20 != 0)
                            {
                                data[24] |= 0x40;
                            }
                            else
                            {
                                data[24] &= 0xBF;
                            }

                            var v19 = (byte)((data[12] >> 3) & 1);
                            if (((data[12] >> 1) & 1) != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            if (v19 != 0)
                            {
                                data[12] |= 2;
                            }
                            else
                            {
                                data[12] &= 0xFD;
                            }

                            var v16 = data[0];
                            data[0] = data[30];
                            data[30] = v16;
                            var v17 = (byte)(data[8] >> 1);
                            data[8] <<= 7;
                            data[8] |= v17;
                            var v18 = (byte)(data[13] >> 1);
                            data[13] <<= 7;
                            data[13] |= v18;
                        }
                        else
                        {
                            var v10 = (byte)(data[5] >> 6);
                            data[5] *= 4;
                            data[5] |= v10;
                            var v11 = data[14];
                            data[14] = data[12];
                            data[12] = v11;
                            var v22 = (byte)((data[13] >> 1) & 1);
                            if (((data[13] >> 3) & 1) != 0)
                            {
                                data[13] |= 2;
                            }
                            else
                            {
                                data[13] &= 0xFD;
                            }

                            if (v22 != 0)
                            {
                                data[13] |= 8;
                            }
                            else
                            {
                                data[13] &= 0xF7;
                            }
                        }
                    }
                    else
                    {
                        var v5 = (byte)(data[5] >> 6);
                        data[5] *= 4;
                        data[5] |= v5;
                        var v2 = (byte)((data[3] >> 2) & 1);
                        if (((data[3] >> 2) & 1) != 0)
                        {
                            data[3] |= 4;
                        }
                        else
                        {
                            data[3] &= 0xFB;
                        }

                        if (v2 != 0)
                        {
                            data[3] |= 4;
                        }
                        else
                        {
                            data[3] &= 0xFB;
                        }

                        var v6 = (byte)(data[3] >> 1);
                        data[3] <<= 7;
                        data[3] |= v6;
                        var v7 = data[1];
                        data[1] = data[4];
                        data[4] = v7;
                        var v8 = data[1];
                        data[1] = data[7];
                        data[7] = v8;
                        var v23 = (byte)((data[2] >> 1) & 1);
                        if (((data[2] >> 5) & 1) != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        if (v23 != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        var v9 = data[2];
                        data[2] = data[3];
                        data[3] = v9;
                        data[4] ^= 0xA0;
                        data[1] ^= 0xA;
                    }
                }
                else
                {
                    var v3 = (byte)(data[0] >> 4);
                    data[0] *= 16;
                    data[0] |= v3;
                    data[2] ^= 0xED;
                    var v4 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v4;
                    data[1] ^= 0x51;
                }
            }
        }
    }
}