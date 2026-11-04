/**
 * The terrain editor keeps the attributes of the 256 x 256 tiles of a terrain in the browser,
 * so that painting is immediate. After each finished edit, the Blazor component is notified
 * and fetches the whole terrain through a stream.
 *
 * Like in the map editor, the game X coordinate is drawn vertically (rows) and
 * the game Y coordinate horizontally (columns).
 */

const SIZE = 256;
const MAX_HISTORY = 100;

class TerrainEditor {
    /**
     * @param {HTMLElement} host - The scrollable host element.
     * @param {HTMLCanvasElement} canvas - The canvas which displays the terrain.
     * @param {HTMLElement} selection - The element which previews a rectangle selection.
     * @param {HTMLElement} status - The element which shows the hovered tile.
     * @param {object} dotNetRef - The reference to the Blazor component.
     * @param {object} options - The palette, flag names and the zoom limits.
     */
    constructor(host, canvas, selection, status, dotNetRef, options) {
        this.host = host;
        this.canvas = canvas;
        this.selection = selection;
        this.status = status;
        this.dotNetRef = dotNetRef;
        this.palette = options.palette.map((p) => ({ mask: p.mask, rgb: TerrainEditor.parseColor(p.color) }));
        this.walkableRgb = TerrainEditor.parseColor(options.walkableColor);
        this.flagNames = options.flagNames;
        this.zoom = options.zoom;
        this.minimumZoom = options.minimumZoom;
        this.maximumZoom = options.maximumZoom;

        this.data = new Uint8Array(SIZE * SIZE);
        this.undoStack = [];
        this.redoStack = [];

        this.tool = "brush";
        this.brushSize = 1;
        this.mode = "add";
        this.flags = 0;

        this.strokeSnapshot = null;
        this.lastTile = null;
        this.startTile = null;
        this.pan = null;

        canvas.width = SIZE;
        canvas.height = SIZE;
        this.context = canvas.getContext("2d");
        this.image = this.context.createImageData(SIZE, SIZE);

        this.onMouseDown = this.onMouseDown.bind(this);
        this.onMouseMove = this.onMouseMove.bind(this);
        this.onMouseUp = this.onMouseUp.bind(this);
        this.onMouseLeave = this.onMouseLeave.bind(this);
        this.onWheel = this.onWheel.bind(this);
        this.onContextMenu = (e) => e.preventDefault();

        host.addEventListener("mousedown", this.onMouseDown);
        host.addEventListener("mouseleave", this.onMouseLeave);
        host.addEventListener("wheel", this.onWheel, { passive: false });
        host.addEventListener("contextmenu", this.onContextMenu);
        document.addEventListener("mousemove", this.onMouseMove);
        document.addEventListener("mouseup", this.onMouseUp);

        this.applyZoom();
    }

    static parseColor(hex) {
        const value = parseInt(hex.substring(1), 16);
        return [(value >> 16) & 0xFF, (value >> 8) & 0xFF, value & 0xFF];
    }

    /**
     * Replaces the displayed terrain.
     * @param {string} base64 - The attributes of all tiles, base64 encoded.
     * @param {boolean} asEdit - True, if the replacement is an undoable edit (e.g. an import);
     *  false, if another terrain was loaded.
     */
    setData(base64, asEdit) {
        const bytes = Uint8Array.from(atob(base64), (c) => c.charCodeAt(0));
        if (bytes.length !== SIZE * SIZE) {
            return;
        }

        if (asEdit) {
            this.pushUndo(this.data.slice());
            this.data = bytes;
            this.render();
            this.notifyChanged();
        } else {
            this.data = bytes;
            this.undoStack = [];
            this.redoStack = [];
            this.render();
        }
    }

    /**
     * Gets the attributes of all tiles. They are requested as stream by the Blazor component,
     * because they exceed the maximum message size of the circuit.
     */
    getData() {
        return this.data;
    }

    setTool(tool, brushSize, mode, flags) {
        this.tool = tool;
        this.brushSize = Math.max(1, brushSize | 0);
        this.mode = mode;
        this.flags = flags & 0xFF;
    }

