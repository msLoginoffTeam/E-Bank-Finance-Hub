import axiosInstance from '../api/axiosInstance';
import { ACCOUNTS_API } from '../api/endpoints';

export const AccountsStoreAPI = {
    getAccounts: async () => {
        const { data } = await axiosInstance.get(`${ACCOUNTS_API}/accounts/all`);
        return data;
    },

    openAccount: async () => {
        const { data } = await axiosInstance.post(`${ACCOUNTS_API}/accounts/open`);
        return data;
    },

    closeAccount: async (accountId: string) => {
        const { data } = await axiosInstance.delete(`${ACCOUNTS_API}/accounts/close`, {
            data: { accountId },
        });
        return data;
    },
};