import React from "react";
import { connect } from "react-redux";

import { ApplicationState } from "../stores/index";

import {
    logFilterByCharacter,
    logFilterByLogger,
    logFilterByServer,
    logSetAutoRefresh
} from "../stores/log/actions";

interface LogFilterProps {
    loggers: string[];
    autoRefresh: boolean;
    loggerFilter: string;
    characterFilter: string;
    serverFilter: string;

    filterByServer: (serverId: string) => void;
    filterByCharacter: (characterName: string) => void;
    filterByLogger: (loggerName: string) => void;
    setAutoRefresh: (value: boolean) => void;
}

class LogFilter extends React.Component<LogFilterProps, {}> {

    private filterByServer(event: InputFormEvent) {

        let serverId = event.target.value;
        this.props.filterByServer(serverId);
    }

    private filterByCharacter(event: InputFormEvent) {
        let characterName = event.target.value;
        this.props.filterByCharacter(characterName);
    }

    private filterByLogger(event: InputFormEvent) {
        let loggerName = event.target.value;
        this.props.filterByLogger(loggerName);
    }

    render() {
        let loggerList = this.props.loggers.map((logger, index) => <option value={logger} key={index} />);

        return (
            <div>Filters: 
                <datalist id="loggers">
                    {loggerList}
                </datalist>
                <input id="server" type="text" placeholder="Server" value={this.props.serverFilter || ''} onChange={(event: InputFormEvent) => this.filterByServer(event)} />
                <input id="character" type="text" placeholder="Character" value={this.props.characterFilter || ''} onChange={(event: InputFormEvent) => this.filterByCharacter(event)} />
                <input id="logger" type="text" list="loggers" placeholder="Logger" value={this.props.loggerFilter || ''} width="200" onChange={(event: InputFormEvent) => this.filterByLogger(event)} />
                <div><input type="checkbox" onChange={() => this.props.setAutoRefresh(!this.props.autoRefresh)} checked={this.props.autoRefresh} /> Auto Refresh</div>
            </div>
        );
    }
}


const mapStateToProps = (state: ApplicationState) => {
    return {
        loggers: state.logTableState.loggers,
        characterFilter: state.logTableState.characterFilter,
        loggerFilter: state.logTableState.loggerFilter,
        serverFilter: state.logTableState.serverFilter,
        autoRefresh: state.logTableState.autoRefresh,
    };
};
const mapDispatchToProps = (dispatch: any) => {
    return {
        filterByServer: (serverId: string) => dispatch(logFilterByServer(serverId)),
        filterByCharacter: (characterName: string) => dispatch(logFilterByCharacter(characterName)),
        filterByLogger: (loggerName: string) => dispatch(logFilterByLogger(loggerName)),
        setAutoRefresh: (value: boolean) => dispatch(logSetAutoRefresh(value)),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(LogFilter);