import Redux from "redux";
import {Constants} from "./actions";

export type FetchState = {
    readonly isFetching: boolean,
};

export const initialState: FetchState = {
    isFetching: false
};

export const fetchStateReducer: Redux.Reducer<FetchState> =
    (state: FetchState = initialState, action: Redux.Action) => {
        switch ((action as Redux.Action).type) {
        case Constants.FETCH_ACTIVE:
            return { isFetching: true };
        case Constants.FETCH_INACTIVE:
            return { isFetching: false };
        default:
            return state;
        }
    }