// <copyright file="GameMapTerrain.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
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
            : this(definition.Name, definition.TerrainData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMapTerrain"/> class.
        /// </summary>
        /// <param name="mapName">Name of the map.</param>
        /// <param name="terrainData">The terrain data.</param>
        public GameMapTerrain(string mapName, byte[] terrainData)
        {
            if (terrainData == null)
            {
                Log.Warn($"Terrain data for {mapName} not defined.");
                return;
            }

            Log.Debug($"Start reading terrain data for {mapName}.");
            this.ReadTerrainData(terrainData.AsSpan(3));
            Log.Debug($"Finished reading terrain data for {mapName}.");
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
        /// <param name="point">The target point.</param>
        /// <param name="maxmimumRadius">The maximum radius around the specified coordinate.</param>
        /// <returns>The random drop coordinate.</returns>
        public Point GetRandomDropCoordinate(Point point, byte maxmimumRadius)
        {
            byte tempx = (byte)Rand.NextInt(Math.Max(0, point.X - maxmimumRadius), Math.Min(255, point.X + maxmimumRadius + 1));
            byte tempy = (byte)Rand.NextInt(Math.Max(0, point.Y - maxmimumRadius), Math.Min(255, point.Y + maxmimumRadius + 1));
            int i = 0;
            while (!this.WalkMap[tempx, tempy] && i < 20)
            {
                tempx = (byte)Rand.NextInt(Math.Max(0, point.X - maxmimumRadius), Math.Min(255, point.X + maxmimumRadius + 1));
                tempy = (byte)Rand.NextInt(Math.Max(0, point.Y - maxmimumRadius), Math.Min(255, point.Y + maxmimumRadius + 1));
                i++;
            }

            if (i == 20)
            {
                return point;
            }

            return new Point(tempx, tempy);
        }

        /// <summary>
        /// Reads the terrain data from a stream.
        /// </summary>
        /// <param name="data">The data.</param>
        private void ReadTerrainData(ReadOnlySpan<byte> data)
        {
            this.WalkMap = new bool[256, 256];
            this.SafezoneMap = new bool[256, 256];
            this.AIgrid = new byte[256, 256];

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
