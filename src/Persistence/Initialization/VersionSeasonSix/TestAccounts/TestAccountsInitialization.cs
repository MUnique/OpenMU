// <copyright file="TestAccountsInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initializes the test accounts.
/// </summary>
public class TestAccountsInitialization : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestAccountsInitialization"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public TestAccountsInitialization(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        for (int i = 0; i < 10; i++)
        {
            var level = (i * 10) + 1;
            new TestAccounts.LowLevel(this.Context, this.GameConfiguration, "test" + i, level).Initialize();
        }

        new TestAccounts.Level300(this.Context, this.GameConfiguration).Initialize();
        new TestAccounts.Level400(this.Context, this.GameConfiguration).Initialize();
        new TestAccounts.Ancient(this.Context, this.GameConfiguration).Initialize();
        new TestAccounts.Socket(this.Context, this.GameConfiguration).Initialize();
        new TestAccounts.Quest150(this.Context, this.GameConfiguration).Initialize();
        new TestAccounts.Quest220(this.Context, this.GameConfiguration).Initialize();
        new TestAccounts.Quest400(this.Context, this.GameConfiguration).Initialize();
        new TestAccounts.GameMaster(this.Context, this.GameConfiguration).Initialize();
        new TestAccounts.GameMaster2(this.Context, this.GameConfiguration).Initialize();
        new TestAccounts.Unlocked(this.Context, this.GameConfiguration).Initialize();
    }
}