import { IStreamResult } from "./Stream";
/** Represents a connection to a SignalR Hub. */
export declare class HubConnection {
    private readonly connection;
    private readonly logger;
    private protocol;
    private handshakeProtocol;
    private callbacks;
    private methods;
    private id;
    private closedCallbacks;
    private timeoutHandle;
    private receivedHandshakeResponse;
    /** The server timeout in milliseconds.
     *
     * If this timeout elapses without receiving any messages from the server, the connection will be terminated with an error.
     * The default timeout value is 30,000 milliseconds (30 seconds).
     */
    serverTimeoutInMilliseconds: number;
    private constructor();
    /** Starts the connection.
     *
     * @returns {Promise<void>} A Promise that resolves when the connection has been successfully established, or rejects with an error.
     */
    start(): Promise<void>;
    /** Stops the connection.
     *
     * @returns {Promise<void>} A Promise that resolves when the connection has been successfully terminated, or rejects with an error.
     */
    stop(): Promise<void>;
    /** Invokes a streaming hub method on the server using the specified name and arguments.
     *
     * @typeparam T The type of the items returned by the server.
     * @param {string} methodName The name of the server method to invoke.
     * @param {any[]} args The arguments used to invoke the server method.
     * @returns {IStreamResult<T>} An object that yields results from the server as they are received.
     */
    stream<T = any>(methodName: string, ...args: any[]): IStreamResult<T>;
    /** Invokes a hub method on the server using the specified name and arguments. Does not wait for a response from the receiver.
     *
     * The Promise returned by this method resolves when the client has sent the invocation to the server. The server may still
     * be processing the invocation.
     *
     * @param {string} methodName The name of the server method to invoke.
     * @param {any[]} args The arguments used to invoke the server method.
     * @returns {Promise<void>} A Promise that resolves when the invocation has been successfully sent, or rejects with an error.
     */
    send(methodName: string, ...args: any[]): Promise<void>;
    /** Invokes a hub method on the server using the specified name and arguments.
     *
     * The Promise returned by this method resolves when the server indicates it has finished invoking the method. When the promise
     * resolves, the server has finished invoking the method. If the server method returns a result, it is produced as the result of
     * resolving the Promise.
     *
     * @typeparam T The expected return type.
     * @param {string} methodName The name of the server method to invoke.
     * @param {any[]} args The arguments used to invoke the server method.
     * @returns {Promise<T>} A Promise that resolves with the result of the server method (if any), or rejects with an error.
     */
    invoke<T = any>(methodName: string, ...args: any[]): Promise<T>;
    /** Registers a handler that will be invoked when the hub method with the specified method name is invoked.
     *
     * @param {string} methodName The name of the hub method to define.
     * @param {Function} newMethod The handler that will be raised when the hub method is invoked.
     */
    on(methodName: string, newMethod: (...args: any[]) => void): void;
    /** Removes all handlers for the specified hub method.
     *
     * @param {string} methodName The name of the method to remove handlers for.
     */
    off(methodName: string): void;
    /** Removes the specified handler for the specified hub method.
     *
     * You must pass the exact same Function instance as was previously passed to {@link on}. Passing a different instance (even if the function
     * body is the same) will not remove the handler.
     *
     * @param {string} methodName The name of the method to remove handlers for.
     * @param {Function} method The handler to remove. This must be the same Function instance as the one passed to {@link on}.
     */
    off(methodName: string, method: (...args: any[]) => void): void;
    /** Registers a handler that will be invoked when the connection is closed.
     *
     * @param {Function} callback The handler that will be invoked when the connection is closed. Optionally receives a single argument containing the error that caused the connection to close (if any).
     */
    onclose(callback: (error?: Error) => void): void;
    private processIncomingData(data);
    private processHandshakeResponse(data);
    private configureTimeout();
    private serverTimeout();
    private invokeClientMethod(invocationMessage);
    private connectionClosed(error?);
    private cleanupTimeout();
    private createInvocation(methodName, args, nonblocking);
    private createStreamInvocation(methodName, args);
    private createCancelInvocation(id);
}
