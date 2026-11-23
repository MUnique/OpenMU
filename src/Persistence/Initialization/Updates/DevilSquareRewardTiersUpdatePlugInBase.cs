// <copyright file="DevilSquareRewardTiersUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Events;

/// <summary>
/// This update configures the rewards of the Devil Square by tiers, so that every ranked player gets a reward,
/// not only the ranks 1 to 4. The reward amounts of the ranks 1 to 4 are kept as the amounts of the tiers 1 to 4.
/// See <see cref="DevilSquareRewardTiers"/>.
/// </summary>
public abstract class DevilSquareRewardTiersUpdatePlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => PlugInResources.DevilSquareRewardTiersUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.DevilSquareRewardTiersUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 22, 22, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var devilSquare in gameConfiguration.MiniGameDefinitions.Where(game => game.Type == MiniGameType.DevilSquare))
        {
            DevilSquareRewardTiers.Apply(context, devilSquare);
        }

        return ValueTask.CompletedTask;
    }
}
