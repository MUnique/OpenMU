// <copyright file="BotNameGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Threading;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Generates realistic-looking character names for bots.
/// </summary>
/// <remarks>
/// A population sharing one naming scheme is easy to spot, so each name is drawn in one of several
/// styles players actually use: pronounceable fantasy names built from syllables, compound gamer
/// names (e.g. "DarkBlade") and short handles (e.g. "xRavenx"). Every generated name satisfies the
/// default character name rules (3-10 alphanumeric characters) and contains no digits, so bots
/// blend in with players; the bot nature is tracked by <see cref="DataModel.Entities.Account.IsBot"/>,
/// never by the name. Names which look nearly the same as one handed out before - the same handle
/// with another decoration ("Hex", "Hexx", "xHexx"), names a letter or two apart ("Rex", "Vex";
/// "Brodan", "Broaedan") or compound names sharing a word ("DeadFang", "MadFang") - are rejected
/// too: they read like the alternative characters of one player.
/// </remarks>
internal sealed class BotNameGenerator
{
    /// <summary>
    /// The maximum length of a generated name, matching the default character name rules.
    /// </summary>
    internal const int MaxNameLength = 10;

    /// <summary>
    /// The number of failed attempts after which only the crowded fantasy form is drawn.
    /// </summary>
    private const int CrowdedAttemptThreshold = 30;

    /// <summary>
    /// The number of failed attempts after which a similar name is accepted after all: a nearly
    /// full name space must still produce a name.
    /// </summary>
    private const int SimilarityAttemptLimit = 400;

    /// <summary>
    /// The minimum length of two names which may not differ by just two letters. Two short handles
    /// two letters apart are different words ("Hex", "Max").
    /// </summary>
    private const int TwoLettersMinimumLength = 5;

    private static readonly string[] Starts =
    {
        "Dra", "Kar", "Mil", "Tho", "Zan", "Bel", "Gor", "Vyn", "Ael", "Mor",
        "Run", "Syl", "Tor", "Kra", "Fen", "Lyr", "Nyx", "Ori", "Var", "Eld",
        "Bro", "Cyn", "Dar", "Hal", "Ith", "Jor", "Kae", "Lor", "Mag", "Nor",
        "Pyr", "Rha", "Ser", "Ulr", "Wyn", "Xan", "Yor", "Zel", "Ari", "Cae",
    };

    private static readonly string[] Middles =
    {
        string.Empty, string.Empty, string.Empty,
        "a", "e", "i", "o", "ia", "ae", "an", "or", "el", "yn", "ar",
    };

    private static readonly string[] Ends =
    {
        "dor", "lin", "rik", "gar", "wyn", "ric", "mir", "dan", "eth", "ron",
        "ana", "ella", "ix", "ael", "oth", "ara", "une", "is", "ius", "wen",
    };

    /// <summary>
    /// The capital letters, which start the words of a compound name.
    /// </summary>
    private static readonly char[] UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    /// <summary>
    /// The non-empty middles, used by the crowded fantasy form to always add a syllable.
    /// </summary>
    private static readonly string[] CrowdedMiddles = Middles.Where(m => m.Length > 0).ToArray();

    /// <summary>
    /// Short endings appended in the crowded fantasy form to widen the name space without digits.
    /// </summary>
    private static readonly string[] CrowdedTails = { "a", "o", "is", "en", "ar", "on" };