    setGrid(showGrid) {
        this.host.classList.toggle("show-grid", showGrid);
    }

    undo() {
        if (this.undoStack.length === 0) {
            return;
        }

        this.redoStack.push(this.data);
        this.data = this.undoStack.pop();
        this.render();
        this.notifyChanged();
    }

    redo() {
        if (this.redoStack.length === 0) {
            return;
        }

        this.undoStack.push(this.data);
        this.data = this.redoStack.pop();
        this.render();
        this.notifyChanged();
    }

    /**
     * Changes the zoom by the given number of steps, keeping the center of the view.
     * @returns {number} The new zoom.
     */
    zoomBy(steps) {
        return this.zoomTo(this.zoom + steps);
    }

    /**
     * Changes the zoom to the given value, keeping the center of the view.
     * @returns {number} The new zoom.
     */
    zoomTo(zoom) {
        const rect = this.host.getBoundingClientRect();
        this.zoomAround(zoom, rect.left + this.host.clientWidth / 2, rect.top + this.host.clientHeight / 2);
        return this.zoom;
    }

    dispose() {
        this.host.removeEventListener("mousedown", this.onMouseDown);
        this.host.removeEventListener("mouseleave", this.onMouseLeave);
        this.host.removeEventListener("wheel", this.onWheel);
        this.host.removeEventListener("contextmenu", this.onContextMenu);
        document.removeEventListener("mousemove", this.onMouseMove);
        document.removeEventListener("mouseup", this.onMouseUp);
        this.dotNetRef = null;
    }

    zoomAround(newZoom, clientX, clientY) {
        newZoom = Math.max(this.minimumZoom, Math.min(this.maximumZoom, newZoom));
        if (newZoom === this.zoom) {
            return;
        }

        const rect = this.host.getBoundingClientRect();
        const offsetX = clientX - rect.left;
        const offsetY = clientY - rect.top;
        const contentX = (this.host.scrollLeft + offsetX) / this.zoom;
        const contentY = (this.host.scrollTop + offsetY) / this.zoom;
        this.zoom = newZoom;
        this.applyZoom();
        this.host.scrollLeft = contentX * newZoom - offsetX;
        this.host.scrollTop = contentY * newZoom - offsetY;
        this.dotNetRef?.invokeMethodAsync("OnZoomChanged", this.zoom);
    }

    applyZoom() {
        const size = `${SIZE * this.zoom}px`;
        this.canvas.style.width = size;
        this.canvas.style.height = size;
        this.host.style.setProperty("--tile-size", `${this.zoom}px`);
        this.updateSelectionPreview(null);
    }

    render() {
        const pixels = this.image.data;
        for (let row = 0; row < SIZE; row++) {
            for (let column = 0; column < SIZE; column++) {
                // row = game X, column = game Y; the data is stored by y, then x.
                const rgb = this.getColor(this.data[(column * SIZE) + row]);
                const offset = ((row * SIZE) + column) * 4;
                pixels[offset] = rgb[0];
                pixels[offset + 1] = rgb[1];
                pixels[offset + 2] = rgb[2];
                pixels[offset + 3] = 255;
            }
        }

        this.context.putImageData(this.image, 0, 0);
    }

    getColor(value) {
        for (const entry of this.palette) {
            if ((value & entry.mask) !== 0) {
                return entry.rgb;
            }
        }

        return this.walkableRgb;
    }

    getTile(e) {
        const rect = this.canvas.getBoundingClientRect();
        const x = Math.floor((e.clientY - rect.top) / this.zoom);
        const y = Math.floor((e.clientX - rect.left) / this.zoom);
        if (x < 0 || y < 0 || x >= SIZE || y >= SIZE) {
            return null;
        }

        return { x, y };
    }

    applyToValue(value) {
        switch (this.mode) {
            case "remove":
                return value & ~this.flags;
            case "replace":
                return this.flags;
            default:
                return value | this.flags;
        }
    }

    applyToTile(x, y) {
        if (x < 0 || y < 0 || x >= SIZE || y >= SIZE) {
            return;
        }

        const index = (y * SIZE) + x;
        this.data[index] = this.applyToValue(this.data[index]);
    }

