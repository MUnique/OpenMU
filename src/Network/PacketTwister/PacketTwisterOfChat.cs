// <copyright file="PacketTwisterOfChat.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'Chat' type.
    /// </summary>
    internal class PacketTwisterOfChat : IPacketTwister
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
                            data[13] ^= 0x26;
                            var v17 = data[5];
                            data[5] = data[3];
                            data[3] = v17;
                            data[5] ^= 0x91;
                            var v18 = (byte)(data[6] >> 4);
                            data[6] *= 16;
                            data[6] |= v18;
                            data[29] ^= 0x96;
                            var v19 = (byte)(data[7] >> 3);
                            data[7] *= 32;
                            data[7] |= v19;
                            data[15] ^= 0x61;
                            var v21 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 6) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v21 != 0)
                            {
                                data[3] |= 0x40;
                            }
                            else
                            {
                                data[3] &= 0xBF;
                            }

                            var v20 = (byte)((data[16] >> 3) & 1);
                            if (((data[16] >> 6) & 1) != 0)
                            {
                                data[16] |= 8;
                            }
                            else
                            {
                                data[16] &= 0xF7;
                            }

                            if (v20 != 0)
                            {
                                data[16] |= 0x40;
                            }
                            else
                            {
                                data[16] &= 0xBF;
                            }
                        }
                        else
                        {
                            var v12 = (byte)(data[10] >> 4);
                            data[10] *= 16;
                            data[10] |= v12;
                            data[7] ^= 0x5A;
                            var v23 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 1) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            var v13 = data[0];
                            data[0] = data[1];
                            data[1] = v13;
                            var v14 = data[7];
                            data[7] = data[0];
                            data[0] = v14;
                            var v15 = data[15];
                            data[15] = data[4];
                            data[4] = v15;
                            data[2] ^= 0x9E;
                            var v16 = (byte)(data[11] >> 1);
                            data[11] <<= 7;
                            data[11] |= v16;
                            var v22 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 1) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v22 != 0)
                            {
                                data[4] |= 2;
                            }
                            else
                            {
                                data[4] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        var v28 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 1) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v28 != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        data[0] ^= 0x14;
                        var v27 = (byte)((data[4] >> 4) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        if (v27 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        var v7 = data[2];
                        data[2] = data[1];
                        data[1] = v7;
                        var v26 = (byte)((data[4] >> 4) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        if (v26 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        var v8 = data[4];
                        data[4] = data[4];
                        data[4] = v8;
                        var v9 = (byte)(data[5] >> 1);
                        data[5] <<= 7;
                        data[5] |= v9;
                        data[3] ^= 0xFC;
                        var v10 = (byte)(data[3] >> 6);
                        data[3] *= 4;
                        data[3] |= v10;
                        var v11 = (byte)(data[1] >> 2);
                        data[1] <<= 6;
                        data[1] |= v11;
                        var v25 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 3) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v25 != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        var v24 = (byte)((data[3] >> 4) & 1);
                        if (((data[3] >> 3) & 1) != 0)
                        {
                            data[3] |= 0x10;
                        }
                        else
                        {
                            data[3] &= 0xEF;
                        }

                        if (v24 != 0)
                        {
                            data[3] |= 8;
                        }
                        else
                        {
                            data[3] &= 0xF7;
                        }

                        data[6] ^= 0x79;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[0];
                    data[0] = v3;
                    var v4 = data[2];
                    data[2] = data[3];
                    data[3] = v4;
                    var v5 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v5;
                    var v2 = (byte)((data[2] >> 3) & 1);
                    if (((data[2] >> 2) & 1) != 0)
                    {
                        data[2] |= 8;
                    }
                    else
                    {
                        data[2] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    var v32 = (byte)((data[1] >> 5) & 1);
                    if (((data[1] >> 7) & 1) != 0)
                    {
                        data[1] |= 0x20;
                    }
                    else
                    {
                        data[1] &= 0xDF;
                    }

                    if (v32 != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    var v31 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v31 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    var v6 = data[2];
                    data[2] = data[0];
                    data[0] = v6;
                    var v30 = (byte)((data[2] >> 7) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    if (v30 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    var v29 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 5) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v29 != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
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
                            var v21 = (byte)((data[16] >> 3) & 1);
                            if (((data[16] >> 6) & 1) != 0)
                            {
                                data[16] |= 8;
                            }
                            else
                            {
                                data[16] &= 0xF7;
                            }

                            if (v21 != 0)
                            {
                                data[16] |= 0x40;
                            }
                            else
                            {
                                data[16] &= 0xBF;
                            }

                            var v20 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 6) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[3] |= 0x40;
                            }
                            else
                            {
                                data[3] &= 0xBF;
                            }

                            data[15] ^= 0x61;
                            var v17 = (byte)(data[7] >> 5);
                            data[7] *= 8;
                            data[7] |= v17;
                            data[29] ^= 0x96;
                            var v18 = (byte)(data[6] >> 4);
                            data[6] *= 16;
                            data[6] |= v18;
                            data[5] ^= 0x91;
                            var v19 = data[5];
                            data[5] = data[3];
                            data[3] = v19;
                            data[13] ^= 0x26;
                        }
                        else
                        {
                            var v23 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 1) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v23 != 0)
                            {
                                data[4] |= 2;
                            }
                            else
                            {
                                data[4] &= 0xFD;
                            }

                            var v12 = (byte)(data[11] >> 7);
                            data[11] *= 2;
                            data[11] |= v12;
                            data[2] ^= 0x9E;
                            var v13 = data[15];
                            data[15] = data[4];
                            data[4] = v13;
                            var v14 = data[7];
                            data[7] = data[0];
                            data[0] = v14;
                            var v15 = data[0];
                            data[0] = data[1];
                            data[1] = v15;
                            var v22 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 1) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            data[7] ^= 0x5A;
                            var v16 = (byte)(data[10] >> 4);
                            data[10] *= 16;
                            data[10] |= v16;
                        }
                    }
                    else
                    {
                        data[6] ^= 0x79;
                        var v28 = (byte)((data[3] >> 4) & 1);
                        if (((data[3] >> 3) & 1) != 0)
                        {
                            data[3] |= 0x10;
                        }
                        else
                        {
                            data[3] &= 0xEF;
                        }

                        if (v28 != 0)
                        {
                            data[3] |= 8;
                        }
                        else
                        {
                            data[3] &= 0xF7;
                        }

                        var v27 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 3) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v27 != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        var v7 = (byte)(data[1] >> 6);
                        data[1] *= 4;
                        data[1] |= v7;
                        var v8 = (byte)(data[3] >> 2);
                        data[3] <<= 6;
                        data[3] |= v8;
                        data[3] ^= 0xFC;
                        var v9 = (byte)(data[5] >> 7);
                        data[5] *= 2;
                        data[5] |= v9;
                        var v10 = data[4];
                        data[4] = data[4];
                        data[4] = v10;
                        var v26 = (byte)((data[4] >> 4) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        if (v26 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        var v11 = data[2];
                        data[2] = data[1];
                        data[1] = v11;
                        var v25 = (byte)((data[4] >> 4) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        if (v25 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        data[0] ^= 0x14;
                        var v24 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 1) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v24 != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 5) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    var v32 = (byte)((data[2] >> 7) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    if (v32 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    var v3 = data[2];
                    data[2] = data[0];
                    data[0] = v3;
                    var v31 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v31 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    var v30 = (byte)((data[1] >> 5) & 1);
                    if (((data[1] >> 7) & 1) != 0)
                    {
                        data[1] |= 0x20;
                    }
                    else
                    {
                        data[1] &= 0xDF;
                    }

                    if (v30 != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    var v29 = (byte)((data[2] >> 3) & 1);
                    if (((data[2] >> 2) & 1) != 0)
                    {
                        data[2] |= 8;
                    }
                    else
                    {
                        data[2] &= 0xF7;
                    }

                    if (v29 != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    var v4 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v4;
                    var v5 = data[2];
                    data[2] = data[3];
                    data[3] = v5;
                    var v6 = data[1];
                    data[1] = data[0];
                    data[0] = v6;
                }
            }
        }
    }
}