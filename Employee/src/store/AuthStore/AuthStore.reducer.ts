import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import {
  blockUser,
  createUser,
  getEmployeeProfile,
  loginEmployee,
  unblockUser,
} from './AuthStore.actions';
import { AUTH_SLICE_NAME } from './AuthStore.const';
import { AuthState, LoginResponse } from './AuthStore.types';
import { loadTokenFromLocalStorage } from './AuthStore.utils';

const initialState: AuthState = {
  accessToken: loadTokenFromLocalStorage('accessToken'),
  refreshToken: loadTokenFromLocalStorage('refreshToken'),
  profile: {
    id: '',
    email: '',
    fullName: '',
    isBlocked: false,
  },
  isLoggedIn: !!loadTokenFromLocalStorage('accessToken'),
  isLoading: false,
  error: undefined,
};

export const AuthSlice = createSlice({
  name: AUTH_SLICE_NAME,
  initialState,
  reducers: {
    setTokens: (state, action: PayloadAction<LoginResponse>) => {
      localStorage.setItem('accessToken', action.payload.accessToken);
      localStorage.setItem('refreshToken', action.payload.refreshToken);
      state.accessToken = action.payload.accessToken;
      state.refreshToken = action.payload.refreshToken;
    },
    logout: (state) => {
      state.accessToken = null;
      state.refreshToken = null;
      localStorage.setItem('accessToken', '');
      localStorage.setItem('refreshToken', '');
    },
  },
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
      })
      .addCase(createUser.rejected, (state, { payload }) => {
        console.log(payload);
        state.error = payload;
      })
      .addCase(getEmployeeProfile.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(getEmployeeProfile.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.profile = payload;
        state.error = undefined;
      })
      .addCase(getEmployeeProfile.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      })
      .addCase(blockUser.rejected, (state, { payload }) => {
        console.log(payload);
        state.error = payload;
      })
      .addCase(unblockUser.rejected, (state, { payload }) => {
        console.log(payload);
        state.error = payload;
      });
  },
});

export const { setTokens, logout } = AuthSlice.actions;

export const AuthReducer = AuthSlice.reducer;
