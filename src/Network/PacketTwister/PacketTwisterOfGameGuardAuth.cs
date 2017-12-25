// <copyright file="PacketTwisterOfGameGuardAuth.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'GameGuardAuth' type.
    /// </summary>
    internal class PacketTwisterOfGameGuardAuth : IPacketTwister
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
                            var v16 = data[1];
                            data[1] = data[4];
                            data[4] = v16;
                            data[1] ^= 0xA3;
                            var v17 = data[4];
                            data[4] = data[28];
                            data[28] = v17;
                            var v20 = (byte)((data[13] >> 2) & 1);
                            if (((data[13] >> 3) & 1) != 0)
                            {
                                data[13] |= 4;
                            }
                            else
                            {
                                data[13] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[13] |= 8;
                            }
                            else
                            {
                                data[13] &= 0xF7;
                            }

                            data[17] ^= 0xB5;
                            var v18 = (byte)(data[18] >> 2);
                            data[18] <<= 6;
                            data[18] |= v18;
                            var v19 = (byte)((data[6] >> 2) & 1);
                            if (((data[6] >> 7) & 1) != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            if (v19 != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }
                        }
                        else
                        {
                            var v21 = (byte)((data[8] >> 1) & 1);
                            if (((data[8] >> 1) & 1) != 0)
                            {
                                data[8] |= 2;
                            }
                            else
                            {
                                data[8] &= 0xFD;
                            }

                            if (v21 != 0)
                            {
                                data[8] |= 2;
                            }
                            else
                            {
                                data[8] &= 0xFD;
                            }

                            var v14 = data[11];
                            data[11] = data[6];
                            data[6] = v14;
                            var v15 = data[1];
                            data[1] = data[3];
                            data[3] = v15;
                        }
                    }
                    else
                    {
                        var v8 = data[7];
                        data[7] = data[4];
                        data[4] = v8;
                        var v22 = (byte)((data[6] >> 3) & 1);
                        if (((data[6] >> 5) & 1) != 0)
                        {
                            data[6] |= 8;
                        }
                        else
                        {
                            data[6] &= 0xF7;
                        }

                        if (v22 != 0)
                        {
                            data[6] |= 0x20;
                        }
                        else
                        {
                            data[6] &= 0xDF;
                        }

                        var v9 = (byte)(data[5] >> 2);
                        data[5] <<= 6;
                        data[5] |= v9;
                        data[2] ^= 0xC;
                        var v10 = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= v10;
                        var v11 = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= v11;
                        var v12 = (byte)(data[5] >> 4);
                        data[5] *= 16;
                        data[5] |= v12;
                        var v13 = (byte)(data[1] >> 7);
                        data[1] *= 2;
                        data[1] |= v13;
                        data[6] ^= 6;
                        data[0] ^= 0x70;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[0];
                    data[0] = v3;
                    var v4 = data[1];
                    data[1] = data[1];
                    data[1] = v4;
                    var v5 = data[0];
                    data[0] = data[2];
                    data[2] = v5;
                    var v6 = data[0];
                    data[0] = data[1];
                    data[1] = v6;
                    data[2] ^= 0x9E;
                    var v7 = (byte)(data[2] >> 7);
                    data[2] *= 2;
                    data[2] |= v7;
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    data[3] ^= 0xFD;
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
                            var v20 = (byte)((data[6] >> 2) & 1);
                            if (((data[6] >> 7) & 1) != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            var v16 = (byte)(data[18] >> 6);
                            data[18] *= 4;
                            data[18] |= v16;
                            data[17] ^= 0xB5;
                            var v19 = (byte)((data[13] >> 2) & 1);
                            if (((data[13] >> 3) & 1) != 0)
                            {
                                data[13] |= 4;
                            }
                            else
                            {
                                data[13] &= 0xFB;
                            }

                            if (v19 != 0)
                            {
                                data[13] |= 8;
                            }
                            else
                            {
                                data[13] &= 0xF7;
                            }

                            var v17 = data[4];
                            data[4] = data[28];
                            data[28] = v17;
                            data[1] ^= 0xA3;
                            var v18 = data[1];
                            data[1] = data[4];
                            data[4] = v18;
                        }
                        else
                        {
                            var v14 = data[1];
                            data[1] = data[3];
                            data[3] = v14;
                            var v15 = data[11];
                            data[11] = data[6];
                            data[6] = v15;
                            var v21 = (byte)((data[8] >> 1) & 1);
                            if (((data[8] >> 1) & 1) != 0)
                            {
                                data[8] |= 2;
                            }
                            else
                            {
                                data[8] &= 0xFD;
                            }

                            if (v21 != 0)
                            {
                                data[8] |= 2;
                            }
                            else
                            {
                                data[8] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        data[0] ^= 0x70;
                        data[6] ^= 6;
                        var v8 = (byte)(data[1] >> 1);
                        data[1] <<= 7;
                        data[1] |= v8;
                        var v9 = (byte)(data[5] >> 4);
                        data[5] *= 16;
                        data[5] |= v9;
                        var v10 = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= v10;
                        var v11 = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= v11;
                        data[2] ^= 0xC;
                        var v12 = (byte)(data[5] >> 6);
                        data[5] *= 4;
                        data[5] |= v12;
                        var v22 = (byte)((data[6] >> 3) & 1);
                        if (((data[6] >> 5) & 1) != 0)
                        {
                            data[6] |= 8;
                        }
                        else
                        {
                            data[6] &= 0xF7;
                        }

                        if (v22 != 0)
                        {
                            data[6] |= 0x20;
                        }
                        else
                        {
                            data[6] &= 0xDF;
                        }

                        var v13 = data[7];
                        data[7] = data[4];
                        data[4] = v13;
                    }
                }
                else
                {
                    data[3] ^= 0xFD;
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v3 = (byte)(data[2] >> 1);
                    data[2] <<= 7;
                    data[2] |= v3;
                    data[2] ^= 0x9E;
                    var v4 = data[0];
                    data[0] = data[1];
                    data[1] = v4;
                    var v5 = data[0];
                    data[0] = data[2];
                    data[2] = v5;
                    var v6 = data[1];
                    data[1] = data[1];
                    data[1] = v6;
                    var v7 = data[2];
                    data[2] = data[0];
                    data[0] = v7;
                }
            }
        }
    }
}