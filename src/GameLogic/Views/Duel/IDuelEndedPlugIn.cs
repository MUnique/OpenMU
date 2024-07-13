namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about a duel end.
/// </summary>
public interface IDuelEndedPlugIn : IViewPlugIn
{
    /// <summary>
    /// A duel has ended.
    /// </summary>
    ValueTask DuelEndedAsync();
}