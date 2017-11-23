// <copyright file="GameServerDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    /// <summary>
    /// Defines the configuration of a game server.
    /// </summary>
    public class GameServerDefinition
    {
        /// <summary>
        /// Gets or sets the server identifier.
        /// </summary>
        public byte ServerID { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the network port on which the server is listening.
        /// </summary>
        public int NetworkPort { get; set; }

        /// <summary>
        /// Gets or sets the server configuration.
        /// </summary>
        public virtual GameServerConfiguration ServerConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the game configuration.
        /// </summary>
        public virtual GameConfiguration GameConfiguration { get; set; }
        /*
        private IEnumerable<IJewelMix> GetMixes()
        {
            yield return GetJewelMix(0, 13, 0xE, 30, 0xC); //Bless
            yield return GetJewelMix(1, 14, 0xE, 31, 0xC); //Soul
            yield return GetJewelMix(2, 16, 0xE, 136, 0xC); //Jol
            yield return GetJewelMix(3, 22, 0xE, 137, 0xC); //JoC
            yield return GetJewelMix(4, 31, 0xE, 138, 0xC); //Jewel of Guardian
            yield return GetJewelMix(5, 41, 0xE, 139, 0xC); //gemstones 139
            yield return GetJewelMix(6, 42, 0xE, 140, 0xC); //Joh 140
            yield return GetJewelMix(7, 15, 0xC, 141, 0xC); //Chaos
            yield return GetJewelMix(8, 43, 0xE, 142, 0xC); //Lower Refine Stone
            yield return GetJewelMix(9, 44, 0xE, 143, 0xC); //Higher Refine Stone
                        /*
            new JewelMix(){SingleJewelID=13, SingleJewelGroup=0xE, MixedJewelID=30, MixedJewelGroup=0xC0}, //Bless
            new JewelMix(){SingleJewelID=14, SingleJewelGroup=0xE, MixedJewelID=31, MixedJewelGroup=0xC0}, //Soul
            new JewelMix(){SingleJewelID=16, SingleJewelGroup=0xE, MixedJewelID=136, MixedJewelGroup=0xC0}, //Jol
            new JewelMix(){SingleJewelID=22, SingleJewelGroup=0xE, MixedJewelID=137, MixedJewelGroup=0xC0}, //JoC
            new JewelMix(){SingleJewelID=31, SingleJewelGroup=0xE, MixedJewelID=138, MixedJewelGroup=0xC0}, //Jewel of Guardian
            new JewelMix(){SingleJewelID=41, SingleJewelGroup=0xE, MixedJewelID=139, MixedJewelGroup=0xC0}, //gemstones 139
            new JewelMix(){SingleJewelID=42, SingleJewelGroup=0xE, MixedJewelID=140, MixedJewelGroup=0xC0}, //Joh 140
            new JewelMix(){SingleJewelID=15, SingleJewelGroup=0xC, MixedJewelID=141, MixedJewelGroup=0xC0}, //Chaos
            new JewelMix(){SingleJewelID=43, SingleJewelGroup=0xE, MixedJewelID=142, MixedJewelGroup=0xC0}, //Lower Refine Stone
            new JewelMix(){SingleJewelID=44, SingleJewelGroup=0xE, MixedJewelID=143, MixedJewelGroup=0xC0}, //Higher Refine Stone
         *
        }*/

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("[GameServerDefinition ServerID={0}, Description={1}, NetworkPort={2}]", this.ServerID, this.Description, this.NetworkPort);
        }
    }
}
