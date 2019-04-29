import Redux from "redux";
import Action = Redux.Action;
import ActionCreator = Redux.ActionCreator;
import { Account } from "./types";
import { hideModal } from "content/js/stores/modal/actions";

export enum Constants {
    ACCOUNTS_FETCH_OK = "ACCOUNTS_FETCH_OK",
    ACCOUNTS_FETCH_ERROR = "ACCOUNTS_FETCH_ERROR",
    ACCOUNT_SAVE_OK = "ACCOUNT_SAVE_OK",
    ACCOUNT_SAVE_ERROR = "ACCOUNT_SAVE_ERROR",
    ACCOUNT_DELETE_OK = "ACCOUNT_DELETE_OK",
    ACCOUNT_DELETE_ERROR = "ACCOUNT_DELETE_ERROR",
};

export interface FetchAccountsSuccessAction extends Action {
    type: Constants.ACCOUNTS_FETCH_OK,
    response?: Account[],
    page: number;
    pageSize: number;
}

export interface FetchAccountsErrorAction extends Action {
    type: Constants.ACCOUNTS_FETCH_ERROR,
    error: string,
}

interface AccountSuccessAction extends Action {
    type: any;
    account: Account;
}

interface AccountErrorAction extends Action {
    type: any;
    account: Account;
    error: string;
}


export interface AccountSaveSuccessAction extends AccountSuccessAction {
    type: Constants.ACCOUNT_SAVE_OK
}

export interface AccountSaveErrorAction extends AccountErrorAction {
    type: Constants.ACCOUNT_SAVE_ERROR
}

export interface AccountDeleteSuccessAction extends AccountSuccessAction {
    type: Constants.ACCOUNT_DELETE_OK
}
export interface AccountDeleteErrorAction extends AccountErrorAction {
    type: Constants.ACCOUNT_DELETE_ERROR
}


export const saveAccountSuccess: ActionCreator<AccountSaveSuccessAction> = (account: Account) => ({
    type: Constants.ACCOUNT_SAVE_OK,
    account,
});

export const saveAccountError: ActionCreator<AccountSaveErrorAction> = (account: Account, error: string) => ({
    type: Constants.ACCOUNT_SAVE_ERROR,
    account,
    error
});

export const deleteAccountSuccess: ActionCreator<AccountDeleteSuccessAction> = (account: Account) => ({
    type: Constants.ACCOUNT_DELETE_OK,
    account,
});

export const deleteAccountError: ActionCreator<AccountDeleteErrorAction> = (account: Account, error: string) => ({
    type: Constants.ACCOUNT_DELETE_ERROR,
    account,
    error
});

export const fetchedAccountsSuccessfully: ActionCreator<FetchAccountsSuccessAction> = (response: Account[], page: number, pageSize: number) => ({
    type: Constants.ACCOUNTS_FETCH_OK,
    response,
    page,
    pageSize,
});

export const fetchAccountsFailed: ActionCreator<FetchAccountsErrorAction> = (error: string) => ({
    type: Constants.ACCOUNTS_FETCH_ERROR,
    error,
});


export function fetchAccounts(page: number, entriesPerPage: number) {
    return (dispatch: Redux.Dispatch) => {
        let offset = (page - 1) * entriesPerPage;
        return fetch("/admin/account/" + offset + "/" + entriesPerPage)
            .then(
                response => response.json(),
                error => {
                    dispatch(fetchAccountsFailed(error));
                })
            .then(response => {
                dispatch(fetchedAccountsSuccessfully(response, page, entriesPerPage));
            });
    }
}

export function deleteAccount(account: Account) {
    return (dispatch: Redux.Dispatch) => {
        return fetch("/admin/account/" + account.id,
                {
                    cache: 'no-cache',
                    method: 'DELETE',
                })
            .then(() => {
                dispatch(deleteAccountSuccess(account));
            })
            .catch(error => {
                dispatch(deleteAccountError(account, error.toString()));
            });
    }
}

export function saveAccount(account: Account) {
    return (dispatch: Redux.Dispatch) => {

        return fetch("/admin/account",
                {
                    body: JSON.stringify(account),
                    cache: 'no-cache',
                    headers: {
                        'content-type': 'application/json'
                    },
                    method: 'POST',
                })
            .then(response => response.text())
            .then((response) => {
                if (account.id === null) {
                    account = {...account, id: response};
                }

                dispatch(saveAccountSuccess(account));
                dispatch(hideModal());
            })
            .catch(error => {
                dispatch(saveAccountError(account, error.toString()));
            });
    }
}