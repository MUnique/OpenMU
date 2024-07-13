namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about the updated spectator list.
/// </summary>
public interface IDuelSpectatorListUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the spectator list.
    /// </summary>
    /// <param name="spectators">The spectators.</param>
    ValueTask UpdateSpectatorListAsync(IList<Player> spectators);
}