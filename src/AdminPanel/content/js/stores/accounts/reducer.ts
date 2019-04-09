import Redux from "redux";
import { Account } from "./types";
import {
    Constants,
    FetchAccountsSuccessAction,
    FetchAccountsErrorAction,
    AccountSaveSuccessAction,
    AccountSaveErrorAction,
    AccountDeleteSuccessAction,
    AccountDeleteErrorAction
} from "./actions";


export type AccountListState = {
    readonly page: number,
    readonly pageSize: number,
    readonly hasMoreEntries: boolean,
    readonly accounts: Account[],
    readonly createDialogVisible: boolean,
};

const initialState: AccountListState =
{
    page: 1,
    pageSize: 20,
    hasMoreEntries: false,
    accounts: [],
    createDialogVisible: false,
};


// This reducer handles the account actions and returns the new account list state.
export const accountStateReducer: Redux.Reducer<AccountListState> =
    (state: AccountListState = initialState, action: Redux.Action) => {
        switch ((action as Redux.Action).type) {
            case Constants.ACCOUNTS_FETCH_OK:
                let fetchSuccessAction = action as FetchAccountsSuccessAction;
                if (fetchSuccessAction.response.length === 0) {
                    return { ...state, hasMoreEntries: false };
                }

                return { ...state, pageSize: fetchSuccessAction.pageSize, page: fetchSuccessAction.page, accounts: fetchSuccessAction.response, hasMoreEntries: fetchSuccessAction.response.length === fetchSuccessAction.pageSize };
            case Constants.ACCOUNTS_FETCH_ERROR:
                let fetchErrorAction = action as FetchAccountsErrorAction;
                console.log('An error occurred when fetching accounts.', fetchErrorAction.error);
                return state;

            case Constants.ACCOUNT_SAVE_OK:
                console.log('ACCOUNT_SAVE_OK');
                let accountSaveAction = action as AccountSaveSuccessAction;
                let savedAccount = accountSaveAction.account;
                let index = state.accounts.findIndex((account : Account) => { return account.id === savedAccount.id });

                let newAccounts = state.accounts.slice(0, state.accounts.length);
                if (index >= 0) {
                    newAccounts[index] = savedAccount;
                } else {
                    newAccounts.push(savedAccount);
                }
                return { ...state, accounts: newAccounts, createDialogVisible: false };
            case Constants.ACCOUNT_SAVE_ERROR:
                let accountSaveErrorAction = action as AccountSaveErrorAction;
                console.log('An error occurred when saving an account.', accountSaveErrorAction.error);

                return state;
            case Constants.ACCOUNT_DELETE_OK:
                let accountDeleteAction = action as AccountDeleteSuccessAction;
                    return { ...state, accounts: state.accounts.filter(account => account !== accountDeleteAction.account) };
            case Constants.ACCOUNT_DELETE_ERROR:
                console.log('An error occurred when deleting an account.', (action as AccountDeleteErrorAction).error);
            case Constants.ACCOUNT_SHOW_CREATE:
                return { ...state, createDialogVisible: true };
            case Constants.ACCOUNT_HIDE_CREATE:
                console.log('ACCOUNT_HIDE_CREATE');
                return { ...state, createDialogVisible: false };
            default:
                return state;
        }
    };