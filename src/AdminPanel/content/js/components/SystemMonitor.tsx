import React, {CSSProperties} from "react";
import { connect } from "react-redux";
import { ApplicationState } from "../stores/index";
import { systemSubscribe, systemUnsubscribe } from "../stores/system/actions";
import { SystemStateSnapshot } from "../stores/system/reducer";


interface SystemMonitorProps {
    data: SystemStateSnapshot[],
    subscribe: (subscriber: any) => void;
    unsubscribe: (subscriber: any) => void;
}

class SystemMonitor extends React.Component<SystemMonitorProps, {}>
{
    componentDidMount(): void {
        this.props.subscribe(this);
    }

    componentWillUnmount(): void {
        this.props.unsubscribe(this);
    }

    render() {
        // TODO: Line Chart maybe with the victory package?
        if (this.props.data.length === 0) {
            return (<span></span>);
        }
        let currentData = this.props.data[this.props.data.length - 1];
        
        // quick&dirty... don't try this at home ;)
        let backCssProps: CSSProperties = {
            width: '300px',
            height: '20px',
            backgroundColor: 'grey',
            position: 'absolute',
            top: 0,
            left: 0,
        }

        let totalCssProps: CSSProperties = {
            width: (currentData.cpuPercentTotal * 300 / 100).toFixed(0) + 'px',
            height: '20px',
            backgroundColor: 'green',
            position: 'absolute',
            top: 0,
            left: 0,
        }

        let instanceCssProps: CSSProperties = {
            width: (currentData.cpuPercentInstance * 300 / 100).toFixed(0) + 'px',
            height: '20px',
            backgroundColor: 'red',
            position: 'absolute',
            top: 0,
            left: 0,
        }

        return (
            <div>
                CPU Usage:
                <div style={{ position: 'relative', height: '30px' }}><span style={backCssProps}/><span style={totalCssProps}/><span style={instanceCssProps}/></div>
                <small>Green: CPU % of the total system; Red: CPU % of OpenMU</small>
            </div>);
    }
}


const mapStateToProps = (state: ApplicationState) => {
    return { data: state.systemState.snapshots };
}

const mapDispatchToProps = (dispatch: any) => {
    return {
        subscribe: (subscriber: any) => dispatch(systemSubscribe(subscriber)),
        unsubscribe: (subscriber: any) => dispatch(systemUnsubscribe(subscriber)),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(SystemMonitor);