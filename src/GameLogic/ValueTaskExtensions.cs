// <copyright file="ValueTaskExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics;
using System.Threading;

/// <summary>
/// Extensions for <see cref="ValueTask"/>s.
/// </summary>
public static class ValueTaskExtensions
{
    /// <summary>
    /// Returns the task as a timer callback.
    /// </summary>
    /// <param name="valueTask">The value task.</param>
    /// <returns>The timer callback.</returns>
    public static TimerCallback AsTimerCallback(this ValueTask valueTask)
    {
        // ReSharper disable once AsyncVoidLambda we catch all exceptions.
        return async _ =>
        {
            try
            {
                await valueTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message, ex.StackTrace);
            }
        };
    }
}