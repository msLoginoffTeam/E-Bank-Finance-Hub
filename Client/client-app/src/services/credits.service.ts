import axiosInstance from "../api/axiosInstance.ts";

export const CreditsAPI = {
    getActiveCredits: async () => {
        const { data } = await axiosInstance.get(`/credits`, {
            params: { active: true }, // API пока не поддерживает limit
        });
        return data;
    },
};
