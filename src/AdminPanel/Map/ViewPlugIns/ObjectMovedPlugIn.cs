// <copyright file="ObjectMovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map.ViewPlugIns
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.JSInterop;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Implementation of <see cref="IObjectMovedPlugIn"/> which uses the javascript map app.
    /// </summary>
    public class ObjectMovedPlugIn : JsViewPlugInBase, IObjectMovedPlugIn
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectMovedPlugIn"/> class.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="worldAccessor">The world accessor.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ObjectMovedPlugIn(IJSRuntime jsRuntime, string worldAccessor, CancellationToken cancellationToken)
            : base(jsRuntime, $"{worldAccessor}.objectMoved", cancellationToken)
        {
        }

        private async Task ObjectMovedAsync(ILocateable movedObject, MoveType moveType)
        {
            Point targetPoint = movedObject.Position;
            object steps = null;
            int walkDelay = 0;
            if (movedObject is ISupportWalk walker && moveType == MoveType.Walk)
            {
                targetPoint = walker.WalkTarget;
                walkDelay = (int) walker.StepDelay.TotalMilliseconds;
                var walkingSteps = new WalkingStep[16];
                var stepCount = walker.GetSteps(walkingSteps);
                var walkSteps = walkingSteps.AsSpan().Slice(0, stepCount).ToArray()
                    .Select(step => new {x = step.To.X, y = step.To.Y, direction = step.Direction}).ToList();

                var lastStep = walkSteps.LastOrDefault();
                if (lastStep != null)
                {
                    var lastPoint = new Point(lastStep.x, lastStep.y);
                    var lastDirection = lastPoint.GetDirectionTo(targetPoint);
                    if (lastDirection != Direction.Undefined)
                    {
                        walkSteps.Add(new {x = targetPoint.X, y = targetPoint.Y, direction = lastDirection});
                    }
                }

                steps = walkSteps;
            }

            await this.InvokeAsync(movedObject.Id, targetPoint.X, targetPoint.Y, moveType, walkDelay, steps);
        }

        /// <inheritdoc />
        public async void ObjectMoved(ILocateable movedObject, MoveType moveType)
        {
            try
            {
                await this.ObjectMovedAsync(movedObject, moveType).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // don't need to handle that.
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(this.ObjectMoved)}; movedObject: {movedObject}, moveType: {moveType}", e);
            }
        }
    }
}