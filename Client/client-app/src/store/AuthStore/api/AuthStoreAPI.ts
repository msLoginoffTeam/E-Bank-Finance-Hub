import {USER_API} from "../../../api/endpoints.ts";
import axiosInstance from "../../../api/axiosInstance.ts";
import {RegisterData} from "../AuthStore.types.ts";

export const AuthStoreAPI = {
    login: (credentials: { email: string; password: string }) =>
        axiosInstance.post(`${USER_API}/users/login`, credentials),

    register: (data: RegisterData) =>
        axiosInstance.post(`${USER_API}/users/create`, data),

    refreshToken: () =>
        axiosInstance.post(`${USER_API}/users/refresh`),
};