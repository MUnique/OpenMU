import React from "react";
import { connect } from "react-redux";

import { ApplicationState } from "../stores/index";
import { PlugInConfiguration } from "../stores/plugins/types";
import { fetchPlugInConfigurations, showCreateDialog } from "../stores/plugins/actions";

import PlugInItem from "./PlugInItem";

interface IPlugInListProps {
    plugins: PlugInConfiguration[];
    page: number;
    pageSize: number;
    createDialogVisible: boolean;
    showCreateDialog: () => void;
    fetchPage(newPage: number, entriesPerPage: number): Promise<void>;
}

class PlugInList extends React.Component<IPlugInListProps, {}> {
    public componentDidMount() {
        this.props.fetchPage(this.props.page, this.props.pageSize);
    }

    public render() {
        let pluginList = this.props.plugins.map(
            plugin => <PlugInItem plugin={plugin} key={plugin.id} />);
        let createDialog = this.props.createDialogVisible
            ? <span>Creating is not supported yet.</span>
            : <span></span>;
        return (
            <div>
                <table className="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th className="col-xs-3">Extension Point</th>
                            <th className="col-xs-3">Plugin Name</th>
                            <th className="col-xs-3">Plugin Type</th>
                            <th className="col-xs-3">Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {pluginList}
                    </tbody>
                    <tfoot>
                        <tr>
                            <td><button type="button" className="btn btn-xs btn-success" onClick={this.props.showCreateDialog}>Create</button></td>
                            <td colSpan={3}><button type="button" className="btn btn-xs" onClick={() => this.props.fetchPage(this.props.page - 1, this.props.pageSize)}>&lt;</button> Page {this.props.page} <button type="button" className="btn btn-xs" onClick={() => this.props.fetchPage(this.props.page + 1, this.props.pageSize)}>&gt;</button></td>
                            <td></td>
                        </tr>
                    </tfoot>
                </table>
                {createDialog}
            </div>
        );
    }
}


const mapDispatchToProps = (dispatch: any) => {
    return {
        fetchPage: (newPage: number, entriesPerPage: number) => dispatch(fetchPlugInConfigurations(newPage, entriesPerPage)),
        showCreateDialog: () => dispatch(showCreateDialog())
    };
}

const mapStateToProps = (state: ApplicationState) => {
    return {
        plugins: state.plugInListState.plugins,
        page: state.plugInListState.page,
        pageSize: state.plugInListState.pageSize,
        createDialogVisible: state.plugInListState.createDialogVisible,
    };
};


export default connect(mapStateToProps, mapDispatchToProps)(PlugInList);