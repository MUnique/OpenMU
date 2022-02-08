using System.ComponentModel;
using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.Interfaces;

public interface IServerProvider : INotifyPropertyChanged
{
    IList<IManageableServer> Servers { get; }
}