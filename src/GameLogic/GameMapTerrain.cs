// <copyright file="GameMapTerrain.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.IO;
    using log4net;

    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// The terrain of a map.
    /// </summary>
    public class GameMapTerrain
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameMapTerrain));

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMapTerrain"/> class.
        /// </summary>
        /// <param name="definition">The game map definition.</param>
        public GameMapTerrain(GameMapDefinition definition)
        {
            if (definition.TerrainData == null)
            {
                Log.Warn($"Terrain data for {definition.Name} not defined.");
                return;
            }

            using (var memoryStream = new MemoryStream(definition.TerrainData))
            {
                Log.Debug($"Start reading terrain data for {definition.Name}.");
                this.ReadTerrainData(memoryStream);
                Log.Debug($"Finished reading terrain data for {definition.Name}.");
            }
        }

        /// <summary>
        /// Gets a grid of all safezone coordinates.
        /// </summary>
        public bool[,] SafezoneMap { get; private set; }

        /// <summary>
        /// Gets a grid of all walkable coordinates.
        /// </summary>
        public bool[,] WalkMap { get; private set; }

        /// <summary>
        /// Gets a grid of the walkable coordinates of monsters.
        /// </summary>
        public byte[,] AIgrid { get; private set; }

        /// <summary>
        /// Gets a random drop coordinate at the specified point in the specified radius.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="maxmimumRadius">The maximum radius around the specifeied coordinate.</param>
        /// <returns>The random drop coordinate.</returns>
        public Point GetRandomDropCoordinate(byte x, byte y, byte maxmimumRadius)
        {
            byte tempx = (byte)Rand.NextInt(Math.Max(0, x - maxmimumRadius), Math.Min(255, x + maxmimumRadius + 1));
            byte tempy = (byte)Rand.NextInt(Math.Max(0, y - maxmimumRadius), Math.Min(255, y + maxmimumRadius + 1));
            int i = 0;
            while (!this.WalkMap[tempx, tempy] && i < 20)
            {
                tempx = (byte)Rand.NextInt(Math.Max(0, x - maxmimumRadius), Math.Min(255, x + maxmimumRadius + 1));
                tempy = (byte)Rand.NextInt(Math.Max(0, y - maxmimumRadius), Math.Min(255, y + maxmimumRadius + 1));
                i++;
            }

            if (i == 20)
            {
                return new Point(x, y);
            }

            return new Point(tempx, tempy);
        }

        /// <summary>
        /// Reads the terrain data from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        private void ReadTerrainData(Stream stream)
        {
            this.WalkMap = new bool[256, 256];
            this.SafezoneMap = new bool[256, 256];
            this.AIgrid = new byte[256, 256];
            stream.ReadByte();
            stream.ReadByte();
            stream.ReadByte();
            var data = new byte[0x10000];
            stream.Read(data, 0, 0x10000);
            for (int i = 0; i < data.Length; i++)
            {
                byte x = (byte)(i & 0xFF);
                byte y = (byte)((i >> 8) & 0xFF);
                byte value = data[i];
                this.WalkMap[x, y] = value == 0 || value == 1;
                this.SafezoneMap[x, y] = value == 1;
                this.AIgrid[x, y] = Convert.ToByte(this.WalkMap[x, y] && !this.SafezoneMap[x, y]);
            }
        }
    }
}
