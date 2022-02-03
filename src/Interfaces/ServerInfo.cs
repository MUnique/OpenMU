namespace MUnique.OpenMU.Interfaces;

public record ServerInfo(ushort Id, string Description, int CurrentConnections, int MaximumConnections)
{
    public int CurrentConnections { get; set; } = CurrentConnections;
}