    private static readonly string[] CompoundPrefixes =
    {
        "Dark", "Soul", "Storm", "Blood", "Shadow", "Fire", "Ice", "Night", "Iron", "Death",
        "Frost", "Doom", "Grim", "Rage", "Ghost", "Wild", "Hell", "Sky", "Steel", "Black",
        "Dead", "Silent", "Mad", "Red", "Holy", "Chaos", "Thunder", "Venom", "Lone", "Star",
        "Ash", "Bone", "Cold", "Cruel", "Dawn", "Dire", "Dread", "Dusk", "Ember", "Evil",
        "Fallen", "Fear", "Fell", "Flame", "Gold", "Grey", "Hollow", "Jade", "Last", "Light",
        "Lunar", "Moon", "Mystic", "Noble", "Pale", "Plague", "Pure", "Quick", "Rune", "Rust",
        "Sacred", "Sand", "Scar", "Shade", "Sharp", "Silver", "Sin", "Snow", "Solar", "Spell",
        "Stone", "Sun", "Swift", "True", "Twin", "Void", "War", "White", "Wind", "Winter",
        "Wise", "Brave", "Bright", "Burn", "Cursed", "Deep", "Demon", "Divine", "Dragon", "Dust",
        "Elder", "Fate", "Feral", "Flash", "Glory", "Grave", "Mighty", "High", "Lost", "Magic",
        "Metal", "Mist", "Mortal", "Nether", "Poison", "Prime", "Rebel", "Royal", "Ruby", "Sage",
        "Savage", "Skull", "Smoke", "Spark", "Stark", "Thorn", "Tide", "Titan", "Unholy", "Wrath",
    };

    private static readonly string[] CompoundSuffixes =
    {
        "Blade", "Hunter", "Knight", "Lord", "Slayer", "Fang", "Fury", "Wolf", "Mage", "Bane",
        "Claw", "Fist", "Heart", "Rider", "Born", "Eye", "Reaper", "Angel", "King", "Strike",
        "Hawk", "Bow", "Axe", "Shot", "Walker", "Storm", "Killer", "Wing", "Spirit", "Guard",
        "Arrow", "Beast", "Bear", "Boar", "Crow", "Drake", "Eagle", "Fox", "Lion", "Lynx",
        "Owl", "Snake", "Spider", "Tiger", "Wyrm", "Hound", "Bat", "Stag", "Bull", "Shark",
        "Archer", "Baron", "Bard", "Brute", "Chief", "Duke", "Druid", "Giant", "Hero", "Monk",
        "Priest", "Prince", "Queen", "Ranger", "Seer", "Shaman", "Smith", "Thief", "Warden", "Witch",
        "Wizard", "Keeper", "Seeker", "Stalker", "Weaver", "Breaker", "Crusher", "Bringer", "Caller", "Cutter",
        "Dancer", "Eater", "Hammer", "Lance", "Mace", "Spear", "Sword", "Staff", "Shield", "Edge",
        "Song", "Tooth", "Horn", "Hand", "Scale", "Warlock", "Vandal", "Lancer", "Saint", "Mauler",
        "Raider", "Brawler", "Ripper", "Viking", "Runner", "Templar", "Slinger", "Whip", "Fiend", "Ghoul",
        "Wraith", "Golem", "Troll", "Orc", "Elf", "Imp", "Lich", "Pyre", "Flare", "Bolt",
    };

    private static readonly string[] HandleWords =
    {
        "Raven", "Vex", "Nova", "Jinx", "Blaze", "Kuro", "Akira", "Zed", "Kira", "Toxic",
        "Rogue", "Viper", "Ghost", "Hex", "Onyx", "Rex", "Echo", "Pixel", "Zero", "Ninja",
        "Ace", "Bolt", "Cobra", "Kaiser", "Loki", "Odin", "Ronin", "Sakura", "Yuki", "Kenji",
        "Marco", "Diego", "Lucas", "Ivan", "Sven", "Elena", "Mika", "Aiden", "Hugo", "Leon",
        "Felix", "Oscar", "Nina", "Luna", "Mia", "Ryu", "Max", "Neo", "Tank", "Joker",
        "Bandit", "Psycho", "Kitsune", "Spike", "Dante", "Vlad", "Igor", "Hiro", "Shin", "Kai",
        "Axel", "Bruno", "Cain", "Ezra", "Faye", "Flux", "Gaia", "Gunner", "Hana", "Haru",
        "Iris", "Juno", "Kato", "Kenzo", "Koda", "Lexi", "Lyra", "Mako", "Maya", "Milo",
        "Momo", "Niko", "Orion", "Otto", "Pablo", "Quinn", "Remy", "Rhea", "Rio", "Rocco",
        "Saga", "Sasha", "Taro", "Theo", "Vega", "Wren", "Zane", "Zion", "Ziggy", "Glitch",
        "Gizmo", "Havoc", "Jester", "Karma", "Lucky", "Mamba", "Moose", "Nacho", "Nitro", "Panda",
        "Phoenix", "Pickle", "Pirate", "Pluto", "Pogo", "Prophet", "Puma", "Rocket", "Rusty", "Sarge",
        "Scout", "Sniper", "Sparky", "Spooky", "Static", "Turbo", "Vortex", "Wasabi", "Yeti", "Ozzy",
        "Taiga", "Hikaru", "Daiki", "Bjorn", "Erik", "Freya", "Astrid", "Ingrid", "Matteo", "Rafael",
        "Tiago", "Andre", "Yusuf", "Omar", "Tariq", "Amir", "Dmitri", "Boris", "Cosmo", "Falco",
        "Hector", "Jasper", "Logan", "Nash", "Orca", "Otis", "Ruben", "Silas", "Tobias", "Vince",
        "Diesel", "Fuzzy", "Jumbo", "Kiwi", "Ludo", "Mango", "Noodle", "Peanut", "Pepper", "Sushi",
        "Tofu", "Waffle",
    };

