import axiosInstance from '../api/axiosInstance';
import { ACCOUNTS_API } from '../api/endpoints';

export const AccountsStoreAPI = {
    getAccounts: async () => {
        const { data } = await axiosInstance.get(`${ACCOUNTS_API}/accounts/all`);
        return data;
    },

    openAccount: async (accountName: string) => {
        const { data } = await axiosInstance.post(`${ACCOUNTS_API}/accounts/open?Name=${accountName}`, accountName);
        return data;
    },

    closeAccount: async (accountId: string) => {
        const { data } = await axiosInstance.delete(`${ACCOUNTS_API}/accounts/close?AccountId=${accountId}`);
        return data;
    },
};

export const AccountsAPI = {
    deposit: (accountId: string, amount: number, type: 'Income' | 'Outcome') =>
        axiosInstance.post(`${ACCOUNTS_API}/account/${accountId}/cash`, { amountInRubles: amount, operationType: type }),

    withdraw: (accountId: string, amount: number) =>
        axiosInstance.post(`${ACCOUNTS_API}/account/${accountId}/cash`, { amount: -amount }),
};
