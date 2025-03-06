import {LOGIN_SUCCESS, LOGIN_ERROR, LOGOUT, CLEAR_ERROR} from './AuthStore.const';
import { AuthState } from './AuthStore.types';

const initialState: AuthState = {
    token: localStorage.getItem('token'),
    error: null,
    loading: false,
};

export const AuthReducer = (state = initialState, action: any): AuthState => {
    switch (action.type) {
        case LOGIN_SUCCESS:
            return { ...state, token: action.payload, error: null };
        case LOGIN_ERROR:
            return { ...state, error: action.payload };
        case CLEAR_ERROR:
            return { ...state, error: null };
        case LOGOUT:
            return { ...state, token: null };
        default:
            return state;
    }
};