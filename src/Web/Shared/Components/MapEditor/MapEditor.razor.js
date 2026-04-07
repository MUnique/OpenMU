// <copyright file="MapEditor.razor.js" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

const BASE_SIZE = 768;
const MIN_ZOOM = 1.0;
const MAX_ZOOM = 4.0;

let _element = null;
let _dotNetRef = null;
let _zoomLevel = 1.0;
let _isDragging = false;
let _isPanning = false;
let _isResizing = false;
let _hasMoved = false;
let _lastClientX = 0;
let _lastClientY = 0;

function _isElementWithClass(node, className) {
    return node.nodeType === Node.ELEMENT_NODE && node.classList?.contains(className);
}

/**
 * Manually walks up the DOM tree checking classList.
 * Safe for SVG elements that lack HTMLElement.closest().
 */
function _closestByClass(node, className) {
    while (node) {
        if (_isElementWithClass(node, className)) {
            return node;
        }
        node = node.parentNode;
    }
    return null;
}

function _getMapTileCoords(clientX, clientY) {
    if (!_element) {
        return { x: 0, y: 0 };
    }

    const rect = _element.getBoundingClientRect();
    let contentX = (clientX - rect.left) + _element.scrollLeft;
    let contentY = (clientY - rect.top) + _element.scrollTop;
    let scale = 3 * _zoomLevel;

    return {
        x: Math.floor(contentY / scale),
        y: Math.floor(contentX / scale)
    };
}

function _updateZoomDisplay() {
    let container = _element ? _element.closest(".map-host-container") : null;
    if (!container) {
        return;
    }

    let zoomLabel = container.querySelector(".zoom-label");
    if (zoomLabel) {
        zoomLabel.textContent = Math.round(_zoomLevel * 100) + "%";
    }
}

function _applyZoom(zoom) {
    if (!_element) {
        return;
    }

    let content = _element.querySelector(".map-content");
    let img = _element.querySelector(".map-content img");

    if (!content) {
        return;
    }

    let size = Math.round(BASE_SIZE * zoom);
    content.style.width = size + "px";
    content.style.height = size + "px";

    if (img) {
        img.style.width = size + "px";
        img.style.height = size + "px";
    }

    _updateZoomDisplay();
}

function _updateCoordsLabel(mapX, mapY) {
    let container = _element ? _element.closest(".map-host-container") : null;
    if (!container) {
        return;
    }

    let coordLabel = container.querySelector(".coord-label");
    if (coordLabel) {
        coordLabel.textContent = "X: " + mapX + ", Y: " + mapY;
    }
}

function _tryStartResize(e) {
    let resizer = _closestByClass(e.target, "resizer");
    if (resizer) {
        _isResizing = true;
        _hasMoved = false;
        return true;
    }
    return false;
}

function _notifyPointerDown(x, y) {
    if (_dotNetRef) {
        _dotNetRef.invokeMethodAsync("OnPointerDown", x, y);
    }
}

function _handlePanning(e) {
    _element.scrollLeft = Math.max(0, _element.scrollLeft - (e.clientX - _lastClientX));
    _element.scrollTop = Math.max(0, _element.scrollTop - (e.clientY - _lastClientY));
    _lastClientX = e.clientX;
    _lastClientY = e.clientY;
}

function _notifyPointerMove(x, y) {
    if (_dotNetRef) {
        _dotNetRef.invokeMethodAsync("OnPointerMove", x, y);
    }
}

function _handlePanEnd() {
    if (!_hasMoved && _dotNetRef) {
        _dotNetRef.invokeMethodAsync("OnPointerClickAsync");
    }

    _isPanning = false;
}

function _handleDragOrResizeEnd() {
    if (_hasMoved) {
        if (_isDragging) {
            _isDragging = false;
            if (_dotNetRef) {
                _dotNetRef.invokeMethodAsync("OnPointerUp");
            }
        }

        if (_isResizing) {
            _isResizing = false;
            if (_dotNetRef) {
                _dotNetRef.invokeMethodAsync("OnResizingEnd");
            }
        }
    }

    if (!_hasMoved && _isResizing) {
        _isResizing = false;
        if (_dotNetRef) {
            _dotNetRef.invokeMethodAsync("OnResizingEnd");
        }
    }
}