    applyBrush(tile) {
        const offset = Math.floor((this.brushSize - 1) / 2);
        for (let x = tile.x - offset; x < tile.x - offset + this.brushSize; x++) {
            for (let y = tile.y - offset; y < tile.y - offset + this.brushSize; y++) {
                this.applyToTile(x, y);
            }
        }
    }

    /** Applies the brush along the line between two tiles, so that fast mouse movements leave no gaps. */
    applyBrushLine(from, to) {
        let x = from.x;
        let y = from.y;
        const dx = Math.abs(to.x - x);
        const dy = -Math.abs(to.y - y);
        const stepX = x < to.x ? 1 : -1;
        const stepY = y < to.y ? 1 : -1;
        let error = dx + dy;
        this.applyBrush({ x, y });
        while (x !== to.x || y !== to.y) {
            const doubledError = 2 * error;
            if (doubledError >= dy) {
                error += dy;
                x += stepX;
            }

            if (doubledError <= dx) {
                error += dx;
                y += stepY;
            }

            this.applyBrush({ x, y });
        }
    }

    applyRectangle(from, to) {
        for (let x = Math.min(from.x, to.x); x <= Math.max(from.x, to.x); x++) {
            for (let y = Math.min(from.y, to.y); y <= Math.max(from.y, to.y); y++) {
                this.applyToTile(x, y);
            }
        }
    }

    /** Applies the edit to all connected tiles which have the same value as the clicked one. */
    applyFill(tile) {
        const target = this.data[(tile.y * SIZE) + tile.x];
        if (this.applyToValue(target) === target) {
            return;
        }

        const visited = new Uint8Array(SIZE * SIZE);
        const stack = [(tile.y * SIZE) + tile.x];
        while (stack.length > 0) {
            const index = stack.pop();
            if (visited[index] || this.data[index] !== target) {
                continue;
            }

            visited[index] = 1;
            this.data[index] = this.applyToValue(target);
            const x = index % SIZE;
            const y = Math.floor(index / SIZE);
            if (x > 0) {
                stack.push(index - 1);
            }

            if (x < SIZE - 1) {
                stack.push(index + 1);
            }

            if (y > 0) {
                stack.push(index - SIZE);
            }

            if (y < SIZE - 1) {
                stack.push(index + SIZE);
            }
        }
    }

    onMouseDown(e) {
        if (e.button === 2 || e.button === 1) {
            this.pan = { x: e.clientX, y: e.clientY, left: this.host.scrollLeft, top: this.host.scrollTop };
            this.host.classList.add("panning");
            e.preventDefault();
            return;
        }

        const tile = e.button === 0 ? this.getTile(e) : null;
        if (!tile || (this.flags === 0 && this.mode !== "replace")) {
            return;
        }

        e.preventDefault();
        this.strokeSnapshot = this.data.slice();
        switch (this.tool) {
            case "fill":
                this.applyFill(tile);
                this.finishStroke();
                break;
            case "rectangle":
                this.startTile = tile;
                this.updateSelectionPreview(tile);
                break;
            default:
                this.lastTile = tile;
                this.applyBrush(tile);
                this.render();
                break;
        }
    }

    onMouseMove(e) {
        if (this.pan) {
            this.host.scrollLeft = this.pan.left - (e.clientX - this.pan.x);
            this.host.scrollTop = this.pan.top - (e.clientY - this.pan.y);
            return;
        }

        const tile = this.getTile(e);
        this.updateStatus(tile);
        if (!tile || !this.strokeSnapshot) {
            return;
        }

        if (this.startTile) {
            this.updateSelectionPreview(tile);
        } else if (this.lastTile && (tile.x !== this.lastTile.x || tile.y !== this.lastTile.y)) {
            this.applyBrushLine(this.lastTile, tile);
            this.lastTile = tile;
            this.render();
        }
    }

