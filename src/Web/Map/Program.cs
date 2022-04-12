// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.GameLogic;

namespace MUnique.OpenMU.Web.Map;

using System.ComponentModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// The class of the entry point.
/// </summary>
public class Program
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddSingleton<IObservableGameServer, NullGameServer>();
                serviceCollection.AddHostedService<MapApp>();
            })
            .Build();
        host.Run();
    }

    private class NullGameServer : IObservableGameServer
    {
        public int Id { get; }

        public IList<IGameMapInfo> Maps { get; } = new List<IGameMapInfo>();

        public void RegisterMapObserver(Guid mapId, ILocateable worldObserver)
        {
            throw new NotImplementedException();
        }

        public void UnregisterMapObserver(Guid mapId, ushort worldObserverId)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}