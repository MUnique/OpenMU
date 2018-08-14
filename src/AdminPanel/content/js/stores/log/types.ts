
export interface LogEventData {
    readonly domain: string;
    readonly exceptionString: string;
    readonly identity: string;
    readonly level: LogLevel;
    readonly logLocation: LogLocation;
    readonly loggerName: string;
    readonly message: string;
    readonly properties: { [key: string]: string };
    readonly threadName: string;
    readonly timeStamp: string;
    readonly userName: string;
}

export interface LogLevel {
    readonly name: string;
    readonly displayName: string;
    readonly value: number;
}

export interface LogLocation {
    readonly methodName: string;
    readonly className: string;
    readonly fileName: string;
    readonly lineNumber: number;
    readonly fullInfo: string;
}

export interface LogEventArgs {
    formattedEvent: string;
    loggingEvent: LogEventData;
    id: number;
}