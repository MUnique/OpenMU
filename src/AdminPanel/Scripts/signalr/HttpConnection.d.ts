import { IConnection } from "./IConnection";
import { IHttpConnectionOptions } from "./IHttpConnectionOptions";
import { HttpTransportType, TransferFormat } from "./ITransport";
export interface INegotiateResponse {
    connectionId?: string;
    availableTransports?: IAvailableTransport[];
    url?: string;
    accessToken?: string;
}
export interface IAvailableTransport {
    transport: keyof typeof HttpTransportType;
    transferFormats: Array<keyof typeof TransferFormat>;
}
export declare class HttpConnection implements IConnection {
    private connectionState;
    private baseUrl;
    private readonly httpClient;
    private readonly logger;
    private readonly options;
    private transport;
    private startPromise;
    private stopError?;
    private accessTokenFactory?;
    readonly features: any;
    onreceive: (data: string | ArrayBuffer) => void;
    onclose: (e?: Error) => void;
    constructor(url: string, options?: IHttpConnectionOptions);
    start(): Promise<void>;
    start(transferFormat: TransferFormat): Promise<void>;
    send(data: string | ArrayBuffer): Promise<void>;
    stop(error?: Error): Promise<void>;
    private startInternal(transferFormat);
    private getNegotiationResponse(url);
    private createConnectUrl(url, connectionId);
    private createTransport(url, requestedTransport, negotiateResponse, requestedTransferFormat);
    private constructTransport(transport);
    private resolveTransport(endpoint, requestedTransport, requestedTransferFormat);
    private isITransport(transport);
    private changeState(from, to);
    private stopConnection(error?);
    private resolveUrl(url);
    private resolveNegotiateUrl(url);
}
