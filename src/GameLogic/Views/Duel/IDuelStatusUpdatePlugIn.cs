namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about the status of the available duel rooms.
/// </summary>
public interface IDuelStatusUpdatePlugIn : IViewPlugIn
{

    /// <summary>
    /// SHows the status of the duel rooms.
    /// </summary>
    /// <param name="duelRooms">The duel rooms.</param>
    ValueTask UpdateStatusAsync(DuelRoom?[] duelRooms);
}