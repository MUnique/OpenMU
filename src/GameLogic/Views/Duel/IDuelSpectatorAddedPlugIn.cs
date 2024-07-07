namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about an added duel spectator.
/// </summary>
public interface IDuelSpectatorAddedPlugIn : IViewPlugIn
{
    /// <summary>
    /// Adds the spectator.
    /// </summary>
    /// <param name="spectator">The spectator.</param>
    ValueTask SpectatorAddedAsync(Player spectator);
}