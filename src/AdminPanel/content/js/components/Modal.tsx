import React from "react";
import { connect } from "react-redux";

import { ApplicationState } from "../stores/index";

interface IModalProps {
    modalContent: any;
}

class Modal extends React.Component<IModalProps, { }> {
    public render() {
        if (this.props.modalContent) {
            return (<div className="modal-backdrop">
                        <div className="modal-window">
                            {this.props.modalContent}
                        </div>
                    </div>);
        }

        return null;
    }
}

const mapStateToProps = (state: ApplicationState) => {
    return {
        modalContent: state.modalState.modalContent,
    };
};

export default connect(mapStateToProps)(Modal);