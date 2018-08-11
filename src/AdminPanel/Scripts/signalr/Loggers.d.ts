import { ILogger, LogLevel } from "./ILogger";
/** A logger that does nothing when log messages are sent to it. */
export declare class NullLogger implements ILogger {
    /** The singleton instance of the {@link NullLogger}. */
    static instance: ILogger;
    private constructor();
    /** @inheritDoc */
    log(logLevel: LogLevel, message: string): void;
}
