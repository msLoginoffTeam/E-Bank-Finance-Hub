import { Dispatch } from 'redux';
import { AuthStoreAPI } from './api/AuthStoreAPI';
import {LOGIN_SUCCESS, LOGIN_ERROR, LOGOUT, CLEAR_ERROR} from './AuthStore.const';
import { Credentials, RegisterData } from './AuthStore.types';

export const login = (credentials: Credentials) => async (dispatch: Dispatch) => {
    try {
        const { data } = await AuthStoreAPI.login(credentials);
        const token = data.accessToken;
        const refreshToken = data.refreshToken;

        localStorage.setItem('token', token);
        localStorage.setItem('refreshToken', refreshToken);

        dispatch({ type: LOGIN_SUCCESS, payload: token });
    } catch (error: any) {
        dispatch({ type: LOGIN_ERROR, payload: error.response?.data?.message || 'Ошибка входа' });
    }
};

export const clearAuthError = () => (dispatch: Dispatch) => {
    dispatch({ type: CLEAR_ERROR });
};

export const register = (userData: RegisterData) => async () => {
    try {
        await AuthStoreAPI.register(userData);
        return { success: true };
    } catch (error: any) {
        return { success: false, message: error.response?.data?.message || 'Ошибка регистрации' };
    }
};

export const logout = () => (dispatch: Dispatch) => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    dispatch({ type: LOGOUT });
};
