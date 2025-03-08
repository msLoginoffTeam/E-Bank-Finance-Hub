import axiosInstance from "../api/axiosInstance.ts";
import {ACCOUNTS_API} from "../api/endpoints.ts";

export const OperationsAPI = {
    getOperations: async (accountId: string) => {
        const { data } = await axiosInstance.get(`${ACCOUNTS_API}/account/${accountId}/operations`);
        return data;
    },
};
