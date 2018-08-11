export declare class AbortController implements AbortSignal {
    private isAborted;
    onabort: () => void;
    abort(): void;
    readonly signal: AbortSignal;
    readonly aborted: boolean;
}
/** Represents a signal that can be monitored to determine if a request has been aborted. */
export interface AbortSignal {
    /** Indicates if the request has been aborted. */
    aborted: boolean;
    /** Set this to a handler that will be invoked when the request is aborted. */
    onabort: () => void;
}
