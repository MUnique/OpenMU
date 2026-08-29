// The scrollable elements which are observed, with their last reported state.
const observedElements = new WeakMap();

// A tolerance of about one row, so that the view still counts as being at the bottom when the
// user scrolled almost to the end.
const bottomTolerance = 32;

// Reports to the component whether the element is scrolled to its bottom. While it isn't, the
// component stops taking new packets, so that the content doesn't move away under the user.
export function observe(element, componentReference) {
    if (!element || observedElements.has(element)) {
        return;
    }

    const state = { isAtBottom: true, componentReference: componentReference };
    observedElements.set(element, state);
    element.addEventListener(
        'scroll',
        () => {
            const isAtBottom = element.scrollHeight - element.clientHeight - element.scrollTop < bottomTolerance;
            if (isAtBottom !== state.isAtBottom) {
                state.isAtBottom = isAtBottom;
                state.componentReference.invokeMethodAsync('SetAtBottomAsync', isAtBottom);
            }
        },
        { passive: true });
}

// Scrolls the given element to its bottom, so that the newest packet is visible.
export function scrollToBottom(element) {
    if (!element) {
        return;
    }

    element.scrollTop = element.scrollHeight;
    const state = observedElements.get(element);
    if (state) {
        state.isAtBottom = true;
    }
}
