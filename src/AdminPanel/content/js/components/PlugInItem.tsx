import React from "react";
import { connect } from "react-redux";
import { PlugInConfiguration } from "../stores/plugins/types";
import { savePlugInConfiguration } from "../stores/plugins/actions";

interface IPlugInItemProps {
    plugin: PlugInConfiguration;
    markedName: string,
    markedType: string,
    save: (plugin: PlugInConfiguration) => Promise<void>;
}

class PlugInItem extends React.Component<IPlugInItemProps, {}> {

    public render() {
        return (
            <tr>
                <td title={this.props.plugin.plugInPointDescription}>{this.props.plugin.plugInPointName}</td>
                <td title={this.props.plugin.plugInDescription}>{this.getMarkedPlugInName()}</td>
                <td title={this.props.plugin.typeId}>{this.getMarkedPlugInTypeName()}</td>
                <td>
                    <button type="button" className={this.getActionClass()} onClick={() => this.changeIsActive()}>{this.getActionCaption()}</button>
                </td>
            </tr>
        );
    }

    getMarkedPlugInName() {
        return this.getMarked(this.props.plugin.plugInName, this.props.markedName);
    }

    getMarkedPlugInTypeName() {
        return this.getMarked(this.props.plugin.typeName, this.props.markedType);
    }

    getMarked(text: string, mark: string) {
        if (mark === null || mark.length === 0) {
            return (<span key="0">{text}</span>);
        }

        let elements: any[] = [];
        let counter = 0;
        let currentIndex = 0;
        while (currentIndex < text.length) {
            let nextMarkStart = text.toLowerCase().indexOf(mark.toLowerCase(), currentIndex);
            if (nextMarkStart >= 0) {
                let end = nextMarkStart + mark.length;
                elements.push(<span key={counter++}>{text.substring(currentIndex, nextMarkStart)}</span>);
                elements.push(<mark key={counter++} className="plugin">{text.substring(nextMarkStart, end)}</mark>);
                currentIndex = end;
            } else {
                let rest = text.substring(currentIndex);
                elements.push(<span key={counter++}>{rest}</span>);
                currentIndex += rest.length;
            }
        }

        return elements;
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