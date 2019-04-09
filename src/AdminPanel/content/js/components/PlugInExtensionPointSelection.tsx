import React from "react";
import { connect } from "react-redux";
import { ApplicationState } from "../stores/index";
import { PlugInExtensionPoint } from "../stores/plugins/types";
import { fetchPlugInConfigurations, fetchPlugInPoints } from "../stores/plugins/actions";

interface IPlugInExtensionPointSelection {
    selectedPoint: any;
    extensionPoints: PlugInExtensionPoint[];
    filterName: string;
    filterType: string;
    changeSelection(selectedExtensionPointId: any, filterName: string, filterType: string): Promise<void>;
    loadPoints(): Promise<void>;
}

class PlugInExtensionPointSelection extends React.Component<IPlugInExtensionPointSelection, {}> {
    public componentDidMount() {
        this.props.loadPoints();
    }

    public render() {
        let extensionPoints = this.props.extensionPoints.map(
            point => {
                if (this.props.selectedPoint === point.id) {
                    return (<option value={point.id} key={point.id} selected>{point.name} ({point.plugInCount})</option>);
                } else {
                    return (<option value={point.id} key={point.id}>{point.name} ({point.plugInCount})</option>);
                }
            });

        return (
            <div>
                <select className="btn extensions" name="extensionPoints" onChange={(e: React.ChangeEvent<HTMLSelectElement>) => this.props.changeSelection(e.target.value, this.props.filterName, this.props.filterType)}>
                    <option value="00000000-0000-0000-0000-000000000000">All</option>
                    {extensionPoints}
                </select>
            </div>
        );
    }
}


const mapDispatchToProps = (dispatch: any) => {
    return {
        changeSelection: (pointId: any, filterName: string, filterType: string) => dispatch(fetchPlugInConfigurations(pointId, filterName, filterType, 1, 20)),
        loadPoints: () => dispatch(fetchPlugInPoints()),
    };
}

const mapStateToProps = (state: ApplicationState) => {
    return {
        selectedPoint: state.plugInListState.selectedExtensionPointId,
        extensionPoints: state.plugInListState.extensionPoints,
        filterName: state.plugInListState.filterName,
        filterType: state.plugInListState.filterType,
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(PlugInExtensionPointSelection);