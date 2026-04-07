// <copyright file="MapDragState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Stores the state required for dragging and resizing map objects.
/// </summary>
internal struct MapDragState
{
    /// <summary>Gets or sets the starting X coordinate of the mouse drag.</summary>
    public byte StartX;

    /// <summary>Gets or sets the starting Y coordinate of the mouse drag.</summary>
    public byte StartY;

    /// <summary>Gets or sets the original X1 corner of the dragged object.</summary>
    public byte OrigX1;

    /// <summary>Gets or sets the original Y1 corner of the dragged object.</summary>
    public byte OrigY1;

    /// <summary>Gets or sets the original X2 corner of the dragged object.</summary>
    public byte OrigX2;

    /// <summary>Gets or sets the original Y2 corner of the dragged object.</summary>
    public byte OrigY2;

    /// <summary>Captures the current bounds of the given spawn area as drag origin.</summary>
    /// <param name="spawn">The spawn area to capture bounds from.</param>
    public void Capture(MonsterSpawnArea spawn)
    {
        this.OrigX1 = spawn.X1;
        this.OrigY1 = spawn.Y1;
        this.OrigX2 = spawn.X2;
        this.OrigY2 = spawn.Y2;
    }

    /// <summary>Captures the current bounds of the given gate as drag origin.</summary>
    /// <param name="gate">The gate to capture bounds from.</param>
    public void Capture(Gate gate)
    {
        this.OrigX1 = gate.X1;
        this.OrigY1 = gate.Y1;
        this.OrigX2 = gate.X2;
        this.OrigY2 = gate.Y2;
    }

    /// <summary>
    /// Computes new dragged bounds and returns whether the position changed.
    /// </summary>
    /// <param name="x">Current mouse X in map coordinates.</param>
    /// <param name="y">Current mouse Y in map coordinates.</param>
    /// <param name="mapSize">The map size in tiles.</param>
    /// <param name="newX1">Computed new X1 value.</param>
    /// <param name="newY1">Computed new Y1 value.</param>
    /// <param name="newX2">Computed new X2 value.</param>
    /// <param name="newY2">Computed new Y2 value.</param>
    /// <returns>True if the computed bounds differ from the originals.</returns>
    public bool ApplyDrag(byte x, byte y, int mapSize, out byte newX1, out byte newY1, out byte newX2, out byte newY2)
    {
        int dx = x - this.StartX;
        int dy = y - this.StartY;
        int width = this.OrigX2 - this.OrigX1;
        int height = this.OrigY2 - this.OrigY1;

        newX1 = (byte)Math.Clamp(this.OrigX1 + dx, 0, mapSize - 1 - width);
        newY1 = (byte)Math.Clamp(this.OrigY1 + dy, 0, mapSize - 1 - height);
        newX2 = (byte)(newX1 + width);
        newY2 = (byte)(newY1 + height);

        return this.OrigX1 != newX1 || this.OrigY1 != newY1 || this.OrigX2 != newX2 || this.OrigY2 != newY2;
    }
}
