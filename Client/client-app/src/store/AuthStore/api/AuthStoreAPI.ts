import {AUTH_API} from "../../../api/endpoints.ts";
import axiosInstance from "../../../api/axiosInstance.ts";
import {RegisterData} from "../AuthStore.types.ts";

export const AuthStoreAPI = {
    login: (credentials: { email: string; password: string }) =>
        axiosInstance.post(`${AUTH_API}/users/login`, credentials),

    register: (data: RegisterData) =>
        axiosInstance.post(`${AUTH_API}/users/create`, data),

    refreshToken: () =>
        axiosInstance.post(`${AUTH_API}/users/refresh`),
};