import React from "react";
import { LogEventData } from "../stores/log/types";
import { LogTableState } from "../stores/log/reducer";
import { connect } from "react-redux";

import { ApplicationState } from "../stores/index";

import {
    logSubscribe,
    logUnsubscribe
    } from "../stores/log/actions";

interface LogNotifierProps {
    showWarning: boolean;
    showError: boolean;
    showFatal: boolean;
    subscribe: (subscriber: any) => void;
    unsubscribe: (subscriber: any) => void;
}

class LogNotifier extends React.Component<LogNotifierProps, {}> {
    componentDidMount() {
        this.props.subscribe(this);
    }

    componentWillUnmount() {
        this.props.unsubscribe(this);
    }


    render() {
        let warnSpan = this.props.showWarning ? (<span style={{ color: 'orange' }} className="glyphicon glyphicon-warning-sign"></span>) : (<span></span>);
        let errorSpan = this.props.showError ? (<span style={{ color: 'orangered' }} className="glyphicon glyphicon-remove-sign"></span>) : (<span></span>);
        let fatalSpan = this.props.showFatal ? (<span style={{ color: 'orangered' }} className="glyphicon glyphicon-fire"></span>) : (<span></span>);
        return (<div style={{ position: 'relative', top: '-18px', height: '0', textAlign: 'center' }}>{warnSpan}{errorSpan}{fatalSpan}</div>);
    }
}

const mapStateToProps = (state: ApplicationState) => {
    return {
        showWarning: anyEntry(state.logTableState, "WARN"),
        showError: anyEntry(state.logTableState, "ERROR"),
        showFatal: anyEntry(state.logTableState, "FATAL"),
    };
}

const mapDispatchToProps = (dispatch: any) => {
    return {
        subscribe: (subscriber: any) => dispatch(logSubscribe(subscriber)),
        unsubscribe: (subscriber: any) => dispatch(logUnsubscribe(subscriber)),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(LogNotifier);



const anyEntry = (state: LogTableState, logLevel: string): boolean => {
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

        if (logLevel.localeCompare(entry.level.name) === 0) {
            return true;
        }
    }

    return false;
}