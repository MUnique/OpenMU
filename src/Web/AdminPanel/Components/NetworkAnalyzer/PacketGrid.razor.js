// Scrolls the given element to its bottom, so that the newest packet is visible.
export function scrollToBottom(element) {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}
