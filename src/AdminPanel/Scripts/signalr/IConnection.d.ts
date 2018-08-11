import { TransferFormat } from "./ITransport";
export interface IConnection {
    readonly features: any;
    start(transferFormat: TransferFormat): Promise<void>;
    send(data: string | ArrayBuffer): Promise<void>;
    stop(error?: Error): Promise<void>;
    onreceive: (data: string | ArrayBuffer) => void;
    onclose: (error?: Error) => void;
}
