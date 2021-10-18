// <copyright file="DevilSquareInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Events
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// The initializer for the devil square event.
    /// </summary>
    internal class DevilSquareInitializer : InitializerBase
    {
        /// <summary>
        /// The client-side map number of the map of devil square 1 to 4.
        /// </summary>
        private const short DevilSquare1To4Number = 9;

        /// <summary>
        /// The client-side map number of the map of devil square 5 to 7.
        /// </summary>
        private const short DevilSquare5To7Number = 32;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevilSquareInitializer" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public DevilSquareInitializer(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            var devilSquare1 = this.CreateDevilSquareDefinition(1);
            devilSquare1.MinimumCharacterLevel = 15;
            devilSquare1.MaximumCharacterLevel = 130;
            devilSquare1.MinimumSpecialCharacterLevel = 10;
            devilSquare1.MaximumSpecialCharacterLevel = 110;

            var devilSquare2 = this.CreateDevilSquareDefinition(2);
            devilSquare2.MinimumCharacterLevel = 131;
            devilSquare2.MaximumCharacterLevel = 180;
            devilSquare2.MinimumSpecialCharacterLevel = 111;
            devilSquare2.MaximumSpecialCharacterLevel = 160;

            var devilSquare3 = this.CreateDevilSquareDefinition(3);
            devilSquare3.MinimumCharacterLevel = 181;
            devilSquare3.MaximumCharacterLevel = 230;
            devilSquare3.MinimumSpecialCharacterLevel = 161;
            devilSquare3.MaximumSpecialCharacterLevel = 210;

            var devilSquare4 = this.CreateDevilSquareDefinition(4);
            devilSquare4.MinimumCharacterLevel = 231;
            devilSquare4.MaximumCharacterLevel = 280;
            devilSquare4.MinimumSpecialCharacterLevel = 211;
            devilSquare4.MaximumSpecialCharacterLevel = 260;
        }

        /// <summary>
        /// Creates a new <see cref="MiniGameDefinition"/> for a devil square event.
        /// </summary>
        /// <param name="level">THe level of the event.</param>
        /// <returns>The created <see cref="MiniGameDefinition"/>.</returns>
        protected MiniGameDefinition CreateDevilSquareDefinition(byte level)
        {
            var devilSquare = this.Context.CreateNew<MiniGameDefinition>();
            this.GameConfiguration.MiniGameDefinitions.Add(devilSquare);
            devilSquare.Name = $"Devil Square {level}";
            devilSquare.Description = $"Event definition for devil square event, level {level}.";
            devilSquare.EnterDuration = TimeSpan.FromMinutes(1);
            devilSquare.GameDuration = TimeSpan.FromMinutes(20);
            devilSquare.ExitDuration = TimeSpan.FromMinutes(3);
            devilSquare.MaximumPlayerCount = 5;
            var mapNumber = level < 5 ? DevilSquare1To4Number : DevilSquare5To7Number;
            devilSquare.Entrance = this.GameConfiguration.Maps.First(m => m.Number == mapNumber && m.Discriminator == level).ExitGates.Single();
            devilSquare.Type = MiniGameType.DevilSquare;
            devilSquare.TicketItem = this.GameConfiguration.Items.Single(item => item.Group == 14 && item.Number == 19);
            devilSquare.TicketItemLevel = level;
            devilSquare.GameLevel = level;
            devilSquare.MapCreationPolicy = MiniGameMapCreationPolicy.OnePerParty;
            devilSquare.SaveRankingStatistics = true;
            return devilSquare;
        }
    }
}
