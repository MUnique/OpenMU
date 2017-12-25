// <copyright file="PacketTwisterOfQuestInfoRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'QuestInfoRequest' type.
    /// </summary>
    internal class PacketTwisterOfQuestInfoRequest : IPacketTwister
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
                            var v15 = (byte)(data[23] >> 5);
                            data[23] *= 8;
                            data[23] |= v15;
                            var v17 = (byte)((data[5] >> 2) & 1);
                            if (((data[5] >> 2) & 1) != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            var v16 = (byte)((data[24] >> 3) & 1);
                            if (((data[24] >> 1) & 1) != 0)
                            {
                                data[24] |= 8;
                            }
                            else
                            {
                                data[24] &= 0xF7;
                            }

                            if (v16 != 0)
                            {
                                data[24] |= 2;
                            }
                            else
                            {
                                data[24] &= 0xFD;
                            }

                            data[19] ^= 4;
                            data[12] ^= 0xB3;
                            data[0] ^= 0xC4;
                            data[3] ^= 0x39;
                        }
                        else
                        {
                            var v10 = (byte)(data[14] >> 4);
                            data[14] *= 16;
                            data[14] |= v10;
                            data[4] ^= 0xE3;
                            var v11 = (byte)(data[4] >> 3);
                            data[4] *= 32;
                            data[4] |= v11;
                            data[4] ^= 0xE0;
                            var v12 = data[7];
                            data[7] = data[1];
                            data[1] = v12;
                            var v13 = (byte)(data[4] >> 2);
                            data[4] <<= 6;
                            data[4] |= v13;
                            var v14 = data[11];
                            data[11] = data[6];
                            data[6] = v14;
                            data[14] ^= 0x68;
                        }
                    }
                    else
                    {
                        data[7] ^= 0xC2;
                        data[1] ^= 0xEA;
                        var v8 = data[3];
                        data[3] = data[7];
                        data[7] = v8;
                        var v9 = data[2];
                        data[2] = data[1];
                        data[1] = v9;
                        var v18 = (byte)((data[0] >> 1) & 1);
                        if (((data[0] >> 2) & 1) != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        if (v18 != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }
                    }
                }
                else
                {
                    data[1] ^= 0xD0;
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    var v4 = data[1];
                    data[1] = data[1];
                    data[1] = v4;
                    data[3] ^= 0xE8;
                    var v5 = data[2];
                    data[2] = data[1];
                    data[1] = v5;
                    var v2 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 5) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    var v6 = data[1];
                    data[1] = data[2];
                    data[2] = v6;
                    var v7 = (byte)(data[3] >> 4);
                    data[3] *= 16;
                    data[3] |= v7;
                    data[0] ^= 0xEE;
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
                            data[3] ^= 0x39;
                            data[0] ^= 0xC4;
                            data[12] ^= 0xB3;
                            data[19] ^= 4;
                            var v17 = (byte)((data[24] >> 3) & 1);
                            if (((data[24] >> 1) & 1) != 0)
                            {
                                data[24] |= 8;
                            }
                            else
                            {
                                data[24] &= 0xF7;
                            }

                            if (v17 != 0)
                            {
                                data[24] |= 2;
                            }
                            else
                            {
                                data[24] &= 0xFD;
                            }

                            var v16 = (byte)((data[5] >> 2) & 1);
                            if (((data[5] >> 2) & 1) != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            if (v16 != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            var v15 = (byte)(data[23] >> 3);
                            data[23] *= 32;
                            data[23] |= v15;
                        }
                        else
                        {
                            data[14] ^= 0x68;
                            var v10 = data[11];
                            data[11] = data[6];
                            data[6] = v10;
                            var v11 = (byte)(data[4] >> 6);
                            data[4] *= 4;
                            data[4] |= v11;
                            var v12 = data[7];
                            data[7] = data[1];
                            data[1] = v12;
                            data[4] ^= 0xE0;
                            var v13 = (byte)(data[4] >> 5);
                            data[4] *= 8;
                            data[4] |= v13;
                            data[4] ^= 0xE3;
                            var v14 = (byte)(data[14] >> 4);
                            data[14] *= 16;
                            data[14] |= v14;
                        }
                    }
                    else
                    {
                        var v18 = (byte)((data[0] >> 1) & 1);
                        if (((data[0] >> 2) & 1) != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        if (v18 != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        var v8 = data[2];
                        data[2] = data[1];
                        data[1] = v8;
                        var v9 = data[3];
                        data[3] = data[7];
                        data[7] = v9;
                        data[1] ^= 0xEA;
                        data[7] ^= 0xC2;
                    }
                }
                else
                {
                    data[0] ^= 0xEE;
                    var v3 = (byte)(data[3] >> 4);
                    data[3] *= 16;
                    data[3] |= v3;
                    var v4 = data[1];
                    data[1] = data[2];
                    data[2] = v4;
                    var v2 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 5) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    var v5 = data[2];
                    data[2] = data[1];
                    data[1] = v5;
                    data[3] ^= 0xE8;
                    var v6 = data[1];
                    data[1] = data[1];
                    data[1] = v6;
                    var v7 = data[1];
                    data[1] = data[2];
                    data[2] = v7;
                    data[1] ^= 0xD0;
                }
            }
        }
    }
}