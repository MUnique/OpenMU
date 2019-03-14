import React from "react";
import { connect } from "react-redux";
import { PlugInConfiguration } from "../stores/plugins/types";
import { savePlugInConfiguration } from "../stores/plugins/actions";

interface IPlugInItemProps {
    plugin: PlugInConfiguration;
    save: (plugin: PlugInConfiguration) => Promise<void>;
}

class PlugInItem extends React.Component<IPlugInItemProps, {}> {

    public render() {
        return (
            <tr>
                <td title={this.props.plugin.plugInPointDescription}>{this.props.plugin.plugInPointName}</td>
                <td title={this.props.plugin.plugInDescription}>{this.props.plugin.plugInName}</td>
                <td title={this.props.plugin.typeId}>{this.props.plugin.typeName}</td>
                <td>
                    <button type="button" className={this.getActionClass()} onClick={() => this.changeIsActive()}>{this.getActionCaption()}</button>
                </td>
            </tr>
        );
    }

    getActionCaption() {
        if (this.props.plugin.isActive)
            return "Deactivate";
        else
            return "Activate";
    }

    getActionClass(): string {
        if (!this.props.plugin.isActive)
            return 'btn btn-xs btn-success';
        else
            return 'btn btn-xs btn-warning';
    }
    private changeIsActive() {
        // make a copy of the instance, so we're not editing the props or state of the application yet
        let plugin = { ...this.props.plugin, isActive : !this.props.plugin.isActive };
        this.props.save(plugin);
    }
}


const mapDispatchToProps = (dispatch: any) => {
    return {
        save: (plugin: PlugInConfiguration) => dispatch(savePlugInConfiguration(plugin))
    };
}

export default connect(null, mapDispatchToProps)(PlugInItem);