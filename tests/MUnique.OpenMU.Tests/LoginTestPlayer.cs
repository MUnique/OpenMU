// <copyright file="LoginTestPlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A player for the tests of the login, without a network connection; its view plugins are mocks.
/// </summary>
internal class LoginTestPlayer : Player
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginTestPlayer"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    public LoginTestPlayer(IGameContext gameContext)
        : base(gameContext)
    {
    }

    /// <inheritdoc />
    protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
    {
        return new MockViewPlugInContainer();
    }
}