    onMouseUp(e) {
        if (this.pan) {
            this.pan = null;
            this.host.classList.remove("panning");
            return;
        }

        if (!this.strokeSnapshot) {
            return;
        }

        if (this.startTile) {
            const tile = this.getTile(e) ?? this.clampTile(e);
            this.applyRectangle(this.startTile, tile);
        }

        this.finishStroke();
    }

    onMouseLeave() {
        this.updateStatus(null);
    }

    onWheel(e) {
        e.preventDefault();
        this.zoomAround(this.zoom + (e.deltaY < 0 ? 1 : -1), e.clientX, e.clientY);
    }

    clampTile(e) {
        const rect = this.canvas.getBoundingClientRect();
        const clamp = (v) => Math.max(0, Math.min(SIZE - 1, v));
        return {
            x: clamp(Math.floor((e.clientY - rect.top) / this.zoom)),
            y: clamp(Math.floor((e.clientX - rect.left) / this.zoom)),
        };
    }

    finishStroke() {
        const snapshot = this.strokeSnapshot;
        this.strokeSnapshot = null;
        this.lastTile = null;
        this.startTile = null;
        this.updateSelectionPreview(null);
        this.render();

        if (snapshot.every((value, index) => value === this.data[index])) {
            return;
        }

        this.pushUndo(snapshot);
        this.notifyChanged();
    }

    pushUndo(snapshot) {
        this.undoStack.push(snapshot);
        if (this.undoStack.length > MAX_HISTORY) {
            this.undoStack.shift();
        }

        this.redoStack = [];
    }

    notifyChanged() {
        this.dotNetRef?.invokeMethodAsync("OnTerrainChangedAsync", this.undoStack.length > 0, this.redoStack.length > 0);
    }

    updateSelectionPreview(tile) {
        if (!this.startTile || !tile) {
            this.selection.style.display = "none";
            return;
        }

        const top = Math.min(this.startTile.x, tile.x);
        const left = Math.min(this.startTile.y, tile.y);
        this.selection.style.display = "block";
        this.selection.style.top = `${top * this.zoom}px`;
        this.selection.style.left = `${left * this.zoom}px`;
        this.selection.style.height = `${(Math.abs(this.startTile.x - tile.x) + 1) * this.zoom}px`;
        this.selection.style.width = `${(Math.abs(this.startTile.y - tile.y) + 1) * this.zoom}px`;
    }

    updateStatus(tile) {
        if (!tile) {
            this.status.textContent = "";
            return;
        }

        const value = this.data[(tile.y * SIZE) + tile.x];
        const names = this.flagNames.filter((f) => (value & f.mask) !== 0).map((f) => f.name);
        let text = `X: ${tile.x}, Y: ${tile.y} | ${value}`;
        if (names.length > 0) {
            text += ` (${names.join(", ")})`;
        }

        if (this.startTile) {
            text += ` | ${Math.abs(this.startTile.x - tile.x) + 1} x ${Math.abs(this.startTile.y - tile.y) + 1}`;
        }

        this.status.textContent = text;
    }
}

/**
 * Creates a terrain editor.
 * @returns {TerrainEditor} The editor, which is used by the Blazor component through an object reference.
 */
export function createEditor(host, canvas, selection, status, dotNetRef, options) {
    return new TerrainEditor(host, canvas, selection, status, dotNetRef, options);
}

/**
 * Sets the value of a select element without triggering a change event.
 * @param {HTMLSelectElement} selectElement - The select element to update.
 * @param {string} value - The value to select.
 */
export function setSelectValue(selectElement, value) {
    if (selectElement) {
        selectElement.value = value;
    }
}

/**
 * Triggers a click on the file input used for importing a terrain file.
 * @param {string} id - The id of the input element.
 */
export function triggerFileInput(id) {
    document.getElementById(id)?.click();
}

/**
 * Downloads a base64-encoded byte array as a file via a temporary anchor element.
 * @param {string} fileName - The suggested download file name.
 * @param {string} base64 - The base64-encoded file content.
 */
export function downloadFile(fileName, base64) {
    const bytes = Uint8Array.from(atob(base64), (c) => c.charCodeAt(0));
    const blob = new Blob([bytes], { type: "application/octet-stream" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}
