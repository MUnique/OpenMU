namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about a duel request from another player.
/// </summary>
public interface IShowDuelRequestPlugIn : IViewPlugIn
{
    /// <summary>
    /// A player has started a duel request which should be answered.
    /// </summary>
    ValueTask ShowDuelRequestAsync(Player requester);
}