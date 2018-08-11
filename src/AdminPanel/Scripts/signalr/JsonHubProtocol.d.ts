import { HubMessage, IHubProtocol } from "./IHubProtocol";
import { ILogger } from "./ILogger";
import { TransferFormat } from "./ITransport";
/** Implements the JSON Hub Protocol. */
export declare class JsonHubProtocol implements IHubProtocol {
    /** @inheritDoc */
    readonly name: string;
    /** @inheritDoc */
    readonly version: number;
    /** @inheritDoc */
    readonly transferFormat: TransferFormat;
    /** Creates an array of {@link HubMessage} objects from the specified serialized representation.
     *
     * @param {string} input A string containing the serialized representation.
     * @param {ILogger} logger A logger that will be used to log messages that occur during parsing.
     */
    parseMessages(input: string, logger: ILogger): HubMessage[];
    /** Writes the specified {@link HubMessage} to a string and returns it.
     *
     * @param {HubMessage} message The message to write.
     * @returns {string} A string containing the serialized representation of the message.
     */
    writeMessage(message: HubMessage): string;
    private isInvocationMessage(message);
    private isStreamItemMessage(message);
    private isCompletionMessage(message);
    private assertNotEmptyString(value, errorMessage);
}
