// <copyright file="PacketTwisterOfFriendAddResponse.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'FriendAddResponse' type.
    /// </summary>
    internal class PacketTwisterOfFriendAddResponse : IPacketTwister
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
                            var v15 = (byte)((data[11] >> 6) & 1);
                            if (((data[11] >> 3) & 1) != 0)
                            {
                                data[11] |= 0x40;
                            }
                            else
                            {
                                data[11] &= 0xBF;
                            }

                            if (v15 != 0)
                            {
                                data[11] |= 8;
                            }
                            else
                            {
                                data[11] &= 0xF7;
                            }

                            var v11 = (byte)(data[17] >> 2);
                            data[17] <<= 6;
                            data[17] |= v11;
                            data[2] ^= 0x73;
                            var v12 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v12;
                            data[29] ^= 0xCE;
                            var v13 = (byte)(data[5] >> 6);
                            data[5] *= 4;
                            data[5] |= v13;
                            var v14 = data[27];
                            data[27] = data[30];
                            data[30] = v14;
                            data[5] ^= 0xE9;
                        }
                        else
                        {
                            var v9 = data[3];
                            data[3] = data[2];
                            data[2] = v9;
                            var v17 = (byte)((data[13] >> 6) & 1);
                            if (((data[13] >> 6) & 1) != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            if (v17 != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            var v10 = data[15];
                            data[15] = data[8];
                            data[8] = v10;
                            var v16 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 1) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v16 != 0)
                            {
                                data[3] |= 2;
                            }
                            else
                            {
                                data[3] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        var v8 = data[3];
                        data[3] = data[6];
                        data[6] = v8;
                        data[3] ^= 0xC1;
                        data[5] ^= 0x29;
                        data[7] ^= 0xA9;
                    }
                }
                else
                {
                    data[1] ^= 0x48;
                    var v2 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    data[0] ^= 0xF5;
                    var v3 = data[1];
                    data[1] = data[1];
                    data[1] = v3;
                    var v4 = data[1];
                    data[1] = data[1];
                    data[1] = v4;
                    data[2] ^= 0x32;
                    data[0] ^= 0x51;
                    var v5 = data[0];
                    data[0] = data[2];
                    data[2] = v5;
                    var v18 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v18 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    var v6 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v6;
                    var v7 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v7;
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
                            data[5] ^= 0xE9;
                            var v11 = data[27];
                            data[27] = data[30];
                            data[30] = v11;
                            var v12 = (byte)(data[5] >> 2);
                            data[5] <<= 6;
                            data[5] |= v12;
                            data[29] ^= 0xCE;
                            var v13 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v13;
                            data[2] ^= 0x73;
                            var v14 = (byte)(data[17] >> 6);
                            data[17] *= 4;
                            data[17] |= v14;
                            var v15 = (byte)((data[11] >> 6) & 1);
                            if (((data[11] >> 3) & 1) != 0)
                            {
                                data[11] |= 0x40;
                            }
                            else
                            {
                                data[11] &= 0xBF;
                            }

                            if (v15 != 0)
                            {
                                data[11] |= 8;
                            }
                            else
                            {
                                data[11] &= 0xF7;
                            }
                        }
                        else
                        {
                            var v17 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 1) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[3] |= 2;
                            }
                            else
                            {
                                data[3] &= 0xFD;
                            }

                            var v9 = data[15];
                            data[15] = data[8];
                            data[8] = v9;
                            var v16 = (byte)((data[13] >> 6) & 1);
                            if (((data[13] >> 6) & 1) != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            if (v16 != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            var v10 = data[3];
                            data[3] = data[2];
                            data[2] = v10;
                        }
                    }
                    else
                    {
                        data[7] ^= 0xA9;
                        data[5] ^= 0x29;
                        data[3] ^= 0xC1;
                        var v8 = data[3];
                        data[3] = data[6];
                        data[6] = v8;
                    }
                }
                else
                {
                    var v3 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v3;
                    var v4 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v4;
                    var v2 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    var v5 = data[0];
                    data[0] = data[2];
                    data[2] = v5;
                    data[0] ^= 0x51;
                    data[2] ^= 0x32;
                    var v6 = data[1];
                    data[1] = data[1];
                    data[1] = v6;
                    var v7 = data[1];
                    data[1] = data[1];
                    data[1] = v7;
                    data[0] ^= 0xF5;
                    var v18 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v18 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    data[1] ^= 0x48;
                }
            }
        }
    }
}