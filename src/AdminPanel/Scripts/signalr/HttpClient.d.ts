import { AbortSignal } from "./AbortController";
import { ILogger } from "./ILogger";
/** Represents an HTTP request. */
export interface HttpRequest {
    /** The HTTP method to use for the request. */
    method?: string;
    /** The URL for the request. */
    url?: string;
    /** The body content for the request. May be a string or an ArrayBuffer (for binary data). */
    content?: string | ArrayBuffer;
    /** An object describing headers to apply to the request. */
    headers?: {
        [key: string]: string;
    };
    /** The XMLHttpRequestResponseType to apply to the request. */
    responseType?: XMLHttpRequestResponseType;
    /** An AbortSignal that can be monitored for cancellation. */
    abortSignal?: AbortSignal;
    /** The time to wait for the request to complete before throwing a TimeoutError. Measured in milliseconds. */
    timeout?: number;
}
/** Represents an HTTP response. */
export declare class HttpResponse {
    readonly statusCode: number;
    readonly statusText: string;
    readonly content: string | ArrayBuffer;
    /** Constructs a new instance of {@link HttpResponse} with the specified status code.
     *
     * @param {number} statusCode The status code of the response.
     */
    constructor(statusCode: number);
    /** Constructs a new instance of {@link HttpResponse} with the specified status code and message.
     *
     * @param {number} statusCode The status code of the response.
     * @param {string} statusText The status message of the response.
     */
    constructor(statusCode: number, statusText: string);
    /** Constructs a new instance of {@link HttpResponse} with the specified status code, message and string content.
     *
     * @param {number} statusCode The status code of the response.
     * @param {string} statusText The status message of the response.
     * @param {string} content The content of the response.
     */
    constructor(statusCode: number, statusText: string, content: string);
    /** Constructs a new instance of {@link HttpResponse} with the specified status code, message and binary content.
     *
     * @param {number} statusCode The status code of the response.
     * @param {string} statusText The status message of the response.
     * @param {ArrayBuffer} content The content of the response.
     */
    constructor(statusCode: number, statusText: string, content: ArrayBuffer);
}
/** Abstraction over an HTTP client.
 *
 * This class provides an abstraction over an HTTP client so that a different implementation can be provided on different platforms.
 */
export declare abstract class HttpClient {
    /** Issues an HTTP GET request to the specified URL, returning a Promise that resolves with an {@link HttpResponse} representing the result.
     *
     * @param {string} url The URL for the request.
     * @returns {Promise<HttpResponse>} A Promise that resolves with an {@link HttpResponse} describing the response, or rejects with an Error indicating a failure.
     */
    get(url: string): Promise<HttpResponse>;
    /** Issues an HTTP GET request to the specified URL, returning a Promise that resolves with an {@link HttpResponse} representing the result.
     *
     * @param {string} url The URL for the request.
     * @param {HttpRequest} options Additional options to configure the request. The 'url' field in this object will be overridden by the url parameter.
     * @returns {Promise<HttpResponse>} A Promise that resolves with an {@link HttpResponse} describing the response, or rejects with an Error indicating a failure.
     */
    get(url: string, options: HttpRequest): Promise<HttpResponse>;
    /** Issues an HTTP POST request to the specified URL, returning a Promise that resolves with an {@link HttpResponse} representing the result.
     *
     * @param {string} url The URL for the request.
     * @returns {Promise<HttpResponse>} A Promise that resolves with an {@link HttpResponse} describing the response, or rejects with an Error indicating a failure.
     */
    post(url: string): Promise<HttpResponse>;
    /** Issues an HTTP POST request to the specified URL, returning a Promise that resolves with an {@link HttpResponse} representing the result.
     *
     * @param {string} url The URL for the request.
     * @param {HttpRequest} options Additional options to configure the request. The 'url' field in this object will be overridden by the url parameter.
     * @returns {Promise<HttpResponse>} A Promise that resolves with an {@link HttpResponse} describing the response, or rejects with an Error indicating a failure.
     */
    post(url: string, options: HttpRequest): Promise<HttpResponse>;
    /** Issues an HTTP DELETE request to the specified URL, returning a Promise that resolves with an {@link HttpResponse} representing the result.
     *
     * @param {string} url The URL for the request.
     * @returns {Promise<HttpResponse>} A Promise that resolves with an {@link HttpResponse} describing the response, or rejects with an Error indicating a failure.
     */
    delete(url: string): Promise<HttpResponse>;
    /** Issues an HTTP DELETE request to the specified URL, returning a Promise that resolves with an {@link HttpResponse} representing the result.
     *
     * @param {string} url The URL for the request.
     * @param {HttpRequest} options Additional options to configure the request. The 'url' field in this object will be overridden by the url parameter.
     * @returns {Promise<HttpResponse>} A Promise that resolves with an {@link HttpResponse} describing the response, or rejects with an Error indicating a failure.
     */
    delete(url: string, options: HttpRequest): Promise<HttpResponse>;
    /** Issues an HTTP request to the specified URL, returning a {@link Promise} that resolves with an {@link HttpResponse} representing the result.
     *
     * @param {HttpRequest} request An {@link HttpRequest} describing the request to send.
     * @returns {Promise<HttpResponse>} A Promise that resolves with an HttpResponse describing the response, or rejects with an Error indicating a failure.
     */
    abstract send(request: HttpRequest): Promise<HttpResponse>;
}
/** Default implementation of {@link HttpClient}. */
export declare class DefaultHttpClient extends HttpClient {
    private readonly logger;
    /** Creates a new instance of the {@link DefaultHttpClient}, using the provided {@link ILogger} to log messages. */
    constructor(logger: ILogger);
    /** @inheritDoc */
    send(request: HttpRequest): Promise<HttpResponse>;
}