    private readonly IRandomizer _randomizer;

    /// <summary>
    /// The names which are already in the similarity index below.
    /// </summary>
    private readonly HashSet<string> _indexedNames = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The roots of the indexed names (see <see cref="GetRoot"/>) with each of their one-letter
    /// deletions: "Rexx" and "xHexx" are Rex and Hex, one letter apart.
    /// </summary>
    private readonly HashSet<string> _rootVariants = new(StringComparer.Ordinal);

    /// <summary>
    /// The indexed names with each of their one-letter deletions: two names one edit apart share one
    /// of these variants.
    /// </summary>
    private readonly HashSet<string> _oneLetterVariants = new(StringComparer.Ordinal);

    /// <summary>
    /// The indexed names of at least <see cref="TwoLettersMinimumLength"/> letters with each of their
    /// deletions of up to two letters: two such names two edits apart share one of these variants.
    /// </summary>
    private readonly HashSet<string> _twoLetterVariants = new(StringComparer.Ordinal);

    /// <summary>
    /// The first and last three letters of the indexed names of at least
    /// <see cref="TwoLettersMinimumLength"/> + 1 letters: "Fenelis" and "Fenelaelis" share them.
    /// </summary>
    private readonly HashSet<string> _frames = new(StringComparer.Ordinal);

    /// <summary>
    /// The words of the indexed compound names, e.g. "dead" and "fang" of "DeadFang": each word is used once.
    /// </summary>
    private readonly HashSet<string> _compoundWords = new(StringComparer.Ordinal);

    /// <summary>
    /// Initializes a new instance of the <see cref="BotNameGenerator"/> class.
    /// </summary>
    /// <param name="randomizer">The randomizer; the shared one when <c>null</c>.</param>
    public BotNameGenerator(IRandomizer? randomizer = null)
    {
        this._randomizer = randomizer ?? Rand.GetRandomizer();
    }

    /// <summary>
    /// The styles a bot name can be drawn in.
    /// </summary>
    internal enum NameStyle
    {
        /// <summary>
        /// A pronounceable name built from syllables, e.g. "Milynric".
        /// </summary>
        Fantasy,

        /// <summary>
        /// Two English words in camel case, e.g. "DarkBlade".
        /// </summary>
        Compound,

        /// <summary>
        /// A short handle or first name, optionally decorated, e.g. "Raven", "xKirax" or "TheHugo".
        /// </summary>
        Handle,
    }

    /// <summary>
    /// Generates a name which is not yet used, neither within this run nor in the database, and which
    /// doesn't look nearly the same as one of the names of this run (see <see cref="IsTooSimilar"/>).
    /// </summary>
    /// <param name="context">The context used to check name availability.</param>
    /// <param name="reserved">The set of names already handed out in this run; the returned name is added to it.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A unique, valid character name.</returns>
    public async ValueTask<string> GenerateUniqueAsync(IPlayerContext context, ISet<string> reserved, CancellationToken cancellationToken = default)
    {
        this.IndexNames(reserved);
        for (var attempt = 0; attempt < 500; attempt++)
        {
            var name = this.BuildName(attempt);
            if ((attempt < SimilarityAttemptLimit && this.IsTooSimilar(name))
                || !reserved.Add(name))
            {
                continue;
            }

            this.IndexName(name);

            var existing = await context.GetAccountByCharacterNameAsync(name, cancellationToken).ConfigureAwait(false);
            if (existing is null)
            {
                return name;
            }
        }

        throw new InvalidOperationException("Could not generate a unique bot character name after many attempts.");
    }

