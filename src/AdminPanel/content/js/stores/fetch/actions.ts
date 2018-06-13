import Redux, { MiddlewareAPI, Dispatch, Middleware } from "redux";
import Action = Redux.Action;
import ActionCreator = Redux.ActionCreator;

export enum Constants {
    FETCH_ACTIVE = "FETCH_ACTIVE",
    FETCH_INACTIVE = "FETCH_INACTIVE",
};

export interface FetchActive extends Action {
    type: Constants.FETCH_ACTIVE
}

export interface FetchInactive extends Action {
    type: Constants.FETCH_INACTIVE
}

const fetchActive: ActionCreator<FetchActive> = () => ({
    type: Constants.FETCH_ACTIVE
});

const fetchInactive: ActionCreator<FetchInactive> = () => ({
    type: Constants.FETCH_INACTIVE
});

/*
 * This middleware automatically dispatches fetchActive() when an async action (promise) is about to get started.
 * fetchInactive() is then dispatched when a non-function is dispatched. This seems to be very hacky, but
 * it works, since usually the result of the async action is forwarded by dispatching new actions.
 * Of course, this approach has some limitations, e.g. when multiple async actions are started - the result of the first one will dispatch fetchInactive().
 */
export const fetchMiddleware: Middleware =
    (api: MiddlewareAPI) =>
        (next: Dispatch) =>
        <A extends Action>(action: A) => {
            if (typeof action === 'function') {
                next(fetchActive());
                return next(action);
            } else {
                next(fetchInactive());
                return next(action);
            }
        };