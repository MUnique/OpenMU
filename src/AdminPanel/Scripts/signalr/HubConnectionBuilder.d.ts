import { HubConnection } from "./HubConnection";
import { IHttpConnectionOptions } from "./IHttpConnectionOptions";
import { IHubProtocol } from "./IHubProtocol";
import { ILogger, LogLevel } from "./ILogger";
import { HttpTransportType } from "./ITransport";
/** A builder for configuring {@link HubConnection} instances. */
export declare class HubConnectionBuilder {
    /** Configures console logging for the {@link HubConnection}.
     *
     * @param {LogLevel} logLevel The minimum level of messages to log. Anything at this level, or a more severe level, will be logged.
     * @returns The {@link HubConnectionBuilder} instance, for chaining.
     */
    configureLogging(logLevel: LogLevel): HubConnectionBuilder;
    /** Configures custom logging for the {@link HubConnection}.
     *
     * @param {ILogger} logger An object implementing the {@link ILogger} interface, which will be used to write all log messages.
     * @returns The {@link HubConnectionBuilder} instance, for chaining.
     */
    configureLogging(logger: ILogger): HubConnectionBuilder;
    /** Configures the {@link HubConnection} to use HTTP-based transports to connect to the specified URL.
     *
     * The transport will be selected automatically based on what the server and client support.
     *
     * @param {string} url The URL the connection will use.
     * @returns The {@link HubConnectionBuilder} instance, for chaining.
     */
    withUrl(url: string): HubConnectionBuilder;
    /** Configures the {@link HubConnection} to use the specified HTTP-based transport to connect to the specified URL.
     *
     * @param {string} url The URL the connection will use.
     * @param {HttpTransportType} transportType The specific transport to use.
     * @returns The {@link HubConnectionBuilder} instance, for chaining.
     */
    withUrl(url: string, transportType: HttpTransportType): HubConnectionBuilder;
    /** Configures the {@link HubConnection} to use HTTP-based transports to connect to the specified URL.
     *
     * @param {string} url The URL the connection will use.
     * @param {IHttpConnectionOptions} options An options object used to configure the connection.
     * @returns The {@link HubConnectionBuilder} instance, for chaining.
     */
    withUrl(url: string, options: IHttpConnectionOptions): HubConnectionBuilder;
    /** Configures the {@link HubConnection} to use the specified Hub Protocol.
     *
     * @param {IHubProtocol} protocol The {@link IHubProtocol} implementation to use.
     */
    withHubProtocol(protocol: IHubProtocol): HubConnectionBuilder;
    /** Creates a {@link HubConnection} from the configuration options specified in this builder.
     *
     * @returns {HubConnection} The configured {@link HubConnection}.
     */
    build(): HubConnection;
}