    /// <summary>
    /// Gets the root of a name: the name in lower case without the decorations of a handle - "i", "x...x",
    /// "The", "Mr" or "Lil" around it, an "x" or "z" appended, or its last letter doubled.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The root, e.g. "hex" for "Hexx", "xHexx" and "MrHex".</returns>
    internal static string GetRoot(string name)
    {
        var root = name;
        foreach (var prefix in new[] { "The", "Mr", "Lil", "i", "x" })
        {
            // Only a decoration when a capitalized word follows - "Ithwyn" and "Xanoth" stay as they are.
            if (root.Length > prefix.Length + 2
                && root.StartsWith(prefix, StringComparison.Ordinal)
                && char.IsUpper(root[prefix.Length]))
            {
                root = root[prefix.Length..];
                break;
            }
        }

        root = root.ToLowerInvariant();
        if (root.Length > 3 && root[^1] is 'x' or 'z')
        {
            root = root[..^1];
        }

        if (root.Length > 3 && root[^1] == root[^2])
        {
            root = root[..^1];
        }

        return root;
    }

    /// <summary>
    /// Builds a name candidate for the given attempt.
    /// </summary>
    /// <param name="attempt">The number of candidates already rejected for this name.</param>
    /// <returns>The name candidate.</returns>
    internal string BuildName(int attempt)
    {
        // The compound and handle name spaces are small, so a crowded population falls back to
        // the widest form instead of appending digits.
        if (attempt > CrowdedAttemptThreshold)
        {
            return this.BuildCrowdedFantasyName();
        }

        return this.BuildName(this.PickStyle());
    }

    /// <summary>
    /// Builds a name candidate in the given style.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <returns>The name candidate.</returns>
    internal string BuildName(NameStyle style)
    {
        return style switch
        {
            NameStyle.Compound => this.BuildCompoundName(),
            NameStyle.Handle => this.BuildHandleName(),
            _ => this.BuildFantasyName(),
        };
    }

    /// <summary>
    /// Picks the style of the next name: 40 % fantasy, 30 % compound, 30 % handle.
    /// </summary>
    /// <returns>The picked style.</returns>
    internal NameStyle PickStyle()
    {
        var roll = this._randomizer.NextInt(0, 100);
        return roll switch
        {
            < 40 => NameStyle.Fantasy,
            < 70 => NameStyle.Compound,
            _ => NameStyle.Handle,
        };
    }

    /// <summary>
    /// Determines whether the name looks nearly the same as one of the indexed names: the same root or
    /// roots one letter apart, one letter apart, two letters apart from a name of at least five letters,
    /// the same first and last three letters, or a word of a compound name which is already used.
    /// </summary>
    /// <param name="name">The name candidate.</param>
    /// <returns><c>true</c>, if the name is too similar.</returns>
    internal bool IsTooSimilar(string name)
    {
        if (GetDeletions(GetRoot(name), 1).Any(this._rootVariants.Contains))
        {
            return true;
        }

        if (GetCompoundWords(name).Any(this._compoundWords.Contains))
        {
            return true;
        }

        var lower = name.ToLowerInvariant();
        if (GetDeletions(lower, 1).Any(this._oneLetterVariants.Contains))
        {
            return true;
        }

        if (GetFrame(lower) is { } frame && this._frames.Contains(frame))
        {
            return true;
        }

        return lower.Length >= TwoLettersMinimumLength
               && GetDeletions(lower, 2).Any(this._twoLetterVariants.Contains);
    }

