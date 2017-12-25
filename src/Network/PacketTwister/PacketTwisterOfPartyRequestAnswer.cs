// <copyright file="PacketTwisterOfPartyRequestAnswer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'PartyRequestAnswer' type.
    /// </summary>
    internal class PacketTwisterOfPartyRequestAnswer : IPacketTwister
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
                            var v14 = data[2];
                            data[2] = data[13];
                            data[13] = v14;
                            data[30] ^= 0x59;
                            var v15 = data[28];
                            data[28] = data[20];
                            data[20] = v15;
                            var v16 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 7) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v16 != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }
                        }
                        else
                        {
                            var v19 = (byte)((data[14] >> 7) & 1);
                            if (((data[14] >> 2) & 1) != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            if (v19 != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            data[9] ^= 0x1F;
                            data[5] ^= 0xE8;
                            var v18 = (byte)((data[2] >> 7) & 1);
                            if (((data[2] >> 3) & 1) != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            if (v18 != 0)
                            {
                                data[2] |= 8;
                            }
                            else
                            {
                                data[2] &= 0xF7;
                            }

                            var v12 = (byte)(data[10] >> 7);
                            data[10] *= 2;
                            data[10] |= v12;
                            data[8] ^= 0x8D;
                            var v17 = (byte)((data[14] >> 4) & 1);
                            if (((data[14] >> 1) & 1) != 0)
                            {
                                data[14] |= 0x10;
                            }
                            else
                            {
                                data[14] &= 0xEF;
                            }

                            if (v17 != 0)
                            {
                                data[14] |= 2;
                            }
                            else
                            {
                                data[14] &= 0xFD;
                            }

                            var v13 = (byte)(data[2] >> 1);
                            data[2] <<= 7;
                            data[2] |= v13;
                        }
                    }
                    else
                    {
                        var v8 = (byte)(data[7] >> 4);
                        data[7] *= 16;
                        data[7] |= v8;
                        var v9 = data[5];
                        data[5] = data[6];
                        data[6] = v9;
                        var v10 = (byte)(data[1] >> 7);
                        data[1] *= 2;
                        data[1] |= v10;
                        var v11 = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= v11;
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[3];
                    data[3] = v3;
                    var v2 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v4 = data[2];
                    data[2] = data[1];
                    data[1] = v4;
                    data[2] ^= 0x53;
                    data[3] ^= 0xA5;
                    var v20 = (byte)((data[0] >> 5) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    if (v20 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    data[3] ^= 0x7C;
                    var v5 = data[3];
                    data[3] = data[0];
                    data[0] = v5;
                    var v6 = data[2];
                    data[2] = data[0];
                    data[0] = v6;
                    var v7 = data[0];
                    data[0] = data[1];
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
                            var v16 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 7) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v16 != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            var v14 = data[28];
                            data[28] = data[20];
                            data[20] = v14;
                            data[30] ^= 0x59;
                            var v15 = data[2];
                            data[2] = data[13];
                            data[13] = v15;
                        }
                        else
                        {
                            var v12 = (byte)(data[2] >> 7);
                            data[2] *= 2;
                            data[2] |= v12;
                            var v19 = (byte)((data[14] >> 4) & 1);
                            if (((data[14] >> 1) & 1) != 0)
                            {
                                data[14] |= 0x10;
                            }
                            else
                            {
                                data[14] &= 0xEF;
                            }

                            if (v19 != 0)
                            {
                                data[14] |= 2;
                            }
                            else
                            {
                                data[14] &= 0xFD;
                            }

                            data[8] ^= 0x8D;
                            var v13 = (byte)(data[10] >> 1);
                            data[10] <<= 7;
                            data[10] |= v13;
                            var v18 = (byte)((data[2] >> 7) & 1);
                            if (((data[2] >> 3) & 1) != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            if (v18 != 0)
                            {
                                data[2] |= 8;
                            }
                            else
                            {
                                data[2] &= 0xF7;
                            }

                            data[5] ^= 0xE8;
                            data[9] ^= 0x1F;
                            var v17 = (byte)((data[14] >> 7) & 1);
                            if (((data[14] >> 2) & 1) != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            if (v17 != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }
                        }
                    }
                    else
                    {
                        var v8 = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= v8;
                        var v9 = (byte)(data[1] >> 1);
                        data[1] <<= 7;
                        data[1] |= v9;
                        var v10 = data[5];
                        data[5] = data[6];
                        data[6] = v10;
                        var v11 = (byte)(data[7] >> 4);
                        data[7] *= 16;
                        data[7] |= v11;
                    }
                }
                else
                {
                    var v3 = data[0];
                    data[0] = data[1];
                    data[1] = v3;
                    var v4 = data[2];
                    data[2] = data[0];
                    data[0] = v4;
                    var v5 = data[3];
                    data[3] = data[0];
                    data[0] = v5;
                    data[3] ^= 0x7C;
                    var v2 = (byte)((data[0] >> 5) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    data[3] ^= 0xA5;
                    data[2] ^= 0x53;
                    var v6 = data[2];
                    data[2] = data[1];
                    data[1] = v6;
                    var v20 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v20 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v7 = data[3];
                    data[3] = data[3];
                    data[3] = v7;
                }
            }
        }
    }
}