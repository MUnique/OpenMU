// <copyright file="BotStartupProfile.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Describes the state a freshly generated bot character is born into: its start level, its seeded
/// reset history and the upgrade level of its starter equipment. The <see cref="BotGenerator"/> asks
/// the chosen profile for these values instead of deciding itself, so adding a new kind of bot
/// population (e.g. a mid-level one between the veteran and the fresh extremes) means adding one
/// class - the generator does not change.
/// </summary>
internal abstract class BotStartupProfile
{
    private const byte FreshStarterItemLevel = 0;

    /// <summary>
    /// Gets the lowest level a character of this profile may be generated with.
    /// </summary>
    public abstract int MinLevel { get; }

    /// <summary>
    /// Gets the highest level a character of this profile may be generated with.
    /// </summary>
    public abstract int MaxLevel { get; }

    /// <summary>
    /// Gets the upgrade level of the starter gear a character of this profile is equipped with.
    /// </summary>
    public abstract byte StarterItemLevel { get; }

    /// <summary>
    /// Creates the startup profile corresponding to the <see cref="BotConfiguration.StartAsFreshCharacters"/>
    /// flag of the bot feature.
    /// </summary>
    /// <param name="startAsFreshCharacters">Whether newly generated bots should start as fresh characters.</param>
    /// <returns>The profile to generate the bot population with.</returns>
    public static BotStartupProfile For(bool startAsFreshCharacters)
        => startAsFreshCharacters ? new FreshStartupProfile() : new VeteranStartupProfile();

    /// <summary>
    /// Rolls the character's start level.
    /// </summary>
    /// <param name="minLevel">The lowest generated level (inclusive).</param>
    /// <param name="maxLevel">The highest generated level (inclusive).</param>
    /// <returns>The character's start level.</returns>
    public abstract int GetStartLevel(int minLevel, int maxLevel);

    /// <summary>
    /// Determines the seeded reset history of the character (see <see cref="BotGenerator"/> for why
    /// existing populations are seeded with a believable reset history on reset servers).
    /// </summary>
    /// <param name="maxSeededResets">The highest number of resets a generated character may seed; 0 disables seeding.</param>
    /// <returns>The number of resets the character starts with.</returns>
    public abstract int GetSeededResets(int maxSeededResets);

    /// <summary>
    /// Generates bots as fresh level-1 characters: the way a regular player's newly created
    /// character looks. Level 1, no reset history and level-0 starter equipment; everything else
    /// (level-up points, skills, class evolution) follows from that state in <see cref="BotGenerator"/>.
    /// </summary>
    private sealed class FreshStartupProfile : BotStartupProfile
    {
        /// <inheritdoc />
        public override int MinLevel => 1;

        /// <inheritdoc />
        public override int MaxLevel => 1;

        /// <inheritdoc />
        public override byte StarterItemLevel => FreshStarterItemLevel;

        /// <inheritdoc />
        public override int GetStartLevel(int minLevel, int maxLevel)
        {
            // A fresh character is always level 1, regardless of the veteran-level bounds.
            return 1;
        }

        /// <inheritdoc />
        public override int GetSeededResets(int maxSeededResets)
        {
            // A fresh character has no history.
            return 0;
        }
    }

    /// <summary>
    /// Generates bots as believable veterans, the way the population has always been generated:
    /// a random start level skewed towards the low/mid range and, on reset servers, a random
    /// reset history so visitors meet a realistic mix of characters.
    /// </summary>
    private sealed class VeteranStartupProfile : BotStartupProfile
    {
        /// <summary>The lowest generated level.</summary>
        private const int VeteranMinLevel = 10;

        /// <summary>
        /// The highest generated level. High enough that the upper maps (Tarkan, Aida, Kanturu, ...) get a
        /// resident bot population and that some bots start beyond the class evolution level
        /// (<see cref="BotProgression.ClassEvolutionLevel"/>) - those are created as their second-generation
        /// class right away, like a player who did the class quest long ago.
        /// </summary>
        private const int VeteranMaxLevel = 250;

        /// <summary>
        /// Skew of the level distribution: values above 1 make low and mid levels more common than high
        /// ones, like a real server's population pyramid (an even spread would feel top-heavy).
        /// </summary>
        private const double LevelSkew = 1.6;

        /// <summary>Upgrade level (+6) of the starter gear, giving fresh bots a survival buffer until they can warp.</summary>
        private const byte VeteranStarterItemLevel = 6;

        /// <inheritdoc />
        public override int MinLevel => VeteranMinLevel;

        /// <inheritdoc />
        public override int MaxLevel => VeteranMaxLevel;

        /// <inheritdoc />
        public override byte StarterItemLevel => VeteranStarterItemLevel;

        /// <inheritdoc />
        public override int GetStartLevel(int minLevel, int maxLevel)
        {
            return minLevel + (int)((maxLevel - minLevel) * Math.Pow(Rand.NextInt(0, 1001) / 1000.0, LevelSkew));
        }

        /// <inheritdoc />
        public override int GetSeededResets(int maxSeededResets)
        {
            return maxSeededResets > 0 ? Rand.NextInt(0, maxSeededResets + 1) : 0;
        }
    }
}