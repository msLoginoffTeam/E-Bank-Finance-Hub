import  axiosInstance  from "../api/axiosInstance.ts";
import {PREFERENCES_API} from "../api/endpoints.ts";

export const ThemeAPI = {
    getTheme:    async (userId: string) => {
        const { data } = await axiosInstance.get(`${PREFERENCES_API}/theme/getTheme?userId=${userId}`)
        return data;
    },
    createTheme: (userId: string, theme: "Light"|"Dark") =>
        axiosInstance.post(`${PREFERENCES_API}/theme/create?userId=${userId}&themeEnum=${theme}`),

    setTheme:    (userId: string, theme: "Light"|"Dark") =>
        axiosInstance.put(`${PREFERENCES_API}/theme/set?userId=${userId}&themeEnum=${theme}`),
};
