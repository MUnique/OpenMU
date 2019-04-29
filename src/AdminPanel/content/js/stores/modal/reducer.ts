import Redux from "redux";
import { Constants, ShowModalAction } from "./actions";

export type ModalState = {
    modalContent: any,
};

export const initialState : ModalState = {
    modalContent: null,
}

export const modalStateReducer: Redux.Reducer<ModalState> =
(state: ModalState = initialState,
    action: Redux.Action) => {
    switch ((action as Redux.Action).type) {
        case Constants.MODAL_SHOW:
        {
            let showAction = action as ShowModalAction;

            return {
                modalContent: showAction.modalContent,
            };
        }

        case Constants.MODAL_HIDE:
        {
            return initialState;
        }
    }

    return state;
};
