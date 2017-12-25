// <copyright file="PacketTwisterOfChatInvitationRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'ChatInvitationRequest' type.
    /// </summary>
    internal class PacketTwisterOfChatInvitationRequest : IPacketTwister
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
                            var v21 = (byte)((data[25] >> 1) & 1);
                            if (((data[25] >> 1) & 1) != 0)
                            {
                                data[25] |= 2;
                            }
                            else
                            {
                                data[25] &= 0xFD;
                            }

                            if (v21 != 0)
                            {
                                data[25] |= 2;
                            }
                            else
                            {
                                data[25] &= 0xFD;
                            }

                            data[7] ^= 0x49;
                            var v14 = (byte)(data[0] >> 6);
                            data[0] *= 4;
                            data[0] |= v14;
                            var v15 = (byte)(data[4] >> 2);
                            data[4] <<= 6;
                            data[4] |= v15;
                            var v16 = (byte)(data[7] >> 6);
                            data[7] *= 4;
                            data[7] |= v16;
                            var v17 = (byte)(data[16] >> 6);
                            data[16] *= 4;
                            data[16] |= v17;
                            var v20 = (byte)((data[2] >> 7) & 1);
                            if (((data[2] >> 5) & 1) != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            if (v20 != 0)
                            {
                                data[2] |= 0x20;
                            }
                            else
                            {
                                data[2] &= 0xDF;
                            }

                            var v19 = (byte)((data[12] >> 7) & 1);
                            if (((data[12] >> 3) & 1) != 0)
                            {
                                data[12] |= 0x80;
                            }
                            else
                            {
                                data[12] &= 0x7F;
                            }

                            if (v19 != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            data[25] ^= 0xB;
                            var v18 = data[31];
                            data[31] = data[22];
                            data[22] = v18;
                        }
                        else
                        {
                            data[7] ^= 0x79;
                            data[12] ^= 0x2E;
                            var v10 = data[3];
                            data[3] = data[6];
                            data[6] = v10;
                            var v11 = data[14];
                            data[14] = data[13];
                            data[13] = v11;
                            var v12 = (byte)(data[14] >> 4);
                            data[14] *= 16;
                            data[14] |= v12;
                            var v22 = (byte)((data[9] >> 6) & 1);
                            if (((data[9] >> 5) & 1) != 0)
                            {
                                data[9] |= 0x40;
                            }
                            else
                            {
                                data[9] &= 0xBF;
                            }

                            if (v22 != 0)
                            {
                                data[9] |= 0x20;
                            }
                            else
                            {
                                data[9] &= 0xDF;
                            }

                            var v13 = (byte)(data[8] >> 3);
                            data[8] *= 32;
                            data[8] |= v13;
                            data[11] ^= 0x52;
                        }
                    }
                    else
                    {
                        var v23 = (byte)((data[4] >> 3) & 1);
                        if (((data[4] >> 2) & 1) != 0)
                        {
                            data[4] |= 8;
                        }
                        else
                        {
                            data[4] &= 0xF7;
                        }

                        if (v23 != 0)
                        {
                            data[4] |= 4;
                        }
                        else
                        {
                            data[4] &= 0xFB;
                        }

                        var v8 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v8;
                        data[1] ^= 0xC9;
                        data[7] ^= 0x22;
                        data[3] ^= 0xE8;
                        var v9 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v9;
                    }
                }
                else
                {
                    var v2 = (byte)((data[0] >> 1) & 1);
                    if (((data[0] >> 3) & 1) != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    data[3] ^= 0x41;
                    var v3 = data[0];
                    data[0] = data[1];
                    data[1] = v3;
                    var v4 = data[0];
                    data[0] = data[1];
                    data[1] = v4;
                    var v5 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v5;
                    var v6 = (byte)(data[0] >> 4);
                    data[0] *= 16;
                    data[0] |= v6;
                    var v7 = data[3];
                    data[3] = data[0];
                    data[0] = v7;
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
                            var v14 = data[31];
                            data[31] = data[22];
                            data[22] = v14;
                            data[25] ^= 0xB;
                            var v21 = (byte)((data[12] >> 7) & 1);
                            if (((data[12] >> 3) & 1) != 0)
                            {
                                data[12] |= 0x80;
                            }
                            else
                            {
                                data[12] &= 0x7F;
                            }

                            if (v21 != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            var v20 = (byte)((data[2] >> 7) & 1);
                            if (((data[2] >> 5) & 1) != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            if (v20 != 0)
                            {
                                data[2] |= 0x20;
                            }
                            else
                            {
                                data[2] &= 0xDF;
                            }

                            var v15 = (byte)(data[16] >> 2);
                            data[16] <<= 6;
                            data[16] |= v15;
                            var v16 = (byte)(data[7] >> 2);
                            data[7] <<= 6;
                            data[7] |= v16;
                            var v17 = (byte)(data[4] >> 6);
                            data[4] *= 4;
                            data[4] |= v17;
                            var v18 = (byte)(data[0] >> 2);
                            data[0] <<= 6;
                            data[0] |= v18;
                            data[7] ^= 0x49;
                            var v19 = (byte)((data[25] >> 1) & 1);
                            if (((data[25] >> 1) & 1) != 0)
                            {
                                data[25] |= 2;
                            }
                            else
                            {
                                data[25] &= 0xFD;
                            }

                            if (v19 != 0)
                            {
                                data[25] |= 2;
                            }
                            else
                            {
                                data[25] &= 0xFD;
                            }
                        }
                        else
                        {
                            data[11] ^= 0x52;
                            var v10 = (byte)(data[8] >> 5);
                            data[8] *= 8;
                            data[8] |= v10;
                            var v22 = (byte)((data[9] >> 6) & 1);
                            if (((data[9] >> 5) & 1) != 0)
                            {
                                data[9] |= 0x40;
                            }
                            else
                            {
                                data[9] &= 0xBF;
                            }

                            if (v22 != 0)
                            {
                                data[9] |= 0x20;
                            }
                            else
                            {
                                data[9] &= 0xDF;
                            }

                            var v11 = (byte)(data[14] >> 4);
                            data[14] *= 16;
                            data[14] |= v11;
                            var v12 = data[14];
                            data[14] = data[13];
                            data[13] = v12;
                            var v13 = data[3];
                            data[3] = data[6];
                            data[6] = v13;
                            data[12] ^= 0x2E;
                            data[7] ^= 0x79;
                        }
                    }
                    else
                    {
                        var v8 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v8;
                        data[3] ^= 0xE8;
                        data[7] ^= 0x22;
                        data[1] ^= 0xC9;
                        var v9 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v9;
                        var v23 = (byte)((data[4] >> 3) & 1);
                        if (((data[4] >> 2) & 1) != 0)
                        {
                            data[4] |= 8;
                        }
                        else
                        {
                            data[4] &= 0xF7;
                        }

                        if (v23 != 0)
                        {
                            data[4] |= 4;
                        }
                        else
                        {
                            data[4] &= 0xFB;
                        }
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[0];
                    data[0] = v3;
                    var v4 = (byte)(data[0] >> 4);
                    data[0] *= 16;
                    data[0] |= v4;
                    var v5 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v5;
                    var v6 = data[0];
                    data[0] = data[1];
                    data[1] = v6;
                    var v7 = data[0];
                    data[0] = data[1];
                    data[1] = v7;
                    data[3] ^= 0x41;
                    var v2 = (byte)((data[0] >> 1) & 1);
                    if (((data[0] >> 3) & 1) != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    if (v2 != 0)
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