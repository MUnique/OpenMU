import React from "react";
import { LogEventData } from "../stores/log/types";
import { LogTableState } from "../stores/log/reducer";
import { connect } from "react-redux";

import { ApplicationState } from "../stores/index";
import LogFilter from "./LogFilter";

import {
    logSubscribe,
    logUnsubscribe
    } from "../stores/log/actions";


interface LogTableProps {
    entries: LogEventData[];
    subscribe: (subscriber: any) => void;
    unsubscribe: (subscriber: any) => void;
}

class LogTable extends React.Component<LogTableProps, {}> {


    componentDidMount() {
        this.props.subscribe(this);

    }

    componentWillUnmount() {
        this.props.unsubscribe(this);
    }

    public render() {
        var entryList =
            this.props.entries.map(entry => <LogEntry entry={entry} key={entry.timeStamp + "_" + entry.threadName}/>);

        return (
            <div className="log">
                <LogFilter key="logFilter"/>
                <table className="log">
                    <thead>
                    <tr>
                        <th className="col-xs-2">Timestamp</th>
                        <th className="col-xs-2">Logger</th>
                        <th className="col-xs-4">Message</th>
                    </tr>
                    </thead>
                    <tbody>
                    {entryList}
                    </tbody>
                </table>
            </div>
        );
    }
}


class LogEntry extends React.Component<{ entry: LogEventData }, {}> {

    render() {
        let dateTime = new Date(Date.parse(this.props.entry.timeStamp));
        let dateStr = dateTime.toISOString();
        let className = "log " + this.props.entry.level.name;
        return (
            <tr className={className}>
                <td>{dateStr}</td>
                <td>{this.props.entry.loggerName}</td>
                <td title={this.props.entry.exceptionString}>{this.props.entry.message}</td>
            </tr>
        );
    }
}

const mapStateToProps = (state: ApplicationState) => {
    return {
        entries: determineFilteredEntries(state.logTableState)
    }
};
const mapDispatchToProps = (dispatch: any) => {
    return {
        subscribe: (subscriber: any) => dispatch(logSubscribe(subscriber)),
        unsubscribe: (subscriber: any) => dispatch(logUnsubscribe(subscriber)),
    };
}


const determineFilteredEntries = (state: LogTableState): LogEventData[] => {
    let filteredEntries: LogEventData[] = [];
    for (var i = state.entries.length - 1; i >= 0; i--) {
        var entry = state.entries[i];
        if (state.loggerFilter && state.loggerFilter !== entry.loggerName) {
            continue;
        }

        if (state.characterFilter && state.characterFilter !== entry.properties["character"]) {
            continue;
        }

        if (state.serverFilter && state.serverFilter !== entry.properties["gameserver"]) {
            continue;
        }

        filteredEntries[filteredEntries.length] = entry;
        if (filteredEntries.length === 20) {
            break;
        }
    }

    return filteredEntries.reverse();
}

export default connect(mapStateToProps, mapDispatchToProps)(LogTable);