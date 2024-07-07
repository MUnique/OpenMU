namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about the health update of the duel players.
/// </summary>
public interface IDuelHealthUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the health of both players.
    /// </summary>
    /// <param name="player1">The player1.</param>
    /// <param name="player2">The player2.</param>
    ValueTask UpdateHealthAsync(Player player1, Player player2);
}