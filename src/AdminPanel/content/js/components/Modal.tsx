import React from "react";
import { connect } from "react-redux";

import { ApplicationState } from "../stores/index";
import { hideModal } from "content/js/stores/modal/actions";

interface IModalProps {
    modalContent: any;
    onClose: () => void;
}

class Modal extends React.Component<IModalProps, { }> {
    public render() {
        if (this.props.modalContent) {
            return (
                <div className="modal-backdrop">
                    <div className="modal-window panel panel-body panel-default">
                        <button className="btn btn-default btn-xs" id="modal-close-button" title="Close Dialog" onClick={() => this.props.onClose()}>
                            <span className="glyphicon glyphicon-remove" aria-hidden={true}/>
                        </button>
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

const mapDispatchToProps = (dispatch: any) => {
    return {
        onClose: () => dispatch(hideModal())
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(Modal);