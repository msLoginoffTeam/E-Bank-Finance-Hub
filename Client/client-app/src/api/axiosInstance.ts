import axios from 'axios';
import {AUTH_TOKEN_API} from './endpoints';

const axiosInstance = axios.create({
    baseURL: '/',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

axiosInstance.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    const method = config.method?.toUpperCase();
    const url = config.url;
    console.log(`[${method}] ${url}`);
    return config;
});

axiosInstance.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (
            error.response?.status === 401 &&
            !originalRequest._retry &&
            !originalRequest.url.includes('/Refresh')
        ) {
            originalRequest._retry = true;

            try {
                const refreshToken = localStorage.getItem('refreshToken');
                if (!refreshToken) throw new Error('No refresh token');

                const refreshResponse = await axios.post(
                    `${AUTH_TOKEN_API}/Refresh?IsClient=true`,
                    {},
                    {
                        headers: {
                            Authorization: `Bearer ${refreshToken}`,
                        },
                    }
                );

                console.log(refreshResponse);

                const newAccessToken = refreshResponse.data.accessToken;
                const newRefreshToken = refreshResponse.data.refreshToken;

                if (!newRefreshToken) {
                    return;
                }

                originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

                localStorage.setItem('token', newAccessToken);
                localStorage.setItem('refreshToken', newRefreshToken);

                return axiosInstance(originalRequest);
            } catch (refreshError) {
                console.log(refreshError);
                localStorage.removeItem('token');
                localStorage.removeItem('refreshToken');
                localStorage.removeItem('userId');
                window.location.href = '/login';
                return Promise.reject(refreshError);
            }
        }

        return Promise.reject(error);
    }
);


export default axiosInstance;
