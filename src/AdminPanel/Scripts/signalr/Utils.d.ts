import { HttpClient } from "./HttpClient";
import { ILogger, LogLevel } from "./ILogger";
import { IStreamResult, IStreamSubscriber, ISubscription } from "./Stream";
export declare class Arg {
    static isRequired(val: any, name: string): void;
    static isIn(val: any, values: any, name: string): void;
}
export declare function getDataDetail(data: any, includeContent: boolean): string;
export declare function formatArrayBuffer(data: ArrayBuffer): string;
export declare function sendMessage(logger: ILogger, transportName: string, httpClient: HttpClient, url: string, accessTokenFactory: () => string | Promise<string>, content: string | ArrayBuffer, logMessageContent: boolean): Promise<void>;
export declare function createLogger(logger?: ILogger | LogLevel): ILogger;
export declare class Subject<T> implements IStreamResult<T> {
    observers: Array<IStreamSubscriber<T>>;
    cancelCallback: () => Promise<void>;
    constructor(cancelCallback: () => Promise<void>);
    next(item: T): void;
    error(err: any): void;
    complete(): void;
    subscribe(observer: IStreamSubscriber<T>): ISubscription<T>;
}
export declare class SubjectSubscription<T> implements ISubscription<T> {
    private subject;
    private observer;
    constructor(subject: Subject<T>, observer: IStreamSubscriber<T>);
    dispose(): void;
}
export declare class ConsoleLogger implements ILogger {
    private readonly minimumLogLevel;
    constructor(minimumLogLevel: LogLevel);
    log(logLevel: LogLevel, message: string): void;
}
