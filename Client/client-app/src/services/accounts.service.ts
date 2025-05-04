import axiosInstance from '../api/axiosInstance';
import {ACCOUNTS_API, USER_API} from '../api/endpoints';

export const AccountsStoreAPI = {
    getAccounts: async () => {
        const { data } = await axiosInstance.get(`${ACCOUNTS_API}/accounts/all`);
        return data;
    },

    openAccount: async (accountName: string, currency: string) => {
        const { data } = await axiosInstance.post(`${ACCOUNTS_API}/accounts/open`, {name: accountName, currency: currency});
        return data;
    },

    closeAccount: async (accountId: string) => {
        const { data } = await axiosInstance.delete(`${ACCOUNTS_API}/accounts/close?AccountId=${accountId}`);
        return data;
    },
};

export const AccountsAPI = {
    deposit: (accountId: string, amount: number, type: 'Income' | 'Outcome') =>
        axiosInstance.post(`${ACCOUNTS_API}/account/${accountId}/cash`, { amount: amount, operationType: type }),

    withdraw: (accountId: string, amount: number) =>
        axiosInstance.post(`${ACCOUNTS_API}/account/${accountId}/cash`, { amount: -amount }),
};

export const UserAPI = {
    getUserId: async () => {
        const { data } = await axiosInstance.get(`${USER_API}/users/api/client/profile`);
        return data.id;
    }
}
