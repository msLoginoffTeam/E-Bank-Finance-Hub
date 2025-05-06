import axios from 'axios';
import {AUTH_TOKEN_API} from './endpoints';
import axiosRetry from "axios-retry";

interface Event { success: boolean; ts: number }
const events: Event[] = []
let circuitOpen = false
let circuitOpenedAt = 0

const FAILURE_THRESHOLD = 0.7
const MIN_REQUESTS      = 1
const COOL_DOWN_PERIOD  = 30_000

function addEvent(success: boolean) {
    const now = Date.now()
    events.push({ success, ts: now })
    if (events.length > 20) events.shift()

    const total    = events.length
    const failures = events.filter(e => !e.success).length
    if (!circuitOpen && total >= MIN_REQUESTS && failures / total > FAILURE_THRESHOLD) {
        circuitOpen      = true
        circuitOpenedAt  = now
        console.warn('[CB] circuit opened — слишком много ошибок')
    }
}

function checkCircuit() {
    if (!circuitOpen) return
    const now = Date.now()
    // если прошло время остывания — закрываем цепь и сбрасываем метрики
    if (now - circuitOpenedAt > COOL_DOWN_PERIOD) {
        circuitOpen = false
        events.length = 0
        console.info('[CB] circuit closed — готов к новым запросам')
    } else {
        throw new axios.AxiosError(
            'Circuit is open — отказано в вызове',
            'ERR_CIRCUIT_OPEN'
        )
    }
}

const axiosInstance = axios.create({
    baseURL: '/',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

axiosRetry(axiosInstance, {
    retries: 10,
    //retryDelay: axiosRetry.exponentialDelay,
    retryCondition: (error) => {
        if (axiosRetry.isNetworkError(error)) return true

        const status = error.response?.status
        return !!status && status >= 500
    },
})


axiosInstance.interceptors.request.use((config) => {
    checkCircuit();

    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    if (
        (config.method?.toLowerCase() === 'post' || config.method?.toLowerCase() === 'delete') &&
        !config.headers['Idempotency-Key'] &&
        !config.url?.includes('/Refresh')
    ) {
        config.headers['Idempotency-Key'] = crypto.randomUUID();
    }

    const method = config.method?.toUpperCase();
    const url = config.url;
    console.log(`[${method}] ${url}`);
    return config;
});

axiosInstance.interceptors.response.use(
    (response) => {
        addEvent(true)
        return response
    },
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

        addEvent(false);
        return Promise.reject(error);
    }
);


export default axiosInstance;
