namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about a duel score update.
/// </summary>
public interface IShowDuelScoreUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// The score of the duel has been changed and needs to be updated.
    /// </summary>
    ValueTask UpdateScoreAsync(Player player1, byte player1Score, Player player2, byte player2Score);
}