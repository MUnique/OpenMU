// <copyright file="MapEditor.razor.js" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

const LEFT_MOUSE_BUTTON = 0;
const MAX_ZOOM = 4.0;
const MIDDLE_MOUSE_BUTTON = 1;
const MIDDLE_MOUSE_BUTTON_MASK = 4;
const MIN_ZOOM = 1.0;
const NO_MOUSE_BUTTONS = 0;
const WHEEL_SENSITIVITY = 0.001;
const WHEEL_LINE_HEIGHT_PX = 16;
const WHEEL_PAGE_HEIGHT_PX = 400;
const ZOOM_NOTIFY_DELAY_MS = 150;

let _element = null;
let _dotNetRef = null;
let _baseScale = 3;
let _defaultZoom = 1.0;
let _zoomLevel = _defaultZoom;
let _isDragging = false;
let _isPanning = false;
let _isResizing = false;
let _hasMoved = false;
let _lastClientX = 0;
let _lastClientY = 0;
let _wheelFactor = 1;
let _wheelClientX = 0;
let _wheelClientY = 0;
let _wheelFrame = 0;
let _zoomNotifyTimer = 0;

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
    let scale = _baseScale * _zoomLevel;

    return {
        x: Math.floor(contentY / scale),
        y: Math.floor(contentX / scale)
    };
}

/**
 * Applies the zoom by setting the "--map-scale" CSS variable (pixels per tile).
 * The size of the map content, the terrain image, the grid and all map objects
 * are derived from it by CSS, so one style change updates everything in the same frame.
 */
function _applyZoom(zoom) {
    if (!_element) {
        return;
    }

    let content = _element.querySelector(".map-content");
    if (!content) {
        return;
    }

    content.style.setProperty("--map-scale", (_baseScale * zoom) + "px");
}

/**
 * Zooms to the given level, keeping the point (anchorX, anchorY), given relative to
 * the map host element's top left corner, at the same position on the screen.
 */
function _zoomAround(newZoom, anchorX, anchorY) {
    const oldZoom = _zoomLevel;
    const baseX = (_element.scrollLeft + anchorX) / oldZoom;
    const baseY = (_element.scrollTop + anchorY) / oldZoom;

    _zoomLevel = newZoom;
    _applyZoom(newZoom);

    // The browser clamps the scroll position to the new scrollable area.
    _element.scrollLeft = Math.max(0, (baseX * newZoom) - anchorX);
    _element.scrollTop = Math.max(0, (baseY * newZoom) - anchorY);
}

/**
 * Tells the Blazor component about the current zoom level, but only after the user
 * stopped zooming for a moment. This avoids server round trips while zooming.
 */
function _scheduleZoomNotification() {
    clearTimeout(_zoomNotifyTimer);
    _zoomNotifyTimer = setTimeout(() => {
        if (_dotNetRef) {
            _dotNetRef.invokeMethodAsync("OnZoomChangedAsync", _zoomLevel);
        }
    }, ZOOM_NOTIFY_DELAY_MS);
}

function _flushWheelZoom() {
    _wheelFrame = 0;
    if (!_element) {
        _wheelFactor = 1;
        return;
    }

    const newZoom = Math.max(MIN_ZOOM, Math.min(MAX_ZOOM, _zoomLevel * _wheelFactor));
    _wheelFactor = 1;
    if (newZoom === _zoomLevel) {
        return;
    }

    const rect = _element.getBoundingClientRect();
    _zoomAround(newZoom, _wheelClientX - rect.left, _wheelClientY - rect.top);
    _scheduleZoomNotification();
}

function _getWheelDeltaY(e) {
    if (e.deltaMode === 1) {
        return e.deltaY * WHEEL_LINE_HEIGHT_PX;
    }

    if (e.deltaMode === 2) {
        return e.deltaY * WHEEL_PAGE_HEIGHT_PX;
    }

    return e.deltaY;
}

function _findZoomableMapHost(target) {
    const host = _closestByClass(target, "map-host");
    if (!host || !host.querySelector(":scope > .map-content")) {
        return null;
    }

    return host;
}

