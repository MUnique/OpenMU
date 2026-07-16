// <copyright file="BotNameGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using System.Threading;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Generates pronounceable, realistic-looking character names for bots.
/// </summary>
/// <remarks>
/// Names are built procedurally from syllables so the generator scales to thousands of unique
/// names while still looking like names a real player might pick. Every generated name satisfies
/// the default character name rules (3-10 alphanumeric characters), so bots blend in with players;
/// the bot nature is tracked by <see cref="DataModel.Entities.Account.IsBot"/>, never by the name.
/// </remarks>
internal sealed class BotNameGenerator
{
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

    private readonly IRandomizer _randomizer = Rand.GetRandomizer();

    /// <summary>
    /// Generates a name which is not yet used, neither within this run nor in the database.
    /// </summary>
    /// <param name="context">The context used to check name availability.</param>
    /// <param name="reserved">The set of names already handed out in this run; the returned name is added to it.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A unique, valid character name.</returns>
    public async ValueTask<string> GenerateUniqueAsync(IPlayerContext context, ISet<string> reserved, CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < 500; attempt++)
        {
            var name = this.BuildName(attempt);
            if (!reserved.Add(name))
            {
                continue;
            }

            var existing = await context.GetAccountByCharacterNameAsync(name, cancellationToken).ConfigureAwait(false);
            if (existing is null)
            {
                return name;
            }
        }

        throw new InvalidOperationException("Could not generate a unique bot character name after many attempts.");
    }

    private string BuildName(int attempt)
    {
        var start = Starts.SelectRandom(this._randomizer)!;
        var middle = Middles.SelectRandom(this._randomizer)!;
        var end = Ends.SelectRandom(this._randomizer)!;
        var name = start + middle + end;

        // Once the simple name space gets crowded, append a digit to keep finding free names
        // without ever exceeding the 10 character limit.
        if (attempt > 30)
        {
            var suffix = (attempt % 10).ToString(CultureInfo.InvariantCulture);
            name = (name.Length >= 10 ? name[..9] : name) + suffix;
        }
        else if (name.Length > 10)
        {
            name = name[..10];
        }

        return char.ToUpperInvariant(name[0]) + name[1..].ToLowerInvariant();
    }
}
