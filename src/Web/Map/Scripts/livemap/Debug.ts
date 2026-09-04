/*
 * Debug logging for the live map.
 *
 * The per-object messages are disabled by default: the browser console keeps
 * live references to everything which is logged to it, so a message for every
 * added, updated and removed object would pin each object which ever appeared
 * on the map for the lifetime of the console buffer.
 *
 * To enable them, set 'liveMapDebugLogging' to true on the window object,
 * e.g. by entering 'liveMapDebugLogging = true' in the browser console.
 */

interface DebugWindow extends Window {
    liveMapDebugLogging?: boolean;
}

/**
 * Gets a value indicating whether the verbose debug logging is enabled.
 */
export function isDebugLoggingEnabled(): boolean {
    return (window as DebugWindow).liveMapDebugLogging === true;
}

/**
 * Writes a debug message to the console, if the debug logging is enabled.
 * @param message - The message.
 * @param parameters - The additional parameters which are logged with the message.
 */
export function logDebug(message: string, ...parameters: any[]): void {
    if (isDebugLoggingEnabled()) {
        console.debug(message, ...parameters);
    }
}
