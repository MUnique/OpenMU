namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about a started duel.
/// </summary>
public interface IInitializeDuelPlugIn : IViewPlugIn
{
    /// <summary>
    /// Initializes the duel on the client side.
    /// </summary>
    /// <param name="duelRoom">The duel context.</param>
    ValueTask InitializeDuelAsync(DuelRoom duelRoom);
}