function _onDocumentMouseDown(e) {
    let container = _closestByClass(e.target, "map-host");
    if (!container) {
        return;
    }

    if (container !== _element) {
        _element = container;
    }

    if (_tryStartResize(e)) {
        return;
    }

    e.preventDefault();

    _lastClientX = e.clientX;
    _lastClientY = e.clientY;
    _hasMoved = false;
    _isDragging = true;

    let coords = _getMapTileCoords(e.clientX, e.clientY);
    let x = Math.max(0, Math.min(255, coords.x));
    let y = Math.max(0, Math.min(255, coords.y));

    _notifyPointerDown(x, y);
}

function _onDocumentMouseMove(e) {
    if (!_element) {
        return;
    }

    let container = _closestByClass(e.target, "map-host");
    if (!container || !_element.contains(e.target)) {
        return;
    }

    if (container !== _element) {
        _element = container;
    }

    let coords = _getMapTileCoords(e.clientX, e.clientY);
    let x = Math.max(0, Math.min(255, coords.x));
    let y = Math.max(0, Math.min(255, coords.y));

    _updateCoordsLabel(x, y);
    _hasMoved = true;

    if (_isPanning) {
        _handlePanning(e);
        return;
    }

    if (_isDragging || _isResizing) {
        _notifyPointerMove(x, y);
    }
}

function _onDocumentMouseUp(e) {
    if (!_element) {
        return;
    }

    if (_isPanning) {
        _handlePanEnd();
        return;
    }

    _handleDragOrResizeEnd();

    _isDragging = false;
    _isResizing = false;
    _hasMoved = false;
}

/**
 * Initializes the map editor for the given host element.
 * @param {HTMLElement} element - The map host element.
 * @param {object} dotNetRef - JSInvokable reference to the Blazor component.
 * @param {number} initialZoom - The initial zoom level to apply.
 */
export function initialize(element, dotNetRef, initialZoom) {
    if (!element) {
        return;
    }

    _element = element;
    _dotNetRef = dotNetRef;
    _zoomLevel = initialZoom;
    _applyZoom(_zoomLevel);

    document.removeEventListener("mousedown", _onDocumentMouseDown);
    document.removeEventListener("mousemove", _onDocumentMouseMove);
    document.removeEventListener("mouseup", _onDocumentMouseUp);
    document.addEventListener("mousedown", _onDocumentMouseDown);
    document.addEventListener("mousemove", _onDocumentMouseMove);
    document.addEventListener("mouseup", _onDocumentMouseUp);
}

/**
 * Scrolls the map host by the given delta.
 * @param {HTMLElement} element - The map host element.
 * @param {number} deltaX - The horizontal scroll delta.
 * @param {number} deltaY - The vertical scroll delta.
 */
export function scrollBy(element, deltaX, deltaY) {
    if (!element) {
        return;
    }
    element.scrollLeft = Math.max(0, element.scrollLeft - deltaX);
    element.scrollTop = Math.max(0, element.scrollTop - deltaY);
}

/**
 * Sets the value of a select element without triggering a change event.
 * @param {HTMLSelectElement} selectElement - The select element to update.
 * @param {string} value - The value to select.
 */
export function setSelectValue(selectElement, value) {
    if (!selectElement) {
        return;
    }
    selectElement.value = value;
}

/**
 * Sets the scroll position of the map host element.
 * @param {HTMLElement} element - The map host element.
 * @param {number} scrollLeft - The horizontal scroll offset.
 * @param {number} scrollTop - The vertical scroll offset.
 */
export function setScroll(element, scrollLeft, scrollTop) {
    if (!element) {
        return;
    }
    element.scrollLeft = Math.max(0, scrollLeft);
    element.scrollTop = Math.max(0, scrollTop);
}

/**
 * Scrolls the map host so that the given map coordinates are centered in the viewport.
 * @param {HTMLElement} element - The map host element.
 * @param {number} mapX - The map X coordinate to center on.
 * @param {number} mapY - The map Y coordinate to center on.
 * @param {number} baseScale - The base pixel scale factor.
 */
export function centerOn(element, mapX, mapY, baseScale) {
    if (!element) {
        return;
    }

    let scale = baseScale * _zoomLevel;

    let pixelX = mapY * scale;
    let pixelY = mapX * scale;

    let maxScrollLeft = element.scrollWidth - element.clientWidth;
    let maxScrollTop = element.scrollHeight - element.clientHeight;

    element.scrollTo({
        left: Math.max(0, Math.min(pixelX - (element.clientWidth / 2), maxScrollLeft)),
        top: Math.max(0, Math.min(pixelY - (element.clientHeight / 2), maxScrollTop)),
        behavior: "smooth"
    });
}

