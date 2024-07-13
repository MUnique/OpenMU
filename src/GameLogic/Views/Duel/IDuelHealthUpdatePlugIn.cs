namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about the health update of the duel players.
/// </summary>
public interface IDuelHealthUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the health of both players.
    /// </summary>
    /// <param name="duelRoom">The duel room.</param>
    ValueTask UpdateHealthAsync(DuelRoom duelRoom);
}