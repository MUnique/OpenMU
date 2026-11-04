let activeContainer;
let scrollTarget;
let targetSpeed = 0;
let previousFrameTime = 0;
let animationFrame = 0;

const edgeSize = 72;
const maximumSpeed = 720;

function findScrollTarget(element) {
    for (let current = element.parentElement; current; current = current.parentElement) {
        const overflowY = getComputedStyle(current).overflowY;
        if ((overflowY === "auto" || overflowY === "scroll" || overflowY === "overlay")
            && current.scrollHeight > current.clientHeight) {
            return current;
        }
    }

    return document.scrollingElement ?? document.documentElement;
}

function getScrollBounds() {
    if (scrollTarget === document.scrollingElement
        || scrollTarget === document.documentElement
        || scrollTarget === document.body) {
        return { top: 0, bottom: window.innerHeight };
    }

    const bounds = scrollTarget.getBoundingClientRect();
    return { top: bounds.top, bottom: bounds.bottom };
}

function updateScrollSpeed(event) {
    if (!activeContainer || !scrollTarget) {
        targetSpeed = 0;
        return;
    }

    const containerBounds = activeContainer.getBoundingClientRect();
    if (event.clientX < containerBounds.left || event.clientX > containerBounds.right) {
        targetSpeed = 0;
        return;
    }

    const bounds = getScrollBounds();
    const distanceFromTop = event.clientY - bounds.top;
    const distanceFromBottom = bounds.bottom - event.clientY;

    if (distanceFromTop < edgeSize && distanceFromTop > -edgeSize) {
        const intensity = Math.min(1, (edgeSize - distanceFromTop) / edgeSize);
        targetSpeed = -maximumSpeed * intensity;
    } else if (distanceFromBottom < edgeSize && distanceFromBottom > -edgeSize) {
        const intensity = Math.min(1, (edgeSize - distanceFromBottom) / edgeSize);
        targetSpeed = maximumSpeed * intensity;
    } else {
        targetSpeed = 0;
    }

    if (targetSpeed !== 0 && animationFrame === 0) {
        animationFrame = requestAnimationFrame(scrollFrame);
    }
}

function scrollFrame(timestamp) {
    animationFrame = 0;
    if (targetSpeed === 0 || !scrollTarget) {
        previousFrameTime = 0;
        return;
    }

    const elapsed = previousFrameTime === 0 ? 16 : Math.min(timestamp - previousFrameTime, 40);
    previousFrameTime = timestamp;

    const previousScrollTop = scrollTarget.scrollTop;
    scrollTarget.scrollTop += targetSpeed * elapsed / 1000;

    if (scrollTarget.scrollTop === previousScrollTop) {
        targetSpeed = 0;
        previousFrameTime = 0;
        return;
    }

    animationFrame = requestAnimationFrame(scrollFrame);
}

function stop() {
    document.removeEventListener("dragover", updateScrollSpeed, true);
    document.removeEventListener("dragend", stop, true);
    document.removeEventListener("drop", stop, true);

    targetSpeed = 0;
    previousFrameTime = 0;
    if (animationFrame !== 0) {
        cancelAnimationFrame(animationFrame);
        animationFrame = 0;
    }

    activeContainer = null;
    scrollTarget = null;
}

export function start(container) {
    stop();
    activeContainer = container;
    scrollTarget = findScrollTarget(container);

    document.addEventListener("dragover", updateScrollSpeed, true);
    document.addEventListener("dragend", stop, true);
    document.addEventListener("drop", stop, true);
}

export { stop };
