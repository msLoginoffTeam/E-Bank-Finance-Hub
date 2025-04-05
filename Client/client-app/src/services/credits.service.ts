import { CreditDetails } from '../types/creditTypes';
import {CREDITS_API} from "../api/endpoints.ts";
import axiosInstance from "../api/axiosInstance.ts";

export const CreditAPI = {
    async getCredits(): Promise<CreditDetails[]> {
        const response = await axiosInstance.get(`${CREDITS_API}/Credit/GetCreditsList/Client?ElementsNumber=20&PageNumber=1`);
        return response.data.creditsList as CreditDetails[];
    },

    async getCreditDetails(creditId: string): Promise<CreditDetails> {
        const response = await axiosInstance.get(`${CREDITS_API}/Credit/GetCreditHistory/Client?CreditId=${creditId}`);
        return response.data;
    },

    async getCreditPlans() {
        const response = await axiosInstance.get(`${CREDITS_API}/Credit/GetCreditPlans?ElementsNumber=30&PageNumber=1`);
        return response.data.planList;
    },

    async createCredit(data: { accountId: string; creditPlanId: string; amount: number; closingDate: string }) {
        const response = await axiosInstance.post(`${CREDITS_API}/Credit/GetCredit`, data);
        return response.data;
    },

    async payOffCredit(data: { accountId: string; creditId: string; amount: number }) {
        const response = await axiosInstance.post(`${CREDITS_API}/Credit/PayOffTheLoan`, data);
        return response.data;
    },
};
