export function scrollToBottom(elementId) {
    const el = document.getElementById(elementId);
    if (el) {
        el.scrollTop = el.scrollHeight;
    }
}

export function isScrolledToBottom(elementId) {
    const el = document.getElementById(elementId);
    if (el) {
        return Math.abs(el.scrollHeight - el.clientHeight - el.scrollTop) < 50;
    }
    return true;
}
