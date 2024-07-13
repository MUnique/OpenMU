namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about a removed spectator.
/// </summary>
public interface IDuelSpectatorRemovedPlugIn : IViewPlugIn
{
    /// <summary>
    /// Removes the spectator.
    /// </summary>
    /// <param name="spectator">The spectator.</param>
    ValueTask SpectatorRemovedAsync(Player spectator);
}