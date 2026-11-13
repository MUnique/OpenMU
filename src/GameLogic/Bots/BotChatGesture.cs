// <copyright file="BotChatGesture.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// A gesture a bot makes with a line, like a player who types /hello or /clap: waving with a
/// greeting, clapping with a "gz".
/// </summary>
/// <remarks>
/// The values are the animation numbers of the game client's emotes (the actions which follow
/// <c>AT_SIT1</c> = 128, the "sitting" animation the server already knows), so they must match the
/// client and can't be configured freely. They are stored in the configuration as numbers.
/// </remarks>
public enum BotChatGesture : byte
{
    /// <summary>No gesture.</summary>
    None = 0,

    /// <summary>Waving a greeting.</summary>
    Greeting = 131,

    /// <summary>Waving goodbye.</summary>
    Goodbye = 132,

    /// <summary>Clapping.</summary>
    Clap = 133,

    /// <summary>Crying.</summary>
    Cry = 137,

    /// <summary>Cheering.</summary>
    Cheer = 138,

    /// <summary>An awkward shrug.</summary>
    Awkward = 139,

    /// <summary>A victory pose.</summary>
    Win = 141,

    /// <summary>Smiling.</summary>
    Smile = 142,

    /// <summary>Bowing in respect.</summary>
    Respect = 146,

    /// <summary>A salute.</summary>
    Salute = 147,

    /// <summary>A provocation - angry.</summary>
    Provocation = 153,
}
