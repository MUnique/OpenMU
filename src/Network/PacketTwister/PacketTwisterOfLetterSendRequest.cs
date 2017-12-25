// <copyright file="PacketTwisterOfLetterSendRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'LetterSendRequest' type.
    /// </summary>
    internal class PacketTwisterOfLetterSendRequest : IPacketTwister
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
                            var v16 = data[17];
                            data[17] = data[22];
                            data[22] = v16;
                            var v25 = (byte)((data[4] >> 2) & 1);
                            if (((data[4] >> 4) & 1) != 0)
                            {
                                data[4] |= 4;
                            }
                            else
                            {
                                data[4] &= 0xFB;
                            }

                            if (v25 != 0)
                            {
                                data[4] |= 0x10;
                            }
                            else
                            {
                                data[4] &= 0xEF;
                            }

                            var v24 = (byte)((data[12] >> 6) & 1);
                            if (((data[12] >> 6) & 1) != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            if (v24 != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            var v17 = data[23];
                            data[23] = data[9];
                            data[9] = v17;
                            var v23 = (byte)((data[25] >> 2) & 1);
                            if (((data[25] >> 5) & 1) != 0)
                            {
                                data[25] |= 4;
                            }
                            else
                            {
                                data[25] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[25] |= 0x20;
                            }
                            else
                            {
                                data[25] &= 0xDF;
                            }

                            data[22] ^= 0x1F;
                            data[21] ^= 0x75;
                            var v18 = data[23];
                            data[23] = data[22];
                            data[22] = v18;
                            var v19 = (byte)(data[22] >> 6);
                            data[22] *= 4;
                            data[22] |= v19;
                            var v22 = (byte)((data[4] >> 6) & 1);
                            if (((data[4] >> 7) & 1) != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            if (v22 != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            var v20 = data[13];
                            data[13] = data[30];
                            data[30] = v20;
                            var v21 = data[22];
                            data[22] = data[11];
                            data[11] = v21;
                        }
                        else
                        {
                            var v10 = data[13];
                            data[13] = data[8];
                            data[8] = v10;
                            var v11 = (byte)(data[13] >> 7);
                            data[13] *= 2;
                            data[13] |= v11;
                            var v12 = data[3];
                            data[3] = data[7];
                            data[7] = v12;
                            var v26 = (byte)((data[1] >> 6) & 1);
                            if (((data[1] >> 4) & 1) != 0)
                            {
                                data[1] |= 0x40;
                            }
                            else
                            {
                                data[1] &= 0xBF;
                            }

                            if (v26 != 0)
                            {
                                data[1] |= 0x10;
                            }
                            else
                            {
                                data[1] &= 0xEF;
                            }

                            var v13 = (byte)(data[9] >> 7);
                            data[9] *= 2;
                            data[9] |= v13;
                            var v14 = data[3];
                            data[3] = data[4];
                            data[4] = v14;
                            var v15 = (byte)(data[8] >> 6);
                            data[8] *= 4;
                            data[8] |= v15;
                            data[12] ^= 0xC3;
                            data[12] ^= 0x8B;
                        }
                    }
                    else
                    {
                        var v8 = (byte)(data[6] >> 5);
                        data[6] *= 8;
                        data[6] |= v8;
                        var v9 = data[6];
                        data[6] = data[5];
                        data[5] = v9;
                        data[7] ^= 0xDA;
                    }
                }
                else
                {
                    var v2 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 3) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    var v3 = (byte)(data[0] >> 7);
                    data[0] *= 2;
                    data[0] |= v3;
                    var v4 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v4;
                    var v5 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v5;
                    var v27 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v27 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v6 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v6;
                    var v7 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v7;
                    data[3] ^= 0xB4;
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
                            var v16 = data[22];
                            data[22] = data[11];
                            data[11] = v16;
                            var v17 = data[13];
                            data[13] = data[30];
                            data[30] = v17;
                            var v25 = (byte)((data[4] >> 6) & 1);
                            if (((data[4] >> 7) & 1) != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            if (v25 != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            var v18 = (byte)(data[22] >> 2);
                            data[22] <<= 6;
                            data[22] |= v18;
                            var v19 = data[23];
                            data[23] = data[22];
                            data[22] = v19;
                            data[21] ^= 0x75;
                            data[22] ^= 0x1F;
                            var v24 = (byte)((data[25] >> 2) & 1);
                            if (((data[25] >> 5) & 1) != 0)
                            {
                                data[25] |= 4;
                            }
                            else
                            {
                                data[25] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[25] |= 0x20;
                            }
                            else
                            {
                                data[25] &= 0xDF;
                            }

                            var v20 = data[23];
                            data[23] = data[9];
                            data[9] = v20;
                            var v23 = (byte)((data[12] >> 6) & 1);
                            if (((data[12] >> 6) & 1) != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            if (v23 != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            var v22 = (byte)((data[4] >> 2) & 1);
                            if (((data[4] >> 4) & 1) != 0)
                            {
                                data[4] |= 4;
                            }
                            else
                            {
                                data[4] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[4] |= 0x10;
                            }
                            else
                            {
                                data[4] &= 0xEF;
                            }

                            var v21 = data[17];
                            data[17] = data[22];
                            data[22] = v21;
                        }
                        else
                        {
                            data[12] ^= 0x8B;
                            data[12] ^= 0xC3;
                            var v10 = (byte)(data[8] >> 2);
                            data[8] <<= 6;
                            data[8] |= v10;
                            var v11 = data[3];
                            data[3] = data[4];
                            data[4] = v11;
                            var v12 = (byte)(data[9] >> 1);
                            data[9] <<= 7;
                            data[9] |= v12;
                            var v26 = (byte)((data[1] >> 6) & 1);
                            if (((data[1] >> 4) & 1) != 0)
                            {
                                data[1] |= 0x40;
                            }
                            else
                            {
                                data[1] &= 0xBF;
                            }

                            if (v26 != 0)
                            {
                                data[1] |= 0x10;
                            }
                            else
                            {
                                data[1] &= 0xEF;
                            }

                            var v13 = data[3];
                            data[3] = data[7];
                            data[7] = v13;
                            var v14 = (byte)(data[13] >> 1);
                            data[13] <<= 7;
                            data[13] |= v14;
                            var v15 = data[13];
                            data[13] = data[8];
                            data[8] = v15;
                        }
                    }
                    else
                    {
                        data[7] ^= 0xDA;
                        var v8 = data[6];
                        data[6] = data[5];
                        data[5] = v8;
                        var v9 = (byte)(data[6] >> 3);
                        data[6] *= 32;
                        data[6] |= v9;
                    }
                }
                else
                {
                    data[3] ^= 0xB4;
                    var v3 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v3;
                    var v4 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v4;
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

                    var v5 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v5;
                    var v6 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v6;
                    var v7 = (byte)(data[0] >> 1);
                    data[0] <<= 7;
                    data[0] |= v7;
                    var v27 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 3) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v27 != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }
                }
            }
        }
    }
}