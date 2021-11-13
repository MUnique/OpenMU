// <copyright file="TestAccountsInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.TestAccounts;

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
            var level = (i * 20) + 1;
            new LowLevel(this.Context, this.GameConfiguration, "test" + i, level).Initialize();
        }
    }
}