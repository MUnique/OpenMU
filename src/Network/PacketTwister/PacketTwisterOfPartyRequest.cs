// <copyright file="PacketTwisterOfPartyRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'PartyRequest' type.
    /// </summary>
    internal class PacketTwisterOfPartyRequest : IPacketTwister
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
                            var v21 = (byte)(data[4] >> 1);
                            data[4] <<= 7;
                            data[4] |= v21;
                            var v22 = data[20];
                            data[20] = data[5];
                            data[5] = v22;
                            var v29 = (byte)((data[1] >> 1) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            if (v29 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            var v23 = data[28];
                            data[28] = data[26];
                            data[26] = v23;
                            var v24 = (byte)(data[16] >> 6);
                            data[16] *= 4;
                            data[16] |= v24;
                            var v28 = (byte)((data[16] >> 5) & 1);
                            if (((data[16] >> 7) & 1) != 0)
                            {
                                data[16] |= 0x20;
                            }
                            else
                            {
                                data[16] &= 0xDF;
                            }

                            if (v28 != 0)
                            {
                                data[16] |= 0x80;
                            }
                            else
                            {
                                data[16] &= 0x7F;
                            }

                            data[18] ^= 0x6C;
                            var v25 = data[14];
                            data[14] = data[12];
                            data[12] = v25;
                            var v26 = (byte)(data[20] >> 1);
                            data[20] <<= 7;
                            data[20] |= v26;
                            var v27 = (byte)(data[16] >> 4);
                            data[16] *= 16;
                            data[16] |= v27;
                        }
                        else
                        {
                            var v13 = data[12];
                            data[12] = data[0];
                            data[0] = v13;
                            var v14 = data[2];
                            data[2] = data[0];
                            data[0] = v14;
                            var v15 = (byte)(data[0] >> 6);
                            data[0] *= 4;
                            data[0] |= v15;
                            var v16 = data[12];
                            data[12] = data[6];
                            data[6] = v16;
                            data[4] ^= 0xC;
                            var v30 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 7) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v30 != 0)
                            {
                                data[0] |= 0x80;
                            }
                            else
                            {
                                data[0] &= 0x7F;
                            }

                            var v17 = (byte)(data[12] >> 3);
                            data[12] *= 32;
                            data[12] |= v17;
                            data[9] ^= 0x28;
                            var v18 = data[12];
                            data[12] = data[14];
                            data[14] = v18;
                            var v19 = data[1];
                            data[1] = data[15];
                            data[15] = v19;
                            var v20 = data[11];
                            data[11] = data[14];
                            data[14] = v20;
                        }
                    }
                    else
                    {
                        var v7 = (byte)(data[5] >> 4);
                        data[5] *= 16;
                        data[5] |= v7;
                        var v8 = data[6];
                        data[6] = data[3];
                        data[3] = v8;
                        var v2 = (byte)((data[5] >> 1) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        if (v2 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v9 = (byte)(data[4] >> 6);
                        data[4] *= 4;
                        data[4] |= v9;
                        data[1] ^= 0x5D;
                        data[2] ^= 0xB3;
                        var v10 = data[3];
                        data[3] = data[0];
                        data[0] = v10;
                        var v31 = (byte)((data[7] >> 2) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 4;
                        }
                        else
                        {
                            data[7] &= 0xFB;
                        }

                        if (v31 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        var v11 = (byte)(data[3] >> 6);
                        data[3] *= 4;
                        data[3] |= v11;
                        var v12 = (byte)(data[4] >> 7);
                        data[4] *= 2;
                        data[4] |= v12;
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[1];
                    data[1] = v3;
                    var v4 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v4;
                    data[0] ^= 0xC4;
                    var v5 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v5;
                    var v6 = data[2];
                    data[2] = data[0];
                    data[0] = v6;
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
                            var v21 = (byte)(data[16] >> 4);
                            data[16] *= 16;
                            data[16] |= v21;
                            var v22 = (byte)(data[20] >> 7);
                            data[20] *= 2;
                            data[20] |= v22;
                            var v23 = data[14];
                            data[14] = data[12];
                            data[12] = v23;
                            data[18] ^= 0x6C;
                            var v29 = (byte)((data[16] >> 5) & 1);
                            if (((data[16] >> 7) & 1) != 0)
                            {
                                data[16] |= 0x20;
                            }
                            else
                            {
                                data[16] &= 0xDF;
                            }

                            if (v29 != 0)
                            {
                                data[16] |= 0x80;
                            }
                            else
                            {
                                data[16] &= 0x7F;
                            }

                            var v24 = (byte)(data[16] >> 2);
                            data[16] <<= 6;
                            data[16] |= v24;
                            var v25 = data[28];
                            data[28] = data[26];
                            data[26] = v25;
                            var v28 = (byte)((data[1] >> 1) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            if (v28 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            var v26 = data[20];
                            data[20] = data[5];
                            data[5] = v26;
                            var v27 = (byte)(data[4] >> 7);
                            data[4] *= 2;
                            data[4] |= v27;
                        }
                        else
                        {
                            var v13 = data[11];
                            data[11] = data[14];
                            data[14] = v13;
                            var v14 = data[1];
                            data[1] = data[15];
                            data[15] = v14;
                            var v15 = data[12];
                            data[12] = data[14];
                            data[14] = v15;
                            data[9] ^= 0x28;
                            var v16 = (byte)(data[12] >> 5);
                            data[12] *= 8;
                            data[12] |= v16;
                            var v30 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 7) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v30 != 0)
                            {
                                data[0] |= 0x80;
                            }
                            else
                            {
                                data[0] &= 0x7F;
                            }

                            data[4] ^= 0xC;
                            var v17 = data[12];
                            data[12] = data[6];
                            data[6] = v17;
                            var v18 = (byte)(data[0] >> 2);
                            data[0] <<= 6;
                            data[0] |= v18;
                            var v19 = data[2];
                            data[2] = data[0];
                            data[0] = v19;
                            var v20 = data[12];
                            data[12] = data[0];
                            data[0] = v20;
                        }
                    }
                    else
                    {
                        var v7 = (byte)(data[4] >> 1);
                        data[4] <<= 7;
                        data[4] |= v7;
                        var v8 = (byte)(data[3] >> 2);
                        data[3] <<= 6;
                        data[3] |= v8;
                        var v2 = (byte)((data[7] >> 2) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 4;
                        }
                        else
                        {
                            data[7] &= 0xFB;
                        }

                        if (v2 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        var v9 = data[3];
                        data[3] = data[0];
                        data[0] = v9;
                        data[2] ^= 0xB3;
                        data[1] ^= 0x5D;
                        var v10 = (byte)(data[4] >> 2);
                        data[4] <<= 6;
                        data[4] |= v10;
                        var v31 = (byte)((data[5] >> 1) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        if (v31 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v11 = data[6];
                        data[6] = data[3];
                        data[3] = v11;
                        var v12 = (byte)(data[5] >> 4);
                        data[5] *= 16;
                        data[5] |= v12;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[0];
                    data[0] = v3;
                    var v4 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v4;
                    data[0] ^= 0xC4;
                    var v5 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v5;
                    var v6 = data[3];
                    data[3] = data[1];
                    data[1] = v6;
                }
            }
        }
    }
}