/**
 * Handles the mouse wheel natively in the browser. The wheel events are combined and applied
 * at most once per animation frame, so fast wheels or touchpads don't flood the main thread.
 */
function _onDocumentWheel(e) {
    const host = _findZoomableMapHost(e.target);
    if (!host) {
        return;
    }

    e.preventDefault();
    _element = host;

    const deltaY = _getWheelDeltaY(e);
    _wheelFactor *= Math.max(0.5, Math.min(2, 1 - (deltaY * WHEEL_SENSITIVITY)));
    _wheelClientX = e.clientX;
    _wheelClientY = e.clientY;

    if (!_wheelFrame) {
        _wheelFrame = requestAnimationFrame(_flushWheelZoom);
    }
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

function _notifyPointerDown(x, y, shiftKey) {
    if (_dotNetRef) {
        _dotNetRef.invokeMethodAsync("OnPointerDown", x, y, shiftKey);
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
    _isPanning = false;
}

function _notifyPointerUp() {
    if (_dotNetRef) {
        _dotNetRef.invokeMethodAsync("OnPointerUp");
    }
}

function _notifyResizingEnd() {
    if (_dotNetRef) {
        _dotNetRef.invokeMethodAsync("OnResizingEnd");
    }
}

function _handleDragOrResizeEnd() {
    if (_hasMoved && _isDragging) {
        _isDragging = false;
        _notifyPointerUp();
    }

    if (_isResizing) {
        _isResizing = false;
        _notifyResizingEnd();
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

    if (e.button === MIDDLE_MOUSE_BUTTON) {
        e.preventDefault();
        _lastClientX = e.clientX;
        _lastClientY = e.clientY;
        _hasMoved = false;
        _isPanning = true;
        return;
    }

    if (e.button !== LEFT_MOUSE_BUTTON) {
        return;
    }

    if (_tryStartResize(e)) {
        return;
    }

    e.preventDefault();

    _lastClientX = e.clientX;
    _lastClientY = e.clientY;
    _hasMoved = false;
    _isDragging = e.ctrlKey;

    let coords = _getMapTileCoords(e.clientX, e.clientY);
    let x = Math.max(0, Math.min(255, coords.x));
    let y = Math.max(0, Math.min(255, coords.y));

    _notifyPointerDown(x, y, e.shiftKey);
}

function _isValidMouseMoveTarget(e) {
    if (!_element) {
        return false;
    }
    let container = _closestByClass(e.target, "map-host");
    return container !== null && _element.contains(e.target);
}

function _updateElementIfChanged(e) {
    let container = _closestByClass(e.target, "map-host");
    if (container !== _element) {
        _element = container;
    }
}

function _applyMouseMoveAction(x, y) {
    if (_isDragging || _isResizing) {
        _notifyPointerMove(x, y);
    }
}

function _onDocumentMouseMove(e) {
    if (_isPanning) {
        if ((e.buttons & MIDDLE_MOUSE_BUTTON_MASK) === NO_MOUSE_BUTTONS) {
            // The button was released where we didn't get the mouseup event, e.g. outside of the browser window.
            _handlePanEnd();
        } else {
            _handlePanning(e);
        }

        return;
    }

    if (!_isValidMouseMoveTarget(e)) {
        return;
    }

    _updateElementIfChanged(e);

    let coords = _getMapTileCoords(e.clientX, e.clientY);
    let x = Math.max(0, Math.min(255, coords.x));
    let y = Math.max(0, Math.min(255, coords.y));

    _updateCoordsLabel(x, y);
    _hasMoved = true;

    _applyMouseMoveAction(x, y);
}

function _onDocumentMouseUp(e) {
    if (_isPanning) {
        e.preventDefault();
        _handlePanEnd();
        return;
    }

    if (!_element) {
        return;
    }

    _handleDragOrResizeEnd();

    _isDragging = false;
    _isResizing = false;
    _hasMoved = false;
}

function _onDocumentKeyDown(e) {
    if (e.key !== "Delete" && e.key !== "Del") {
        return;
    }

    // Don't interfere with text editing in input/textarea/select elements.
    const tag = e.target.tagName;
    if (tag === "INPUT" || tag === "TEXTAREA") {
        return;
    }

    e.preventDefault();
    if (_dotNetRef) {
        _dotNetRef.invokeMethodAsync("OnDeleteKeyPressed");
    }
}

/**
 * Initializes the map editor for the given host element.
 * @param {HTMLElement} element - The map host element.
 * @param {object} dotNetRef - JSInvokable reference to the Blazor component.
 * @param {number} initialZoom - The initial zoom level to apply.
 * @param {number} baseScale - The base pixel scale factor for the map.
 * @param {boolean} applyInitialZoom - Whether the zoom should be reset to the initial zoom level.
 */
export function initialize(element, dotNetRef, initialZoom, baseScale, applyInitialZoom) {
    if (!element) {
        return;
    }

    _element = element;
    _dotNetRef = dotNetRef;
    _baseScale = baseScale || _baseScale;
    if (applyInitialZoom !== false) {
        _defaultZoom = initialZoom;
        _zoomLevel = _defaultZoom;
    }

    // Also re-applied after DOM replacement, as the Blazor render may have created a new content element.
    _applyZoom(_zoomLevel);

    document.removeEventListener("mousedown", _onDocumentMouseDown);
    document.removeEventListener("mousemove", _onDocumentMouseMove);
    document.removeEventListener("mouseup", _onDocumentMouseUp);
    document.removeEventListener("keydown", _onDocumentKeyDown);
    document.removeEventListener("wheel", _onDocumentWheel);
    document.addEventListener("wheel", _onDocumentWheel, { passive: false });
    document.addEventListener("mousedown", _onDocumentMouseDown);
    document.addEventListener("mousemove", _onDocumentMouseMove);
    document.addEventListener("mouseup", _onDocumentMouseUp);
    document.addEventListener("keydown", _onDocumentKeyDown);
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
 * Zooms the map to a specific level, keeping the viewport center stationary.
 * @param {HTMLElement} element - The map host element.
 * @param {number} newZoom - The target zoom level.
 * @returns {Number} The applied zoom level.
 */
export function zoomTo(element, newZoom) {
    if (!element) {
        return _defaultZoom;
    }

    const clampedZoom = Math.max(MIN_ZOOM, Math.min(MAX_ZOOM, newZoom));
    const rect = element.getBoundingClientRect();
    _element = element;
    _zoomAround(clampedZoom, rect.width / 2, rect.height / 2);

    return clampedZoom;
}

/**
 * Resets the zoom level to 1.0 and scrolls the map back to the origin.
 * @param {HTMLElement} element - The map host element.
 * @returns {number} The reset zoom level (always 1.0).
 */
export function resetZoom(element) {
    if (!element) {
        return _defaultZoom;
    }

    _element = element;
    _wheelFactor = 1;
    _zoomLevel = _defaultZoom;
    _applyZoom(_defaultZoom);
    element.scrollLeft = 0;
    element.scrollTop = 0;

    return _defaultZoom;
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
 * Cleans up the state associated with the map editor module.
 * Called when the Blazor component is disposed.
 */
export function dispose() {
    clearTimeout(_zoomNotifyTimer);
    if (_wheelFrame) {
        cancelAnimationFrame(_wheelFrame);
        _wheelFrame = 0;
    }

    _wheelFactor = 1;
    document.removeEventListener("wheel", _onDocumentWheel);
    document.removeEventListener("mousedown", _onDocumentMouseDown);
    document.removeEventListener("mousemove", _onDocumentMouseMove);
    document.removeEventListener("mouseup", _onDocumentMouseUp);
    document.removeEventListener("keydown", _onDocumentKeyDown);
    _element = null;
    _dotNetRef = null;
    _isDragging = false;
    _isPanning = false;
    _isResizing = false;
    _hasMoved = false;
}
