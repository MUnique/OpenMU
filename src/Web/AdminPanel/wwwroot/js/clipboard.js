// Copies the given text to the clipboard. Returns false when the browser refuses it, which
// happens without a secure context - the caller then leaves the value on screen to select.
export async function copyText(text) {
    try {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
            return true;
        }
    } catch {
        // Fall through to the fallback below.
    }

    try {
        const textArea = document.createElement('textarea');
        textArea.value = text;
        textArea.setAttribute('readonly', '');
        textArea.style.position = 'absolute';
        textArea.style.left = '-9999px';
        document.body.appendChild(textArea);
        textArea.select();
        const copied = document.execCommand('copy');
        document.body.removeChild(textArea);
        return copied;
    } catch {
        return false;
    }
}

// Selects the text of the given input, so the user can copy it by hand when the clipboard
// isn't available.
export function selectElementText(element) {
    if (element && typeof element.select === 'function') {
        element.select();
    }
}
