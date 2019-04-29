import React from "react";
import { connect } from "react-redux";
import { ApplicationState } from "../stores/index";
import { ConnectServerSettings, GameClientDefinition } from "content/js/stores/servers/types";
import { saveConnectServerConfiguration } from "content/js/stores/servers/actions";

export interface IConnectServerConfigurationProps {
    serverSettings: ConnectServerSettings;
    clients: GameClientDefinition[];
    save: (settings: ConnectServerSettings, onSaveSuccess: () => void, onSaveError: (error: any) => void) => Promise<void>;
    onSaveSuccess(): void;
    onCancel(): void;
}

export interface IConnectServerConfigurationState extends ConnectServerSettings {

}

class ConnectServerConfiguration extends React.Component<IConnectServerConfigurationProps, IConnectServerConfigurationState> {

    constructor(props: IConnectServerConfigurationProps) {
        super(props);
        let newState = {};
        Object.assign(newState, this.props.serverSettings);
        this.state = newState as IConnectServerConfigurationState;
    }

    componentDidMount(): void {
        this.setState(this.props.serverSettings);
    }

    public render() {
        let clients: JSX.Element[] = this.props.clients.map(c => (<option value={c.id} selected={this.state.gameClient !== null && this.state.gameClient.id === c.id}>{c.description}</option>));
        return (
        <div className="panel panel-body panel-default">
            <form onSubmit={(event: any) => this.submit(event)}>

                <div className="form-group">
                    <label htmlFor="descriptionInput">Description</label>
                    <input id="descriptionInput" className="form-control" type="text" placeholder="Enter Description" value={this.state.description} onChange={(e: InputFormEvent) => this.setState({description: e.target.value})}/>
                </div>
                <div className="form-group">
                    <label htmlFor="clientListenerPortInput">Client listener port</label>
                    <input id="clientListenerPortInput" className="form-control" type="number" min="1" max="65535" value={this.state.clientListenerPort} onChange={(e: any) =>this.setState({clientListenerPort: e.target.value})}/>
                    </div>
                <div className="form-group">
                    <label htmlFor="gameClientInput">Game Client</label>
                    <select id="gameClientInput" className="form-control" required={true} onChange={(e: any) => this.setState({ gameClient: this.props.clients.find(c => c.id === e.target.value) })}>
                        {clients}
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="patchVersionInput">Patch-Version</label>
                    <div className="input-group" id="patchVersionInput">
                        <span className="input-group-addon version-addon">Major</span>
                        <input id="patchVersionMajorInput" className="form-control" type="number" min="0" max="255" value={atob(this.state.currentPatchVersion).charCodeAt(0)} onChange={(e: InputFormEvent) => { this.setState({ currentPatchVersion: this.copyArrayAndChangeIndex(this.state.currentPatchVersion, 0, parseInt(e.target.value)) }) }} />
                    </div>
                    <div className="input-group">
                        <span className="input-group-addon version-addon">Minor</span>
                        <input id="patchVersionMinorInput" className="form-control" type="number" min="0" max="255" value={atob(this.state.currentPatchVersion).charCodeAt(1)} onChange={(e: InputFormEvent) => { this.setState({ currentPatchVersion: this.copyArrayAndChangeIndex(this.state.currentPatchVersion, 1, parseInt(e.target.value)) }) }} />                                
                    </div>
                    <div className="input-group">
                        <span className="input-group-addon version-addon">Patch</span>
                        <input id="patchVersionPatchInput" className="form-control" type="number" min="0" max="255" value={atob(this.state.currentPatchVersion).charCodeAt(2)} onChange={(e: InputFormEvent) => { this.setState({ currentPatchVersion: this.copyArrayAndChangeIndex(this.state.currentPatchVersion, 2, parseInt(e.target.value)) }) }} />
                    </div>
                </div>
                <div className="form-group">
                    <label htmlFor="patchAddressInput">Patch-Address</label>
                    <div className="input-group" id="patchAddressInput">
                        <span className="input-group-addon">ftp://</span>
                        <input className="form-control" type="text" value={this.state.patchAddress} onChange={(e: InputFormEvent) => this.setState({ patchAddress: e.target.value })} />
                    </div>
                </div>
                <div className="form-group">
                    <label htmlFor="disconnectOnUnknownPacketInput">Disconnect on unknown packet</label>
                    <div className="input-group" id="disconnectOnUnknownPacketInput">
                        <span className="input-group-addon" aria-label="Disconnect on unknown packet">
                            <input type="checkbox" checked={this.state.disconnectOnUnknownPacket} onChange={(e: any) => this.setState({ disconnectOnUnknownPacket: e.target.checked })}/>
                        </span>
                        <input type="text" className="form-control" disabled={true} />
                    </div>
                </div>
                <div className="form-group">
                    <label htmlFor="maximumReceiveSizeInput">Maximum packet size</label>
                    <input id="maximumReceiveSizeInput" className="form-control" type="number" min="3" max="255" value={this.state.maximumReceiveSize} onChange={(e: any) => this.setState({ maximumReceiveSize: e.target.value })} />
                </div>
                <div className="form-group">
                    <label htmlFor="maxConnectionsPerAddressInput">Maximum connections per address</label>
                    <div className="input-group" id="maxConnectionsPerAddressInput">
                        <span className="input-group-addon" aria-label="Maximum connections per address">
                            <input id="checkMaxConnectionsPerAddressInput" type="checkbox" checked={this.state.checkMaxConnectionsPerAddress} onChange={(e: any) => this.setState({ checkMaxConnectionsPerAddress: e.target.checked })} />
                        </span>
                        <input className="form-control" type="number" min="1" value={this.state.maxConnectionsPerAddress} onChange={(e: any) => this.setState({ maxConnectionsPerAddress: e.target.value })} />
                    </div>
                </div>
                <div className="form-group">
                    <label htmlFor="maxConnectionsInput">Maximum connections</label>
                    <input id="maxConnectionsInput" className="form-control" type="number" min="1" value={this.state.maxConnections} onChange={(e: any) => this.setState({maxConnections: e.target.value})}/>
                </div>
                <div className="form-group">
                    <label htmlFor="listenerBacklogInput">Listener Backlog</label>
                    <input id="listenerBacklogInput" className="form-control" type="number" min="1" value={this.state.listenerBacklog} onChange={(e: any) => this .setState({listenerBacklog: e.target.value})}/>
                </div>
                <div className="form-group">
                    <label htmlFor="maxFtpRequestsInput">Maximum ftp requests per session</label>
                    <input id="maxFtpRequestsInput" className="form-control" type="number" min="0" value={this.state.maxFtpRequests} onChange={(e: any) => this.setState({maxFtpRequests: e.target.value})}/>
                </div>
                <div className="form-group">
                    <label htmlFor="maxIpRequestsInput">Maximum address requests per session</label>
                    <input id="maxIpRequestsInput" className="form-control" type="number" min="0" value={this.state.maxIpRequests} onChange={(e: any) => this.setState({maxIpRequests: e.target.value})}/>
                </div>
                <div className="form-group">
                    <label htmlFor="maxServerListRequestsInput">Maximum server list requests per session</label>
                    <input id="maxServerListRequestsInput" className="form-control" type="number" min="0" value={this.state.maxServerListRequests} onChange={(e: any) => this.setState({maxServerListRequests: e.target.value})}/>
                </div>
                <div className="form-group">
                    <label htmlFor="timeoutSecondsInput">Connection timeout (seconds)</label>
                    <input id="timeoutSecondsInput" className="form-control" type="number" min="1" value={ConnectServerSettings.timeoutSeconds(this.state)} onChange={(e: any) => this.setState({ timeout: ConnectServerSettings.toTimeSpanString(e.target.value) })}/>
                </div>

                    <button type="submit" className="btn btn-xs btn-success btn-primary">Save</button>
                    {this.props.onCancel ? <button type="button" className="btn btn-xs" onClick={() => this.props.onCancel()}>Cancel</button> : null }

            </form>
        </div>);
    }

    private copyArrayAndChangeIndex(source: string, index: number, newValue: number) : string {
        let rawSource = atob(source);
        let rawResult = rawSource.substr(0, index) + String.fromCharCode(newValue) + rawSource.substr(index + 1);
        return btoa(rawResult);
    }

    private submit(event: any) {
        this.props.save(this.state, () => this.onSaveSuccess(), (error) => this.onSaveError(error));
        event.preventDefault();
    }

    private onSaveSuccess() {
        alert('The changes have been saved.');
        this.props.onSaveSuccess();
    }

    private onSaveError(error: any) {
        alert('Something went wrong. Error message: ' + error);
    }
}

const mapDispatchToProps = (dispatch: any) => {
    return {
        save: (settings: ConnectServerSettings, successCallback: () => void, errorCallback: (error: any) => void) => {
            dispatch(saveConnectServerConfiguration(settings, successCallback, errorCallback));
        }
    };
};


const mapStateToProps = (state: ApplicationState) => {return { clients: state.serverListState.clients }};


export default connect(mapStateToProps, mapDispatchToProps)(ConnectServerConfiguration);