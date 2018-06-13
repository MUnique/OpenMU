// ReSharper disable InconsistentNaming naming is caused by C# classes which are serialized like that.

export interface LogEntryData {
    readonly Id: number;
    readonly TimeStamp: string;
    readonly LoggerName: string;
    readonly Message: string;
    readonly ExceptionString: string;
    readonly ThreadName: string;
    readonly Level: LogLevel;
    readonly Properties: { [key: string]: string };
    readonly LocationInfo: LogLocation;
}
export interface LogLevel {
    readonly Name: string;
}

export interface LogLocation extends StackLocation {
    readonly MethodName: string;
    readonly StackFrames: StackLocation[];
}

export interface StackLocation {
    readonly ClassName: string;
    readonly FileName: string;
    readonly LineNumber: number;
    readonly FullInfo: string;
}