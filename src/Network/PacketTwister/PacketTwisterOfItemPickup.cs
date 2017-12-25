// <copyright file="PacketTwisterOfItemPickup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'ItemPickup' type.
    /// </summary>
    internal class PacketTwisterOfItemPickup : IPacketTwister
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
                            var v19 = data[29];
                            data[29] = data[11];
                            data[11] = v19;
                            data[14] ^= 0x81;
                            var v20 = (byte)(data[4] >> 6);
                            data[4] *= 4;
                            data[4] |= v20;
                            var v21 = data[21];
                            data[21] = data[29];
                            data[29] = v21;
                            var v22 = data[20];
                            data[20] = data[18];
                            data[18] = v22;
                            data[28] ^= 0x85;
                            data[11] ^= 0x53;
                            data[18] ^= 0xAB;
                            var v23 = data[0];
                            data[0] = data[4];
                            data[4] = v23;
                            data[17] ^= 0x5B;
                        }
                        else
                        {
                            var v14 = data[6];
                            data[6] = data[4];
                            data[4] = v14;
                            var v15 = (byte)(data[4] >> 4);
                            data[4] *= 16;
                            data[4] |= v15;
                            data[4] ^= 0x59;
                            data[5] ^= 0xA0;
                            data[15] ^= 0x89;
                            var v16 = (byte)(data[11] >> 3);
                            data[11] *= 32;
                            data[11] |= v16;
                            var v17 = data[10];
                            data[10] = data[10];
                            data[10] = v17;
                            var v18 = (byte)(data[10] >> 5);
                            data[10] *= 8;
                            data[10] |= v18;
                        }
                    }
                    else
                    {
                        data[6] ^= 0x18;
                        data[2] ^= 8;
                        var v8 = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= v8;
                        data[3] ^= 0x5F;
                        var v9 = (byte)(data[1] >> 2);
                        data[1] <<= 6;
                        data[1] |= v9;
                        var v27 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 1) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v27 != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        var v26 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 3) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v26 != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        var v10 = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= v10;
                        var v25 = (byte)((data[0] >> 2) & 1);
                        if (((data[0] >> 7) & 1) != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        if (v25 != 0)
                        {
                            data[0] |= 0x80;
                        }
                        else
                        {
                            data[0] &= 0x7F;
                        }

                        var v11 = (byte)(data[5] >> 1);
                        data[5] <<= 7;
                        data[5] |= v11;
                        var v12 = data[1];
                        data[1] = data[4];
                        data[4] = v12;
                        var v24 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 1) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v24 != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        var v13 = (byte)(data[6] >> 6);
                        data[6] *= 4;
                        data[6] |= v13;
                    }
                }
                else
                {
                    data[0] ^= 0xFA;
                    data[1] ^= 0x8A;
                    var v3 = data[3];
                    data[3] = data[0];
                    data[0] = v3;
                    var v4 = data[1];
                    data[1] = data[0];
                    data[0] = v4;
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v31 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v31 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v5 = (byte)(data[3] >> 1);
                    data[3] <<= 7;
                    data[3] |= v5;
                    var v6 = data[0];
                    data[0] = data[2];
                    data[2] = v6;
                    data[2] ^= 0xEC;
                    var v7 = (byte)(data[2] >> 3);
                    data[2] *= 32;
                    data[2] |= v7;
                    var v30 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v30 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    var v29 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v29 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v28 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v28 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }
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
                            data[17] ^= 0x5B;
                            var v19 = data[0];
                            data[0] = data[4];
                            data[4] = v19;
                            data[18] ^= 0xAB;
                            data[11] ^= 0x53;
                            data[28] ^= 0x85;
                            var v20 = data[20];
                            data[20] = data[18];
                            data[18] = v20;
                            var v21 = data[21];
                            data[21] = data[29];
                            data[29] = v21;
                            var v22 = (byte)(data[4] >> 2);
                            data[4] <<= 6;
                            data[4] |= v22;
                            data[14] ^= 0x81;
                            var v23 = data[29];
                            data[29] = data[11];
                            data[11] = v23;
                        }
                        else
                        {
                            var v14 = (byte)(data[10] >> 3);
                            data[10] *= 32;
                            data[10] |= v14;
                            var v15 = data[10];
                            data[10] = data[10];
                            data[10] = v15;
                            var v16 = (byte)(data[11] >> 5);
                            data[11] *= 8;
                            data[11] |= v16;
                            data[15] ^= 0x89;
                            data[5] ^= 0xA0;
                            data[4] ^= 0x59;
                            var v17 = (byte)(data[4] >> 4);
                            data[4] *= 16;
                            data[4] |= v17;
                            var v18 = data[6];
                            data[6] = data[4];
                            data[4] = v18;
                        }
                    }
                    else
                    {
                        var v8 = (byte)(data[6] >> 2);
                        data[6] <<= 6;
                        data[6] |= v8;
                        var v27 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 1) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v27 != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        var v9 = data[1];
                        data[1] = data[4];
                        data[4] = v9;
                        var v10 = (byte)(data[5] >> 7);
                        data[5] *= 2;
                        data[5] |= v10;
                        var v26 = (byte)((data[0] >> 2) & 1);
                        if (((data[0] >> 7) & 1) != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        if (v26 != 0)
                        {
                            data[0] |= 0x80;
                        }
                        else
                        {
                            data[0] &= 0x7F;
                        }

                        var v11 = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= v11;
                        var v25 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 3) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v25 != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        var v24 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 1) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v24 != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        var v12 = (byte)(data[1] >> 6);
                        data[1] *= 4;
                        data[1] |= v12;
                        data[3] ^= 0x5F;
                        var v13 = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= v13;
                        data[2] ^= 8;
                        data[6] ^= 0x18;
                    }
                }
                else
                {
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

                    var v31 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v31 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v30 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v30 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    var v3 = (byte)(data[2] >> 5);
                    data[2] *= 8;
                    data[2] |= v3;
                    data[2] ^= 0xEC;
                    var v4 = data[0];
                    data[0] = data[2];
                    data[2] = v4;
                    var v5 = (byte)(data[3] >> 7);
                    data[3] *= 2;
                    data[3] |= v5;
                    var v29 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v29 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v28 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v28 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v6 = data[1];
                    data[1] = data[0];
                    data[0] = v6;
                    var v7 = data[3];
                    data[3] = data[0];
                    data[0] = v7;
                    data[1] ^= 0x8A;
                    data[0] ^= 0xFA;
                }
            }
        }
    }
}