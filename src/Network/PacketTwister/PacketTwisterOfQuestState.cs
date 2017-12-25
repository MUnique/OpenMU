// <copyright file="PacketTwisterOfQuestState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'QuestState' type.
    /// </summary>
    internal class PacketTwisterOfQuestState : IPacketTwister
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
                            data[24] ^= 0xF5;
                            var v20 = data[10];
                            data[10] = data[31];
                            data[31] = v20;
                            var v26 = (byte)((data[31] >> 2) & 1);
                            if (((data[31] >> 1) & 1) != 0)
                            {
                                data[31] |= 4;
                            }
                            else
                            {
                                data[31] &= 0xFB;
                            }

                            if (v26 != 0)
                            {
                                data[31] |= 2;
                            }
                            else
                            {
                                data[31] &= 0xFD;
                            }

                            var v21 = data[24];
                            data[24] = data[8];
                            data[8] = v21;
                            var v22 = (byte)(data[17] >> 7);
                            data[17] *= 2;
                            data[17] |= v22;
                            data[0] ^= 0x82;
                            data[27] ^= 0x75;
                            var v23 = (byte)(data[25] >> 4);
                            data[25] *= 16;
                            data[25] |= v23;
                            var v24 = (byte)(data[12] >> 3);
                            data[12] *= 32;
                            data[12] |= v24;
                            var v25 = (byte)((data[13] >> 2) & 1);
                            if (((data[13] >> 2) & 1) != 0)
                            {
                                data[13] |= 4;
                            }
                            else
                            {
                                data[13] &= 0xFB;
                            }

                            if (v25 != 0)
                            {
                                data[13] |= 4;
                            }
                            else
                            {
                                data[13] &= 0xFB;
                            }

                            data[9] ^= 0x7B;
                            data[25] ^= 0x1D;
                        }
                        else
                        {
                            data[7] ^= 0x73;
                            var v15 = data[10];
                            data[10] = data[13];
                            data[13] = v15;
                            data[15] ^= 0x43;
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

                            var v16 = (byte)(data[11] >> 6);
                            data[11] *= 4;
                            data[11] |= v16;
                            var v17 = (byte)(data[11] >> 5);
                            data[11] *= 8;
                            data[11] |= v17;
                            var v18 = data[15];
                            data[15] = data[9];
                            data[9] = v18;
                            var v19 = data[11];
                            data[11] = data[13];
                            data[13] = v19;
                            data[1] ^= 0xCD;
                        }
                    }
                    else
                    {
                        data[1] ^= 0x1E;
                        var v8 = (byte)(data[4] >> 2);
                        data[4] <<= 6;
                        data[4] |= v8;
                        var v9 = (byte)(data[0] >> 5);
                        data[0] *= 8;
                        data[0] |= v9;
                        var v10 = data[0];
                        data[0] = data[4];
                        data[4] = v10;
                        var v11 = data[5];
                        data[5] = data[6];
                        data[6] = v11;
                        var v12 = (byte)(data[6] >> 4);
                        data[6] *= 16;
                        data[6] |= v12;
                        var v13 = data[6];
                        data[6] = data[6];
                        data[6] = v13;
                        var v2 = (byte)((data[5] >> 4) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        if (v2 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v14 = data[5];
                        data[5] = data[5];
                        data[5] = v14;
                        data[7] ^= 0xF9;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[3];
                    data[3] = v3;
                    var v4 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v4;
                    var v5 = data[1];
                    data[1] = data[2];
                    data[2] = v5;
                    var v6 = data[0];
                    data[0] = data[0];
                    data[0] = v6;
                    var v7 = data[2];
                    data[2] = data[1];
                    data[1] = v7;
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
                            data[25] ^= 0x1D;
                            data[9] ^= 0x7B;
                            var v26 = (byte)((data[13] >> 2) & 1);
                            if (((data[13] >> 2) & 1) != 0)
                            {
                                data[13] |= 4;
                            }
                            else
                            {
                                data[13] &= 0xFB;
                            }

                            if (v26 != 0)
                            {
                                data[13] |= 4;
                            }
                            else
                            {
                                data[13] &= 0xFB;
                            }

                            var v20 = (byte)(data[12] >> 5);
                            data[12] *= 8;
                            data[12] |= v20;
                            var v21 = (byte)(data[25] >> 4);
                            data[25] *= 16;
                            data[25] |= v21;
                            data[27] ^= 0x75;
                            data[0] ^= 0x82;
                            var v22 = (byte)(data[17] >> 1);
                            data[17] <<= 7;
                            data[17] |= v22;
                            var v23 = data[24];
                            data[24] = data[8];
                            data[8] = v23;
                            var v25 = (byte)((data[31] >> 2) & 1);
                            if (((data[31] >> 1) & 1) != 0)
                            {
                                data[31] |= 4;
                            }
                            else
                            {
                                data[31] &= 0xFB;
                            }

                            if (v25 != 0)
                            {
                                data[31] |= 2;
                            }
                            else
                            {
                                data[31] &= 0xFD;
                            }

                            var v24 = data[10];
                            data[10] = data[31];
                            data[31] = v24;
                            data[24] ^= 0xF5;
                        }
                        else
                        {
                            data[1] ^= 0xCD;
                            var v15 = data[11];
                            data[11] = data[13];
                            data[13] = v15;
                            var v16 = data[15];
                            data[15] = data[9];
                            data[9] = v16;
                            var v17 = (byte)(data[11] >> 3);
                            data[11] *= 32;
                            data[11] |= v17;
                            var v18 = (byte)(data[11] >> 2);
                            data[11] <<= 6;
                            data[11] |= v18;
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

                            data[15] ^= 0x43;
                            var v19 = data[10];
                            data[10] = data[13];
                            data[13] = v19;
                            data[7] ^= 0x73;
                        }
                    }
                    else
                    {
                        data[7] ^= 0xF9;
                        var v8 = data[5];
                        data[5] = data[5];
                        data[5] = v8;
                        var v2 = (byte)((data[5] >> 4) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        if (v2 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v9 = data[6];
                        data[6] = data[6];
                        data[6] = v9;
                        var v10 = (byte)(data[6] >> 4);
                        data[6] *= 16;
                        data[6] |= v10;
                        var v11 = data[5];
                        data[5] = data[6];
                        data[6] = v11;
                        var v12 = data[0];
                        data[0] = data[4];
                        data[4] = v12;
                        var v13 = (byte)(data[0] >> 3);
                        data[0] *= 32;
                        data[0] |= v13;
                        var v14 = (byte)(data[4] >> 6);
                        data[4] *= 4;
                        data[4] |= v14;
                        data[1] ^= 0x1E;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[1];
                    data[1] = v3;
                    var v4 = data[0];
                    data[0] = data[0];
                    data[0] = v4;
                    var v5 = data[1];
                    data[1] = data[2];
                    data[2] = v5;
                    var v6 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v6;
                    var v7 = data[1];
                    data[1] = data[3];
                    data[3] = v7;
                }
            }
        }
    }
}