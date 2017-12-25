// <copyright file="PacketTwisterOfPartyList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'PartyList' type.
    /// </summary>
    internal class PacketTwisterOfPartyList : IPacketTwister
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
                            var v13 = data[9];
                            data[9] = data[28];
                            data[28] = v13;
                            var v16 = (byte)((data[15] >> 7) & 1);
                            if (((data[15] >> 1) & 1) != 0)
                            {
                                data[15] |= 0x80;
                            }
                            else
                            {
                                data[15] &= 0x7F;
                            }

                            if (v16 != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            data[22] ^= 0x10;
                            var v14 = (byte)(data[19] >> 2);
                            data[19] <<= 6;
                            data[19] |= v14;
                            var v15 = data[9];
                            data[9] = data[17];
                            data[17] = v15;
                            data[9] ^= 0xC2;
                        }
                        else
                        {
                            var v9 = data[6];
                            data[6] = data[9];
                            data[9] = v9;
                            data[0] ^= 0x78;
                            data[5] ^= 0x65;
                            data[2] ^= 0x64;
                            var v20 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v20 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            var v19 = (byte)((data[1] >> 5) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 0x20;
                            }
                            else
                            {
                                data[1] &= 0xDF;
                            }

                            if (v19 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            var v18 = (byte)((data[13] >> 4) & 1);
                            if (((data[13] >> 1) & 1) != 0)
                            {
                                data[13] |= 0x10;
                            }
                            else
                            {
                                data[13] &= 0xEF;
                            }

                            if (v18 != 0)
                            {
                                data[13] |= 2;
                            }
                            else
                            {
                                data[13] &= 0xFD;
                            }

                            var v10 = data[8];
                            data[8] = data[10];
                            data[10] = v10;
                            var v17 = (byte)((data[5] >> 2) & 1);
                            if (((data[5] >> 7) & 1) != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            var v11 = data[10];
                            data[10] = data[6];
                            data[6] = v11;
                            var v12 = (byte)(data[5] >> 4);
                            data[5] *= 16;
                            data[5] |= v12;
                        }
                    }
                    else
                    {
                        var v4 = data[3];
                        data[3] = data[3];
                        data[3] = v4;
                        data[4] ^= 0x32;
                        var v21 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 1) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v21 != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        var v5 = (byte)(data[7] >> 2);
                        data[7] <<= 6;
                        data[7] |= v5;
                        var v6 = (byte)(data[6] >> 6);
                        data[6] *= 4;
                        data[6] |= v6;
                        var v7 = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= v7;
                        var v8 = (byte)(data[6] >> 7);
                        data[6] *= 2;
                        data[6] |= v8;
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

                    data[0] ^= 0x65;
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    data[2] ^= 0x31;
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
                            data[9] ^= 0xC2;
                            var v13 = data[9];
                            data[9] = data[17];
                            data[17] = v13;
                            var v14 = (byte)(data[19] >> 6);
                            data[19] *= 4;
                            data[19] |= v14;
                            data[22] ^= 0x10;
                            var v16 = (byte)((data[15] >> 7) & 1);
                            if (((data[15] >> 1) & 1) != 0)
                            {
                                data[15] |= 0x80;
                            }
                            else
                            {
                                data[15] &= 0x7F;
                            }

                            if (v16 != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            var v15 = data[9];
                            data[9] = data[28];
                            data[28] = v15;
                        }
                        else
                        {
                            var v9 = (byte)(data[5] >> 4);
                            data[5] *= 16;
                            data[5] |= v9;
                            var v10 = data[10];
                            data[10] = data[6];
                            data[6] = v10;
                            var v20 = (byte)((data[5] >> 2) & 1);
                            if (((data[5] >> 7) & 1) != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            var v11 = data[8];
                            data[8] = data[10];
                            data[10] = v11;
                            var v19 = (byte)((data[13] >> 4) & 1);
                            if (((data[13] >> 1) & 1) != 0)
                            {
                                data[13] |= 0x10;
                            }
                            else
                            {
                                data[13] &= 0xEF;
                            }

                            if (v19 != 0)
                            {
                                data[13] |= 2;
                            }
                            else
                            {
                                data[13] &= 0xFD;
                            }

                            var v18 = (byte)((data[1] >> 5) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 0x20;
                            }
                            else
                            {
                                data[1] &= 0xDF;
                            }

                            if (v18 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            var v17 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v17 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            data[2] ^= 0x64;
                            data[5] ^= 0x65;
                            data[0] ^= 0x78;
                            var v12 = data[6];
                            data[6] = data[9];
                            data[9] = v12;
                        }
                    }
                    else
                    {
                        var v4 = (byte)(data[6] >> 1);
                        data[6] <<= 7;
                        data[6] |= v4;
                        var v5 = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= v5;
                        var v6 = (byte)(data[6] >> 2);
                        data[6] <<= 6;
                        data[6] |= v6;
                        var v7 = (byte)(data[7] >> 6);
                        data[7] *= 4;
                        data[7] |= v7;
                        var v21 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 1) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v21 != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        data[4] ^= 0x32;
                        var v8 = data[3];
                        data[3] = data[3];
                        data[3] = v8;
                    }
                }
                else
                {
                    data[2] ^= 0x31;
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    data[0] ^= 0x65;
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
                }
            }
        }
    }
}