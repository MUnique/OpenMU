// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map;

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.GameLogic;

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

    /// <summary>
    /// An implementation of <see cref="IObservableGameServer"/> which does nothing.
    /// </summary>
    private class NullGameServer : IObservableGameServer
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public IList<IGameMapInfo> Maps { get; } = new List<IGameMapInfo>();

        /// <inheritdoc />
        public ValueTask RegisterMapObserverAsync(Guid mapId, ILocateable worldObserver)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ValueTask UnregisterMapObserverAsync(Guid mapId, ushort worldObserverId)
        {
            throw new NotImplementedException();
        }
    }
}