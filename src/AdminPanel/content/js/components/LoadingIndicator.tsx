import React from "react";
import {connect} from "react-redux";
import { ApplicationState } from "../stores/index";

class LoadingIndicator extends React.Component<{ isLoading: boolean }, {}>{

    public render() {
        let className = "spinner";

        if (this.props.isLoading) {
            className += " spinner-spin";
        }

        return (
            <div className={className}></div>
        );
    }
}


const mapStateToProps = (state: ApplicationState) => {
    return { isLoading: state.fetchState.isFetching };
};

export default connect(mapStateToProps)(LoadingIndicator);