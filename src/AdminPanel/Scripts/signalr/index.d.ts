/** The version of the SignalR client. */
export declare const VERSION: string;
export { AbortSignal } from "./AbortController";
export { HttpError, TimeoutError } from "./Errors";
export { DefaultHttpClient, HttpClient, HttpRequest, HttpResponse } from "./HttpClient";
export { IHttpConnectionOptions } from "./IHttpConnectionOptions";
export { HubConnection } from "./HubConnection";
export { HubConnectionBuilder } from "./HubConnectionBuilder";
export { MessageType, MessageHeaders, HubMessage, HubMessageBase, HubInvocationMessage, InvocationMessage, StreamInvocationMessage, StreamItemMessage, CompletionMessage, PingMessage, CloseMessage, CancelInvocationMessage, IHubProtocol } from "./IHubProtocol";
export { ILogger, LogLevel } from "./ILogger";
export { HttpTransportType, TransferFormat, ITransport } from "./ITransport";
export { IStreamSubscriber, IStreamResult, ISubscription } from "./Stream";
export { NullLogger } from "./Loggers";
export { JsonHubProtocol } from "./JsonHubProtocol";

export as namespace signalR;