    /// <summary>
    /// Adds the names to the similarity index, if they aren't in it yet - the names which were reserved
    /// before this call, e.g. by an earlier call for another bot of the same run.
    /// </summary>
    /// <param name="names">The names.</param>
    internal void IndexNames(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            this.IndexName(name);
        }
    }

    /// <summary>
    /// Gets the first and last three letters of a name of at least six letters, e.g. "fen-lis" of
    /// "fenelis"; <c>null</c> for a shorter name.
    /// </summary>
    private static string? GetFrame(string lowerName)
        => lowerName.Length > TwoLettersMinimumLength ? $"{lowerName[..3]}-{lowerName[^3..]}" : null;

    /// <summary>
    /// Gets the two words of a compound name in lower case, e.g. "dead" and "fang" of "DeadFang"; none
    /// for a name with one capital letter.
    /// </summary>
    private static string[] GetCompoundWords(string name)
    {
        var second = name.Length > 1 ? name.IndexOfAny(UpperCaseLetters, 1) : -1;
        return second <= 0
            ? []
            : [name[..second].ToLowerInvariant(), name[second..].ToLowerInvariant()];
    }

    /// <summary>
    /// Gets the text and every text which results from deleting up to the given number of letters.
    /// </summary>
    private static HashSet<string> GetDeletions(string text, int maximumDeletions)
    {
        var result = new HashSet<string>(StringComparer.Ordinal) { text };
        var current = new List<string> { text };
        for (var deletion = 0; deletion < maximumDeletions; deletion++)
        {
            var next = new List<string>();
            foreach (var variant in current)
            {
                for (var i = 0; i < variant.Length; i++)
                {
                    var shorter = variant.Remove(i, 1);
                    if (result.Add(shorter))
                    {
                        next.Add(shorter);
                    }
                }
            }

            current = next;
        }

        return result;
    }

    /// <summary>
    /// Brings a name into the "Capitalized" form and cuts it to the maximum length.
    /// </summary>
    private static string Capitalize(string name)
    {
        if (name.Length > MaxNameLength)
        {
            name = name[..MaxNameLength];
        }

        return char.ToUpperInvariant(name[0]) + name[1..].ToLowerInvariant();
    }

    private string Pick(string[] values) => values.SelectRandom(this._randomizer)!;

    private string BuildFantasyName()
    {
        return Capitalize(this.Pick(Starts) + this.Pick(Middles) + this.Pick(Ends));
    }

    private string BuildCrowdedFantasyName()
    {
        return Capitalize(this.Pick(Starts) + this.Pick(CrowdedMiddles) + this.Pick(Ends) + this.Pick(CrowdedTails));
    }

    private string BuildCompoundName()
    {
        var prefix = this.Pick(CompoundPrefixes);

        // Only suffixes which keep the name within the limit, so no word gets cut in half.
        var fitting = CompoundSuffixes.Where(s => prefix.Length + s.Length <= MaxNameLength && s != prefix).ToArray();
        return prefix + this.Pick(fitting);
    }

    private string BuildHandleName()
    {
        var word = this.Pick(HandleWords);

        // Roughly half of the handles stay plain; the rest get a decoration players commonly use.
        var decorated = this._randomizer.NextInt(0, 16) switch
        {
            0 => "x" + word + "x",
            1 => word + "x",
            2 => word + "z",
            3 => "i" + word,
            4 => "The" + word,
            5 => "Mr" + word,
            6 => "Lil" + word,
            7 => word + char.ToLowerInvariant(word[^1]),
            _ => word,
        };

        return decorated.Length <= MaxNameLength ? decorated : word;
    }

    private void IndexName(string name)
    {
        if (!this._indexedNames.Add(name))
        {
            return;
        }

        this._rootVariants.UnionWith(GetDeletions(GetRoot(name), 1));
        this._compoundWords.UnionWith(GetCompoundWords(name));
        var lower = name.ToLowerInvariant();
        if (GetFrame(lower) is { } frame)
        {
            this._frames.Add(frame);
        }

        this._oneLetterVariants.UnionWith(GetDeletions(lower, 1));
        if (lower.Length >= TwoLettersMinimumLength)
        {
            this._twoLetterVariants.UnionWith(GetDeletions(lower, 2));
        }
    }
}
