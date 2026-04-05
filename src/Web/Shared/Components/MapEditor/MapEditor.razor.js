// <copyright file="MapEditor.razor.js" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

const BASE_SIZE = 768;
const MIN_ZOOM = 1.0;
const MAX_ZOOM = 4.0;

const _zoomLevels = new WeakMap();

function _getZoomLevel(element) {
    return _zoomLevels.get(element) ?? 1.0;
}

function _setZoomLevel(element, zoom) {
    _zoomLevels.set(element, zoom);
}

/**
 * Updates the zoom percentage label in the map host container.
 * @param {HTMLElement} element - The map host element.
 * @param {number} zoom - The current zoom level.
 */
function _updateZoomDisplay(element, zoom) {
    const container = element.closest(".map-host-container");
    if (!container) {
        return;
    }

    const zoomLabel = container.querySelector(".zoom-label");
    if (zoomLabel) {
        zoomLabel.textContent = Math.round(zoom * 100) + "%";
    }
}

/**
 * Applies a zoom level to the map content and updates the zoom display label.
 * @param {HTMLElement} element - The map host element.
 * @param {number} zoom - The zoom level to apply.
 */
function _applyZoom(element, zoom) {
    const content = element.querySelector(".map-content");
    const img = element.querySelector(".map-content img");

    if (!content) {
        return;
    }

    const size = Math.round(BASE_SIZE * zoom);
    content.style.width = size + "px";
    content.style.height = size + "px";

    if (img) {
        img.style.width = size + "px";
        img.style.height = size + "px";
    }

    _updateZoomDisplay(element, zoom);
}

/**
 * Initializes the map editor for the given host element.
 * @param dotNetRef - The .NET component reference (reserved for future callbacks).
 * @param {HTMLElement} element - The map host element.
 * @param {number} initialZoom - The initial zoom level to apply.
 */
export function initialize(dotNetRef, element, initialZoom) {
    if (!element) {
        return;
    }
    _setZoomLevel(element, initialZoom);
    _applyZoom(element, initialZoom);
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
 * Returns the current state of the map host element including its bounding rect,
 * scroll position, and zoom level.
 * @param {HTMLElement} element - The map host element.
 * @returns {{ rect: object, scroll: object, zoomLevel: number }}
 */
export function getState(element) {
    if (!element) {
        return {
            rect: { left: 0, top: 0, width: 0, height: 0 },
            scroll: { scrollLeft: 0, scrollTop: 0 },
            zoomLevel: 1.0
        };
    }

    const rect = element.getBoundingClientRect();

    return {
        rect: {
            left: rect.left,
            top: rect.top,
            width: rect.width,
            height: rect.height
        },
        scroll: {
            scrollLeft: element.scrollLeft,
            scrollTop: element.scrollTop
        },
        zoomLevel: _getZoomLevel(element)
    };
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
 * Handles a mouse wheel event to zoom the map in or out, keeping the point
 * under the cursor stationary.
 * @param {HTMLElement} element - The map host element.
 * @param {number} deltaY - The vertical scroll delta from the wheel event.
 * @param {number} clientX - The client X position of the cursor.
 * @param {number} clientY - The client Y position of the cursor.
 * @returns {{ zoomLevel: number, handled: boolean, scrollLeft?: number, scrollTop?: number }}
 */
export function handleWheel(element, deltaY, clientX, clientY) {
    if (!element) {
        return { zoomLevel: 1.0, handled: false };
    }

    const oldZoom = _getZoomLevel(element);
    const sensitivity = 0.001;
    const zoomDelta = -deltaY * sensitivity;
    const newZoom = Math.max(MIN_ZOOM, Math.min(MAX_ZOOM, oldZoom * (1.0 + zoomDelta)));

    const zoomThreshold = 1e-4;
    if (Math.abs(newZoom - oldZoom) < zoomThreshold) {
        return { zoomLevel: oldZoom, handled: false };
    }

    const rect = element.getBoundingClientRect();
    const mouseX = clientX - rect.left;
    const mouseY = clientY - rect.top;

    const baseX = (element.scrollLeft + mouseX) / oldZoom;
    const baseY = (element.scrollTop + mouseY) / oldZoom;

    _setZoomLevel(element, newZoom);
    _applyZoom(element, newZoom);

    const maxScrollLeft = Math.max(0, BASE_SIZE * newZoom - rect.width);
    const maxScrollTop = Math.max(0, BASE_SIZE * newZoom - rect.height);

    element.scrollLeft = Math.max(0, Math.min((baseX * newZoom) - mouseX, maxScrollLeft));
    element.scrollTop = Math.max(0, Math.min((baseY * newZoom) - mouseY, maxScrollTop));

    return {
        zoomLevel: newZoom,
        handled: true,
        scrollLeft: element.scrollLeft,
        scrollTop: element.scrollTop
    };
}

/**
 * Zooms the map to a specific level, keeping the viewport center stationary.
 * @param {HTMLElement} element - The map host element.
 * @param {number} newZoom - The target zoom level.
 * @returns {number} The applied zoom level.
 */
export function zoomTo(element, newZoom) {
    if (!element) {
        return 1.0;
    }

    const oldZoom = _getZoomLevel(element);
    const clampedZoom = Math.max(MIN_ZOOM, Math.min(MAX_ZOOM, newZoom));

    const zoomThreshold = 1e-4;
    if (Math.abs(clampedZoom - oldZoom) < zoomThreshold) {
        return oldZoom;
    }

    const rect = element.getBoundingClientRect();
    const centerX = rect.width / 2;
    const centerY = rect.height / 2;

    const baseX = (element.scrollLeft + centerX) / oldZoom;
    const baseY = (element.scrollTop + centerY) / oldZoom;

    _setZoomLevel(element, clampedZoom);
    _applyZoom(element, clampedZoom);

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

    _setZoomLevel(element, 1.0);
    _applyZoom(element, 1.0);
    element.scrollLeft = 0;
    element.scrollTop = 0;

    return 1.0;
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

    const zoom = _getZoomLevel(element);
    const scale = baseScale * zoom;

    const pixelX = mapY * scale;
    const pixelY = mapX * scale;

    const maxScrollLeft = element.scrollWidth - element.clientWidth;
    const maxScrollTop = element.scrollHeight - element.clientHeight;

    element.scrollTo({
        left: Math.max(0, Math.min(pixelX - (element.clientWidth / 2), maxScrollLeft)),
        top: Math.max(0, Math.min(pixelY - (element.clientHeight / 2), maxScrollTop)),
        behavior: "smooth"
    });
}

/**
 * Cleans up state associated with the map editor module.
 * Called when the Blazor component is disposed.
 */
export function dispose() {
    // WeakMap entries are automatically cleaned up when their element keys
    // are garbage collected, so no explicit cleanup is required here.
}