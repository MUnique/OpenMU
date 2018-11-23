// <copyright file="ObservableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Linq;

    /// <summary>
    /// Extensions for <see cref="IObservable"/> objects.
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// Executes the action for all observing players of the observable.
        /// </summary>
        /// <param name="observable">The observable.</param>
        /// <param name="action">The action.</param>
        /// <param name="includeThis">if set to <c>true</c> the <paramref name="action"/> should be done for <paramref name="observable"/> too.</param>
        public static void ForEachObservingPlayer(this IObservable observable, Action<Player> action, bool includeThis)
        {
            observable.ObserverLock.EnterReadLock();
            try
            {
                foreach (var obs in observable.Observers.OfType<Player>())
                {
                    if (!includeThis && obs.Equals(observable))
                    {
                        continue;
                    }

                    action(obs);
                }
            }
            finally
            {
                observable.ObserverLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Executes the action for all observers of the observable.
        /// </summary>
        /// <param name="observable">The observable.</param>
        /// <param name="action">The action.</param>
        /// <param name="includeThis">if set to <c>true</c> the <paramref name="action"/> should be done for <paramref name="observable"/> too.</param>
        public static void ForEachWorldObserver(this IObservable observable, Action<IWorldObserver> action, bool includeThis)
        {
            observable.ObserverLock.EnterReadLock();
            try
            {
                foreach (var obs in observable.Observers)
                {
                    if (!includeThis && obs.Equals(observable))
                    {
                        continue;
                    }

                    action(obs);
                }
            }
            finally
            {
                observable.ObserverLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets the observing player with the specified identifier, if found. Otherwise, null.
        /// </summary>
        /// <param name="observable">The observable.</param>
        /// <param name="playerId">The player identifier.</param>
        /// <returns>The observing player with the specified identifier, if found. Otherwise, null.</returns>
        public static Player GetObservingPlayerWithId(this IObservable observable, ushort playerId)
        {
            observable.ObserverLock.EnterReadLock();
            try
            {
                return observable.Observers.OfType<Player>().FirstOrDefault(p => p.Id == playerId);
            }
            finally
            {
                observable.ObserverLock.ExitReadLock();
            }
        }
    }
}