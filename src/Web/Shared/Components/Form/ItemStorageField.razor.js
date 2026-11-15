let activeContainer;
let scrollTarget;
let targetSpeed = 0;
let previousFrameTime = 0;
let animationFrame = 0;

const edgeSize = 72;
const maximumSpeed = 720;

function isScrollable(element) {
    const overflowY = getComputedStyle(element).overflowY;
    return ["auto", "scroll", "overlay"].includes(overflowY)
        && element.scrollHeight > element.clientHeight;
}

function findScrollTarget(element) {
    let current = element.parentElement;
    while (current) {
        if (isScrollable(current)) {
            return current;
        }

        current = current.parentElement;
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

function isPointerInsideContainer(event) {
    const bounds = activeContainer.getBoundingClientRect();
    return event.clientX >= bounds.left && event.clientX <= bounds.right;
}

function isWithinEdge(distance) {
    return distance < edgeSize && distance > -edgeSize;
}

function getEdgeIntensity(distance) {
    return Math.min(1, (edgeSize - distance) / edgeSize);
}

function getEdgeScrollSpeed(pointerY) {
    const bounds = getScrollBounds();
    const distanceFromTop = pointerY - bounds.top;
    const distanceFromBottom = bounds.bottom - pointerY;

    if (isWithinEdge(distanceFromTop)) {
        return -maximumSpeed * getEdgeIntensity(distanceFromTop);
    }

    if (isWithinEdge(distanceFromBottom)) {
        return maximumSpeed * getEdgeIntensity(distanceFromBottom);
    }

    return 0;
}

function isScrollActive() {
    return targetSpeed !== 0 && scrollTarget !== null;
}

function scrollFrame(timestamp) {
    animationFrame = 0;
    if (!isScrollActive()) {
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

function scheduleScrollFrame() {
    if (targetSpeed !== 0 && animationFrame === 0) {
        animationFrame = requestAnimationFrame(scrollFrame);
    }
}

function updateScrollSpeed(event) {
    if (!activeContainer || !scrollTarget || !isPointerInsideContainer(event)) {
        targetSpeed = 0;
        return;
    }

    targetSpeed = getEdgeScrollSpeed(event.clientY);
    scheduleScrollFrame();
}

export function stop() {
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