/**
 * Handles a mouse wheel event to zoom in or out centered on the cursor position.
 * @param {HTMLElement} element - The map host element.
 * @param {number} deltaY - The vertical scroll delta from the wheel event.
 * @param {number} clientX - The client X position of the cursor.
 * @param {number} clientY - The client Y position of the cursor.
 * @returns {{ zoomLevel: number, handled: boolean }}
 */
export function handleWheel(element, deltaY, clientX, clientY) {
    if (!element) {
        return { zoomLevel: 1.0, handled: false };
    }

    const oldZoom = _zoomLevel;
    const sensitivity = 0.001;
    const zoomDelta = -deltaY * sensitivity;
    const newZoom = Math.max(MIN_ZOOM, Math.min(MAX_ZOOM, oldZoom * (1.0 + zoomDelta)));

    const rect = element.getBoundingClientRect();
    const mouseX = clientX - rect.left;
    const mouseY = clientY - rect.top;

    const baseX = (element.scrollLeft + mouseX) / oldZoom;
    const baseY = (element.scrollTop + mouseY) / oldZoom;

    _zoomLevel = newZoom;
    _applyZoom(_zoomLevel);

    const maxScrollLeft = Math.max(0, BASE_SIZE * newZoom - rect.width);
    const maxScrollTop = Math.max(0, BASE_SIZE * newZoom - rect.height);

    element.scrollLeft = Math.max(0, Math.min((baseX * newZoom) - mouseX, maxScrollLeft));
    element.scrollTop = Math.max(0, Math.min((baseY * newZoom) - mouseY, maxScrollTop));

    return {
        zoomLevel: newZoom,
        handled: true
    };
}

/**
 * Zooms the map to a specific level, keeping the viewport center stationary.
 * @param {HTMLElement} element - The map host element.
 * @param {number} newZoom - The target zoom level.
 * @returns {Number} The applied zoom level.
 */
export function zoomTo(element, newZoom) {
    if (!element) {
        return 1.0;
    }

    const oldZoom = _zoomLevel;
    const clampedZoom = Math.max(MIN_ZOOM, Math.min(MAX_ZOOM, newZoom));

    const rect = element.getBoundingClientRect();
    const centerX = rect.width / 2;
    const centerY = rect.height / 2;

    const baseX = (element.scrollLeft + centerX) / oldZoom;
    const baseY = (element.scrollTop + centerY) / oldZoom;

    _zoomLevel = clampedZoom;
    _applyZoom(_zoomLevel);

    const maxScrollLeft = Math.max(0, BASE_SIZE * clampedZoom - rect.width);
    const maxScrollTop = Math.max(0, BASE_SIZE * clampedZoom - rect.height);

    element.scrollLeft = Math.max(0, Math.min((baseX * clampedZoom) - centerX, maxScrollLeft));
    element.scrollTop = Math.max(0, Math.min((baseY * clampedZoom) - centerY, maxScrollTop));

    return clampedZoom;
}

/**
 * Resets the zoom level to 1.0 and scrolls the map back to the origin.
 * @param {HTMLElement} element - The map host element.
 * @returns {number} The reset zoom level (always 1.0).
 */
export function resetZoom(element) {
    if (!element) {
        return 1.0;
    }

    _zoomLevel = 1.0;
    _applyZoom(1.0);
    element.scrollLeft = 0;
    element.scrollTop = 0;

    return 1.0;
}

/**
 * Sets the pan state. Called from Blazor to signal panning mode.
 * @param {boolean} panning - Whether panning is active.
 */
export function setPanning(panning) {
    _isPanning = panning;
    _hasMoved = false;
}

/**
 * Sets the drag state. Called from Blazor to signal dragging mode.
 * @param {boolean} dragging - Whether dragging is active.
 */
export function setDragging(dragging) {
    _isDragging = dragging;
    _hasMoved = false;
}

/**
 * Cleans up state associated with the map editor module.
 * Called when the Blazor component is disposed.
 */
export function dispose() {
    document.removeEventListener("mousedown", _onDocumentMouseDown);
    document.removeEventListener("mousemove", _onDocumentMouseMove);
    document.removeEventListener("mouseup", _onDocumentMouseUp);
    _element = null;
    _dotNetRef = null;
}
