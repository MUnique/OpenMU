// <copyright file="PacketTwisterOfWhisper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'Whisper' type.
    /// </summary>
    internal class PacketTwisterOfWhisper : IPacketTwister
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
                            var v18 = (byte)(data[27] >> 1);
                            data[27] <<= 7;
                            data[27] |= v18;
                            var v19 = (byte)(data[1] >> 6);
                            data[1] *= 4;
                            data[1] |= v19;
                            data[13] ^= 0x35;
                            var v20 = data[30];
                            data[30] = data[9];
                            data[9] = v20;
                            var v25 = (byte)((data[1] >> 4) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 0x10;
                            }
                            else
                            {
                                data[1] &= 0xEF;
                            }

                            if (v25 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            var v21 = (byte)(data[13] >> 5);
                            data[13] *= 8;
                            data[13] |= v21;
                            var v22 = data[15];
                            data[15] = data[25];
                            data[25] = v22;
                            var v23 = data[13];
                            data[13] = data[18];
                            data[18] = v23;
                            data[27] ^= 0x11;
                            var v24 = (byte)(data[27] >> 2);
                            data[27] <<= 6;
                            data[27] |= v24;
                        }
                        else
                        {
                            data[3] ^= 0x32;
                            var v10 = data[7];
                            data[7] = data[7];
                            data[7] = v10;
                            var v11 = data[14];
                            data[14] = data[3];
                            data[3] = v11;
                            var v12 = (byte)(data[9] >> 1);
                            data[9] <<= 7;
                            data[9] |= v12;
                            var v13 = (byte)(data[0] >> 5);
                            data[0] *= 8;
                            data[0] |= v13;
                            var v26 = (byte)((data[5] >> 2) & 1);
                            if (((data[5] >> 4) & 1) != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            if (v26 != 0)
                            {
                                data[5] |= 0x10;
                            }
                            else
                            {
                                data[5] &= 0xEF;
                            }

                            var v14 = (byte)(data[10] >> 1);
                            data[10] <<= 7;
                            data[10] |= v14;
                            data[5] ^= 0x84;
                            var v15 = data[0];
                            data[0] = data[9];
                            data[9] = v15;
                            data[6] = data[6];
                            var v16 = data[10];
                            data[10] = data[2];
                            data[2] = v16;
                            var v17 = data[2];
                            data[2] = data[7];
                            data[7] = v17;
                        }
                    }
                    else
                    {
                        var v30 = (byte)((data[5] >> 2) & 1);
                        if (((data[5] >> 2) & 1) != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        if (v30 != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        data[1] ^= 0xFB;
                        data[0] ^= 0xAB;
                        data[4] ^= 0x78;
                        var v6 = (byte)(data[2] >> 3);
                        data[2] *= 32;
                        data[2] |= v6;
                        var v29 = (byte)((data[7] >> 7) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        if (v29 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        var v28 = (byte)((data[2] >> 5) & 1);
                        if (((data[2] >> 1) & 1) != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        if (v28 != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        var v7 = data[6];
                        data[6] = data[1];
                        data[1] = v7;
                        data[3] ^= 0xE7;
                        var v8 = (byte)(data[5] >> 7);
                        data[5] *= 2;
                        data[5] |= v8;
                        var v27 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v27 != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        var v9 = data[0];
                        data[0] = data[5];
                        data[5] = v9;
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[2];
                    data[2] = v3;
                    var v2 = (byte)((data[2] >> 4) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v32 = (byte)((data[2] >> 3) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 8;
                    }
                    else
                    {
                        data[2] &= 0xF7;
                    }

                    if (v32 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    var v4 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v4;
                    data[1] ^= 0x3E;
                    var v31 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v31 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v5 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v5;
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
                            var v18 = (byte)(data[27] >> 6);
                            data[27] *= 4;
                            data[27] |= v18;
                            data[27] ^= 0x11;
                            var v19 = data[13];
                            data[13] = data[18];
                            data[18] = v19;
                            var v20 = data[15];
                            data[15] = data[25];
                            data[25] = v20;
                            var v21 = (byte)(data[13] >> 3);
                            data[13] *= 32;
                            data[13] |= v21;
                            var v25 = (byte)((data[1] >> 4) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 0x10;
                            }
                            else
                            {
                                data[1] &= 0xEF;
                            }

                            if (v25 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            var v22 = data[30];
                            data[30] = data[9];
                            data[9] = v22;
                            data[13] ^= 0x35;
                            var v23 = (byte)(data[1] >> 2);
                            data[1] <<= 6;
                            data[1] |= v23;
                            var v24 = (byte)(data[27] >> 7);
                            data[27] *= 2;
                            data[27] |= v24;
                        }
                        else
                        {
                            var v10 = data[2];
                            data[2] = data[7];
                            data[7] = v10;
                            var v11 = data[10];
                            data[10] = data[2];
                            data[2] = v11;
                            data[6] = data[6];
                            var v12 = data[0];
                            data[0] = data[9];
                            data[9] = v12;
                            data[5] ^= 0x84;
                            var v13 = (byte)(data[10] >> 7);
                            data[10] *= 2;
                            data[10] |= v13;
                            var v26 = (byte)((data[5] >> 2) & 1);
                            if (((data[5] >> 4) & 1) != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            if (v26 != 0)
                            {
                                data[5] |= 0x10;
                            }
                            else
                            {
                                data[5] &= 0xEF;
                            }

                            var v14 = (byte)(data[0] >> 3);
                            data[0] *= 32;
                            data[0] |= v14;
                            var v15 = (byte)(data[9] >> 7);
                            data[9] *= 2;
                            data[9] |= v15;
                            var v16 = data[14];
                            data[14] = data[3];
                            data[3] = v16;
                            var v17 = data[7];
                            data[7] = data[7];
                            data[7] = v17;
                            data[3] ^= 0x32;
                        }
                    }
                    else
                    {
                        var v6 = data[0];
                        data[0] = data[5];
                        data[5] = v6;
                        var v30 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v30 != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        var v7 = (byte)(data[5] >> 1);
                        data[5] <<= 7;
                        data[5] |= v7;
                        data[3] ^= 0xE7;
                        var v8 = data[6];
                        data[6] = data[1];
                        data[1] = v8;
                        var v29 = (byte)((data[2] >> 5) & 1);
                        if (((data[2] >> 1) & 1) != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        if (v29 != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        var v28 = (byte)((data[7] >> 7) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        if (v28 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        var v9 = (byte)(data[2] >> 5);
                        data[2] *= 8;
                        data[2] |= v9;
                        data[4] ^= 0x78;
                        data[0] ^= 0xAB;
                        data[1] ^= 0xFB;
                        var v27 = (byte)((data[5] >> 2) & 1);
                        if (((data[5] >> 2) & 1) != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        if (v27 != 0)
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
                    var v3 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v3;
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

                    data[1] ^= 0x3E;
                    var v4 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v4;
                    var v32 = (byte)((data[2] >> 3) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 8;
                    }
                    else
                    {
                        data[2] &= 0xF7;
                    }

                    if (v32 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    var v31 = (byte)((data[2] >> 4) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    if (v31 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v5 = data[3];
                    data[3] = data[2];
                    data[2] = v5;
                }
            }
        }
    }
}