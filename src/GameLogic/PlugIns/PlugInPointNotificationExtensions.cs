// <copyright file="PlugInPointNotificationExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.PlugIns;

/// <summary>
/// Extensions to notify plugin points about events of long running processes, like mini games or castle siege.
/// </summary>
public static class PlugInPointNotificationExtensions
{
    /// <summary>
    /// Notifies the plugins of a plugin point. Errors of the plugins are logged instead of thrown,
    /// so that a faulty plugin can't break the process which notifies it.
    /// </summary>
    /// <typeparam name="TPlugIn">The type of the plugin point.</typeparam>
    /// <param name="plugInManager">The plugin manager.</param>
    /// <param name="notify">The function which notifies the plugin point.</param>
    /// <param name="logger">The logger for the errors of the plugins.</param>
    public static async ValueTask NotifyPlugInsAsync<TPlugIn>(this PlugInManager plugInManager, Func<TPlugIn, ValueTask> notify, ILogger logger)
        where TPlugIn : class
    {
        try
        {
            if (plugInManager.GetPlugInPoint<TPlugIn>() is { } plugInPoint)
            {
                await notify(plugInPoint).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when notifying the plugins of {plugInPoint}", typeof(TPlugIn).Name);
        }
    }
}
