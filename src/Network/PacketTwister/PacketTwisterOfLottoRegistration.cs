// <copyright file="PacketTwisterOfLottoRegistration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'LottoRegistration' type.
    /// </summary>
    internal class PacketTwisterOfLottoRegistration : IPacketTwister
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
                            data[0] ^= 0x26;
                            var v21 = data[7];
                            data[7] = data[15];
                            data[15] = v21;
                            var v22 = data[29];
                            data[29] = data[15];
                            data[15] = v22;
                        }
                        else
                        {
                            data[11] ^= 0xF8;
                            var v15 = (byte)(data[2] >> 5);
                            data[2] *= 8;
                            data[2] |= v15;
                            var v23 = (byte)((data[7] >> 2) & 1);
                            if (((data[7] >> 1) & 1) != 0)
                            {
                                data[7] |= 4;
                            }
                            else
                            {
                                data[7] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[7] |= 2;
                            }
                            else
                            {
                                data[7] &= 0xFD;
                            }

                            var v16 = (byte)(data[7] >> 5);
                            data[7] *= 8;
                            data[7] |= v16;
                            var v17 = (byte)(data[15] >> 6);
                            data[15] *= 4;
                            data[15] |= v17;
                            var v18 = (byte)(data[9] >> 4);
                            data[9] *= 16;
                            data[9] |= v18;
                            var v19 = (byte)(data[8] >> 7);
                            data[8] *= 2;
                            data[8] |= v19;
                            var v20 = (byte)(data[5] >> 4);
                            data[5] *= 16;
                            data[5] |= v20;
                        }
                    }
                    else
                    {
                        var v26 = (byte)((data[1] >> 3) & 1);
                        if (((data[1] >> 6) & 1) != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        if (v26 != 0)
                        {
                            data[1] |= 0x40;
                        }
                        else
                        {
                            data[1] &= 0xBF;
                        }

                        var v9 = data[3];
                        data[3] = data[3];
                        data[3] = v9;
                        data[5] ^= 0xDA;
                        var v25 = (byte)((data[4] >> 5) & 1);
                        if (((data[4] >> 2) & 1) != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        if (v25 != 0)
                        {
                            data[4] |= 4;
                        }
                        else
                        {
                            data[4] &= 0xFB;
                        }

                        var v24 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 1) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v24 != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        var v10 = (byte)(data[1] >> 7);
                        data[1] *= 2;
                        data[1] |= v10;
                        var v11 = (byte)(data[1] >> 6);
                        data[1] *= 4;
                        data[1] |= v11;
                        var v12 = (byte)(data[3] >> 2);
                        data[3] <<= 6;
                        data[3] |= v12;
                        var v13 = data[2];
                        data[2] = data[0];
                        data[0] = v13;
                        var v14 = (byte)(data[3] >> 6);
                        data[3] *= 4;
                        data[3] |= v14;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[1];
                    data[1] = v3;
                    var v4 = (byte)(data[2] >> 4);
                    data[2] *= 16;
                    data[2] |= v4;
                    var v5 = data[3];
                    data[3] = data[2];
                    data[2] = v5;
                    data[2] ^= 0x7A;
                    var v2 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 3) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    data[0] ^= 0x1F;
                    var v27 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v27 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v6 = data[2];
                    data[2] = data[1];
                    data[1] = v6;
                    data[0] ^= 0x7B;
                    var v7 = data[1];
                    data[1] = data[0];
                    data[0] = v7;
                    data[1] ^= 0x57;
                    var v8 = data[1];
                    data[1] = data[2];
                    data[2] = v8;
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
                            var v21 = data[29];
                            data[29] = data[15];
                            data[15] = v21;
                            var v22 = data[7];
                            data[7] = data[15];
                            data[15] = v22;
                            data[0] ^= 0x26;
                        }
                        else
                        {
                            var v15 = (byte)(data[5] >> 4);
                            data[5] *= 16;
                            data[5] |= v15;
                            var v16 = (byte)(data[8] >> 1);
                            data[8] <<= 7;
                            data[8] |= v16;
                            var v17 = (byte)(data[9] >> 4);
                            data[9] *= 16;
                            data[9] |= v17;
                            var v18 = (byte)(data[15] >> 2);
                            data[15] <<= 6;
                            data[15] |= v18;
                            var v19 = (byte)(data[7] >> 3);
                            data[7] *= 32;
                            data[7] |= v19;
                            var v23 = (byte)((data[7] >> 2) & 1);
                            if (((data[7] >> 1) & 1) != 0)
                            {
                                data[7] |= 4;
                            }
                            else
                            {
                                data[7] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[7] |= 2;
                            }
                            else
                            {
                                data[7] &= 0xFD;
                            }

                            var v20 = (byte)(data[2] >> 3);
                            data[2] *= 32;
                            data[2] |= v20;
                            data[11] ^= 0xF8;
                        }
                    }
                    else
                    {
                        var v9 = (byte)(data[3] >> 2);
                        data[3] <<= 6;
                        data[3] |= v9;
                        var v10 = data[2];
                        data[2] = data[0];
                        data[0] = v10;
                        var v11 = (byte)(data[3] >> 6);
                        data[3] *= 4;
                        data[3] |= v11;
                        var v12 = (byte)(data[1] >> 2);
                        data[1] <<= 6;
                        data[1] |= v12;
                        var v13 = (byte)(data[1] >> 1);
                        data[1] <<= 7;
                        data[1] |= v13;
                        var v26 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 1) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v26 != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        var v25 = (byte)((data[4] >> 5) & 1);
                        if (((data[4] >> 2) & 1) != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        if (v25 != 0)
                        {
                            data[4] |= 4;
                        }
                        else
                        {
                            data[4] &= 0xFB;
                        }

                        data[5] ^= 0xDA;
                        var v14 = data[3];
                        data[3] = data[3];
                        data[3] = v14;
                        var v24 = (byte)((data[1] >> 3) & 1);
                        if (((data[1] >> 6) & 1) != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        if (v24 != 0)
                        {
                            data[1] |= 0x40;
                        }
                        else
                        {
                            data[1] &= 0xBF;
                        }
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    data[1] ^= 0x57;
                    var v4 = data[1];
                    data[1] = data[0];
                    data[0] = v4;
                    data[0] ^= 0x7B;
                    var v5 = data[2];
                    data[2] = data[1];
                    data[1] = v5;
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

                    data[0] ^= 0x1F;
                    var v27 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 3) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v27 != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    data[2] ^= 0x7A;
                    var v6 = data[3];
                    data[3] = data[2];
                    data[2] = v6;
                    var v7 = (byte)(data[2] >> 4);
                    data[2] *= 16;
                    data[2] |= v7;
                    var v8 = data[1];
                    data[1] = data[1];
                    data[1] = v8;
                }
            }
        }
    }
}