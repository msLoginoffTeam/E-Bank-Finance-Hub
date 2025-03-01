import { createSlice } from '@reduxjs/toolkit';

import { loginEmployee } from './AuthStore.actions';
import { AUTH_SLICE_NAME } from './AuthStore.const';
import { AuthState } from './AuthStore.types';
import { loadTokenFromLocalStorage } from './AuthStore.utils';

const initialState: AuthState = {
  accessToken: loadTokenFromLocalStorage('accessToken'),
  refreshToken: loadTokenFromLocalStorage('refreshToken'),
  isLoggedIn: !!loadTokenFromLocalStorage('accessToken'),
  isLoading: false,
  error: undefined,
};

export const AuthSlice = createSlice({
  name: AUTH_SLICE_NAME,
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(loginEmployee.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(loginEmployee.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.accessToken = payload.accessToken;
        state.refreshToken = payload.refreshToken;
        localStorage.setItem('accessToken', payload.accessToken);
        localStorage.setItem('refreshToken', payload.refreshToken);
        state.isLoggedIn = true;
        state.error = undefined;
      })
      .addCase(loginEmployee.rejected, (state, { payload }) => {
        state.accessToken = null;
        state.refreshToken = null;
        localStorage.setItem('accessToken', '');
        localStorage.setItem('refreshToken', '');
        state.isLoading = false;
        state.isLoggedIn = false;
        console.log(payload);
        state.error = payload;
      });
  },
});

//export const { } = AuthSlice.actions;

export const AuthReducer = AuthSlice.reducer;
