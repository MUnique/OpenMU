namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about a duel score update.
/// </summary>
public interface IShowDuelScoreUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// The score of the duel has been changed and needs to be updated.
    /// </summary>
    /// <param name="duelRoom">The duel room.</param>
    ValueTask UpdateScoreAsync(DuelRoom duelRoom);
}