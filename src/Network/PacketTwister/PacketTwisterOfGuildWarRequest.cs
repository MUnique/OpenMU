// <copyright file="PacketTwisterOfGuildWarRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'GuildWarRequest' type.
    /// </summary>
    internal class PacketTwisterOfGuildWarRequest : IPacketTwister
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
                            var v21 = (byte)(data[7] >> 1);
                            data[7] <<= 7;
                            data[7] |= v21;
                            var v22 = (byte)(data[30] >> 6);
                            data[30] *= 4;
                            data[30] |= v22;
                            var v23 = data[18];
                            data[18] = data[5];
                            data[5] = v23;
                            var v24 = data[20];
                            data[20] = data[13];
                            data[13] = v24;
                            var v25 = (byte)(data[4] >> 1);
                            data[4] <<= 7;
                            data[4] |= v25;
                            data[18] ^= 0x91;
                        }
                        else
                        {
                            var v28 = (byte)((data[1] >> 2) & 1);
                            if (((data[1] >> 5) & 1) != 0)
                            {
                                data[1] |= 4;
                            }
                            else
                            {
                                data[1] &= 0xFB;
                            }

                            if (v28 != 0)
                            {
                                data[1] |= 0x20;
                            }
                            else
                            {
                                data[1] &= 0xDF;
                            }

                            var v16 = data[5];
                            data[5] = data[6];
                            data[6] = v16;
                            data[11] ^= 0xBC;
                            var v17 = data[0];
                            data[0] = data[0];
                            data[0] = v17;
                            var v18 = data[0];
                            data[0] = data[6];
                            data[6] = v18;
                            var v19 = data[0];
                            data[0] = data[12];
                            data[12] = v19;
                            var v20 = (byte)(data[15] >> 7);
                            data[15] *= 2;
                            data[15] |= v20;
                            var v27 = (byte)((data[15] >> 3) & 1);
                            if (((data[15] >> 5) & 1) != 0)
                            {
                                data[15] |= 8;
                            }
                            else
                            {
                                data[15] &= 0xF7;
                            }

                            if (v27 != 0)
                            {
                                data[15] |= 0x20;
                            }
                            else
                            {
                                data[15] &= 0xDF;
                            }

                            var v26 = (byte)((data[13] >> 3) & 1);
                            if (((data[13] >> 6) & 1) != 0)
                            {
                                data[13] |= 8;
                            }
                            else
                            {
                                data[13] &= 0xF7;
                            }

                            if (v26 != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }
                        }
                    }
                    else
                    {
                        var v8 = (byte)(data[5] >> 7);
                        data[5] *= 2;
                        data[5] |= v8;
                        var v9 = data[5];
                        data[5] = data[0];
                        data[0] = v9;
                        var v10 = (byte)(data[0] >> 5);
                        data[0] *= 8;
                        data[0] |= v10;
                        var v11 = data[3];
                        data[3] = data[4];
                        data[4] = v11;
                        var v12 = data[7];
                        data[7] = data[1];
                        data[1] = v12;
                        var v13 = data[0];
                        data[0] = data[1];
                        data[1] = v13;
                        data[3] ^= 0x47;
                        data[0] ^= 0xAF;
                        var v29 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 5) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v29 != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        data[1] ^= 0x18;
                        var v14 = data[3];
                        data[3] = data[6];
                        data[6] = v14;
                        var v15 = (byte)(data[5] >> 5);
                        data[5] *= 8;
                        data[5] |= v15;
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[3];
                    data[3] = v3;
                    var v4 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v4;
                    var v5 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v5;
                    var v6 = data[3];
                    data[3] = data[2];
                    data[2] = v6;
                    data[1] ^= 0xD8;
                    var v2 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    var v7 = data[1];
                    data[1] = data[2];
                    data[2] = v7;
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
                            data[18] ^= 0x91;
                            var v21 = (byte)(data[4] >> 7);
                            data[4] *= 2;
                            data[4] |= v21;
                            var v22 = data[20];
                            data[20] = data[13];
                            data[13] = v22;
                            var v23 = data[18];
                            data[18] = data[5];
                            data[5] = v23;
                            var v24 = (byte)(data[30] >> 2);
                            data[30] <<= 6;
                            data[30] |= v24;
                            var v25 = (byte)(data[7] >> 7);
                            data[7] *= 2;
                            data[7] |= v25;
                        }
                        else
                        {
                            var v28 = (byte)((data[13] >> 3) & 1);
                            if (((data[13] >> 6) & 1) != 0)
                            {
                                data[13] |= 8;
                            }
                            else
                            {
                                data[13] &= 0xF7;
                            }

                            if (v28 != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            var v27 = (byte)((data[15] >> 3) & 1);
                            if (((data[15] >> 5) & 1) != 0)
                            {
                                data[15] |= 8;
                            }
                            else
                            {
                                data[15] &= 0xF7;
                            }

                            if (v27 != 0)
                            {
                                data[15] |= 0x20;
                            }
                            else
                            {
                                data[15] &= 0xDF;
                            }

                            var v16 = (byte)(data[15] >> 1);
                            data[15] <<= 7;
                            data[15] |= v16;
                            var v17 = data[0];
                            data[0] = data[12];
                            data[12] = v17;
                            var v18 = data[0];
                            data[0] = data[6];
                            data[6] = v18;
                            var v19 = data[0];
                            data[0] = data[0];
                            data[0] = v19;
                            data[11] ^= 0xBC;
                            var v20 = data[5];
                            data[5] = data[6];
                            data[6] = v20;
                            var v26 = (byte)((data[1] >> 2) & 1);
                            if (((data[1] >> 5) & 1) != 0)
                            {
                                data[1] |= 4;
                            }
                            else
                            {
                                data[1] &= 0xFB;
                            }

                            if (v26 != 0)
                            {
                                data[1] |= 0x20;
                            }
                            else
                            {
                                data[1] &= 0xDF;
                            }
                        }
                    }
                    else
                    {
                        var v8 = (byte)(data[5] >> 3);
                        data[5] *= 32;
                        data[5] |= v8;
                        var v9 = data[3];
                        data[3] = data[6];
                        data[6] = v9;
                        data[1] ^= 0x18;
                        var v29 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 5) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v29 != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        data[0] ^= 0xAF;
                        data[3] ^= 0x47;
                        var v10 = data[0];
                        data[0] = data[1];
                        data[1] = v10;
                        var v11 = data[7];
                        data[7] = data[1];
                        data[1] = v11;
                        var v12 = data[3];
                        data[3] = data[4];
                        data[4] = v12;
                        var v13 = (byte)(data[0] >> 3);
                        data[0] *= 32;
                        data[0] |= v13;
                        var v14 = data[5];
                        data[5] = data[0];
                        data[0] = v14;
                        var v15 = (byte)(data[5] >> 1);
                        data[5] <<= 7;
                        data[5] |= v15;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    var v2 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    data[1] ^= 0xD8;
                    var v4 = data[3];
                    data[3] = data[2];
                    data[2] = v4;
                    var v5 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v5;
                    var v6 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v6;
                    var v7 = data[3];
                    data[3] = data[3];
                    data[3] = v7;
                }
            }
        }
    }
}