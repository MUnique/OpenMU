// The elements which are scrolled automatically, with the information whether their content
// should stick to the bottom.
const trackedElements = new WeakMap();

// Scrolls the given element to its bottom, as long as the user didn't scroll up. That way, new
// packets don't pull the view away while somebody is looking at the older ones.
export function scrollToBottom(element) {
    if (!element) {
        return;
    }

    const state = track(element);
    if (state.stickToBottom) {
        element.scrollTop = element.scrollHeight;
    }
}

function track(element) {
    let state = trackedElements.get(element);
    if (state) {
        return state;
    }

    state = { stickToBottom: true };
    trackedElements.set(element, state);
    element.addEventListener(
        'scroll',
        () => {
            // A tolerance of about one row, so that the view keeps sticking when the user
            // scrolled almost to the bottom.
            state.stickToBottom = element.scrollHeight - element.clientHeight - element.scrollTop < 32;
        },
        { passive: true });

    return state;
}
