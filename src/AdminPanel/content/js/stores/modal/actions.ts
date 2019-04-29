import Redux from "redux";

export enum Constants {
    MODAL_HIDE = "MODAL_HIDE",
    MODAL_SHOW = "MODAL_SHOW",
}


export interface ShowModalAction extends Redux.Action {
    type: Constants.MODAL_SHOW,
    modalContent: any,
}

export interface HideModalAction extends Redux.Action {
    type: Constants.MODAL_HIDE,
}

export const showModal: Redux.ActionCreator<ShowModalAction> =
    (modalContent: any) => ({ type: Constants.MODAL_SHOW, modalContent });

export const hideModal: Redux.ActionCreator<HideModalAction> =
    () => ({ type: Constants.MODAL_HIDE });
