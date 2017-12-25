// <copyright file="PacketTwisterOfLetterDeleteRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'LetterDeleteRequest' type.
    /// </summary>
    internal class PacketTwisterOfLetterDeleteRequest : IPacketTwister
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
                            var v12 = (byte)(data[6] >> 1);
                            data[6] <<= 7;
                            data[6] |= v12;
                            var v14 = (byte)((data[27] >> 1) & 1);
                            if (((data[27] >> 6) & 1) != 0)
                            {
                                data[27] |= 2;
                            }
                            else
                            {
                                data[27] &= 0xFD;
                            }

                            if (v14 != 0)
                            {
                                data[27] |= 0x40;
                            }
                            else
                            {
                                data[27] &= 0xBF;
                            }

                            var v13 = (byte)((data[27] >> 4) & 1);
                            if (((data[27] >> 6) & 1) != 0)
                            {
                                data[27] |= 0x10;
                            }
                            else
                            {
                                data[27] &= 0xEF;
                            }

                            if (v13 != 0)
                            {
                                data[27] |= 0x40;
                            }
                            else
                            {
                                data[27] &= 0xBF;
                            }

                            data[31] ^= 0xC7;
                            data[25] ^= 0x45;
                        }
                        else
                        {
                            var v20 = (byte)((data[14] >> 6) & 1);
                            if (((data[14] >> 1) & 1) != 0)
                            {
                                data[14] |= 0x40;
                            }
                            else
                            {
                                data[14] &= 0xBF;
                            }

                            if (v20 != 0)
                            {
                                data[14] |= 2;
                            }
                            else
                            {
                                data[14] &= 0xFD;
                            }

                            var v19 = (byte)((data[11] >> 3) & 1);
                            if (((data[11] >> 5) & 1) != 0)
                            {
                                data[11] |= 8;
                            }
                            else
                            {
                                data[11] &= 0xF7;
                            }

                            if (v19 != 0)
                            {
                                data[11] |= 0x20;
                            }
                            else
                            {
                                data[11] &= 0xDF;
                            }

                            var v7 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v7;
                            var v8 = data[4];
                            data[4] = data[14];
                            data[14] = v8;
                            data[3] ^= 0xAD;
                            var v9 = data[10];
                            data[10] = data[13];
                            data[13] = v9;
                            var v18 = (byte)((data[5] >> 2) & 1);
                            if (((data[5] >> 3) & 1) != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[5] |= 8;
                            }
                            else
                            {
                                data[5] &= 0xF7;
                            }

                            var v17 = (byte)((data[11] >> 2) & 1);
                            if (((data[11] >> 4) & 1) != 0)
                            {
                                data[11] |= 4;
                            }
                            else
                            {
                                data[11] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            var v10 = data[9];
                            data[9] = data[6];
                            data[6] = v10;
                            var v16 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 5) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v16 != 0)
                            {
                                data[4] |= 0x20;
                            }
                            else
                            {
                                data[4] &= 0xDF;
                            }

                            var v11 = data[6];
                            data[6] = data[9];
                            data[9] = v11;
                            var v15 = (byte)((data[3] >> 6) & 1);
                            if (((data[3] >> 1) & 1) != 0)
                            {
                                data[3] |= 0x40;
                            }
                            else
                            {
                                data[3] &= 0xBF;
                            }

                            if (v15 != 0)
                            {
                                data[3] |= 2;
                            }
                            else
                            {
                                data[3] &= 0xFD;
                            }

                            data[10] ^= 3;
                        }
                    }
                    else
                    {
                        var v24 = (byte)((data[5] >> 1) & 1);
                        if (((data[5] >> 6) & 1) != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        if (v24 != 0)
                        {
                            data[5] |= 0x40;
                        }
                        else
                        {
                            data[5] &= 0xBF;
                        }

                        var v23 = (byte)((data[5] >> 6) & 1);
                        if (((data[5] >> 3) & 1) != 0)
                        {
                            data[5] |= 0x40;
                        }
                        else
                        {
                            data[5] &= 0xBF;
                        }

                        if (v23 != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        var v4 = (byte)(data[6] >> 7);
                        data[6] *= 2;
                        data[6] |= v4;
                        var v22 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 7) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v22 != 0)
                        {
                            data[5] |= 0x80;
                        }
                        else
                        {
                            data[5] &= 0x7F;
                        }

                        data[5] ^= 0x45;
                        data[0] ^= 0xF3;
                        var v5 = data[1];
                        data[1] = data[5];
                        data[5] = v5;
                        var v21 = (byte)((data[4] >> 5) & 1);
                        if (((data[4] >> 4) & 1) != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        if (v21 != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        var v6 = (byte)(data[6] >> 6);
                        data[6] *= 4;
                        data[6] |= v6;
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v3 = data[1];
                    data[1] = data[0];
                    data[0] = v3;
                    data[2] ^= 3;
                    var v26 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 2) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v26 != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    var v25 = (byte)((data[3] >> 7) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
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
                            data[25] ^= 0x45;
                            data[31] ^= 0xC7;
                            var v14 = (byte)((data[27] >> 4) & 1);
                            if (((data[27] >> 6) & 1) != 0)
                            {
                                data[27] |= 0x10;
                            }
                            else
                            {
                                data[27] &= 0xEF;
                            }

                            if (v14 != 0)
                            {
                                data[27] |= 0x40;
                            }
                            else
                            {
                                data[27] &= 0xBF;
                            }

                            var v13 = (byte)((data[27] >> 1) & 1);
                            if (((data[27] >> 6) & 1) != 0)
                            {
                                data[27] |= 2;
                            }
                            else
                            {
                                data[27] &= 0xFD;
                            }

                            if (v13 != 0)
                            {
                                data[27] |= 0x40;
                            }
                            else
                            {
                                data[27] &= 0xBF;
                            }

                            var v12 = (byte)(data[6] >> 7);
                            data[6] *= 2;
                            data[6] |= v12;
                        }
                        else
                        {
                            data[10] ^= 3;
                            var v20 = (byte)((data[3] >> 6) & 1);
                            if (((data[3] >> 1) & 1) != 0)
                            {
                                data[3] |= 0x40;
                            }
                            else
                            {
                                data[3] &= 0xBF;
                            }

                            if (v20 != 0)
                            {
                                data[3] |= 2;
                            }
                            else
                            {
                                data[3] &= 0xFD;
                            }

                            var v7 = data[6];
                            data[6] = data[9];
                            data[9] = v7;
                            var v19 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 5) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v19 != 0)
                            {
                                data[4] |= 0x20;
                            }
                            else
                            {
                                data[4] &= 0xDF;
                            }

                            var v8 = data[9];
                            data[9] = data[6];
                            data[6] = v8;
                            var v18 = (byte)((data[11] >> 2) & 1);
                            if (((data[11] >> 4) & 1) != 0)
                            {
                                data[11] |= 4;
                            }
                            else
                            {
                                data[11] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            var v17 = (byte)((data[5] >> 2) & 1);
                            if (((data[5] >> 3) & 1) != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[5] |= 8;
                            }
                            else
                            {
                                data[5] &= 0xF7;
                            }

                            var v9 = data[10];
                            data[10] = data[13];
                            data[13] = v9;
                            data[3] ^= 0xAD;
                            var v10 = data[4];
                            data[4] = data[14];
                            data[14] = v10;
                            var v11 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v11;
                            var v16 = (byte)((data[11] >> 3) & 1);
                            if (((data[11] >> 5) & 1) != 0)
                            {
                                data[11] |= 8;
                            }
                            else
                            {
                                data[11] &= 0xF7;
                            }

                            if (v16 != 0)
                            {
                                data[11] |= 0x20;
                            }
                            else
                            {
                                data[11] &= 0xDF;
                            }

                            var v15 = (byte)((data[14] >> 6) & 1);
                            if (((data[14] >> 1) & 1) != 0)
                            {
                                data[14] |= 0x40;
                            }
                            else
                            {
                                data[14] &= 0xBF;
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
                    }
                    else
                    {
                        var v4 = (byte)(data[6] >> 2);
                        data[6] <<= 6;
                        data[6] |= v4;
                        var v24 = (byte)((data[4] >> 5) & 1);
                        if (((data[4] >> 4) & 1) != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        if (v24 != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        var v5 = data[1];
                        data[1] = data[5];
                        data[5] = v5;
                        data[0] ^= 0xF3;
                        data[5] ^= 0x45;
                        var v23 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 7) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v23 != 0)
                        {
                            data[5] |= 0x80;
                        }
                        else
                        {
                            data[5] &= 0x7F;
                        }

                        var v6 = (byte)(data[6] >> 1);
                        data[6] <<= 7;
                        data[6] |= v6;
                        var v22 = (byte)((data[5] >> 6) & 1);
                        if (((data[5] >> 3) & 1) != 0)
                        {
                            data[5] |= 0x40;
                        }
                        else
                        {
                            data[5] &= 0xBF;
                        }

                        if (v22 != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        var v21 = (byte)((data[5] >> 1) & 1);
                        if (((data[5] >> 6) & 1) != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        if (v21 != 0)
                        {
                            data[5] |= 0x40;
                        }
                        else
                        {
                            data[5] &= 0xBF;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 7) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v26 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 2) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v26 != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    data[2] ^= 3;
                    var v3 = data[1];
                    data[1] = data[0];
                    data[0] = v3;
                    var v25 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
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
        }
    }
}