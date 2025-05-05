import { createAsyncThunk } from '@reduxjs/toolkit';
import { AxiosError } from 'axios';

import { AuthAPI } from './api';
import {
  BLOCK_USER_ACTION_NAME,
  CREATE_USER_ACTION_NAME,
  GET_EMPLOYEE_PROFILE_ACTION_NAME,
  LOGIN_EMPLOYEE_ACTION_NAME,
  UNBLOCK_USER_ACTION_NAME,
} from './AuthStore.const';
import {
  AuthCredentials,
  CreateUser,
  EmployeeProfile,
  LoginResponse,
} from './AuthStore.types';

import { createRetryableThunk } from '~/store/retryableThunk';

export const loginEmployee = createAsyncThunk<
  LoginResponse,
  AuthCredentials,
  { rejectValue: string }
>(LOGIN_EMPLOYEE_ACTION_NAME, async (id, { rejectWithValue }) => {
  try {
    return await AuthAPI.login(id);
  } catch (e) {
    console.log(e);

    if (e instanceof AxiosError) {
      return rejectWithValue(e.response?.data?.message);
    }

    return rejectWithValue('Произошла ошибка');
  }
});

export const createUser = createRetryableThunk<
  void,
  { accessToken: string; userData: CreateUser }
>(CREATE_USER_ACTION_NAME, async ({ accessToken, userData }, idempotencyKey) =>
  AuthAPI.register(accessToken, userData, idempotencyKey),
);

export const getEmployeeProfile = createRetryableThunk<
  EmployeeProfile,
  { accessToken: string; id: string }
>(
  GET_EMPLOYEE_PROFILE_ACTION_NAME,
  async ({ accessToken, id }, idempotencyKey) =>
    AuthAPI.getEmployeeProfile(accessToken, id, idempotencyKey),
);

export const blockUser = createRetryableThunk<
  void,
  { accessToken: string; id: string }
>(BLOCK_USER_ACTION_NAME, async ({ accessToken, id }, idempotencyKey) =>
  AuthAPI.blockUser(accessToken, id, idempotencyKey),
);

export const unblockUser = createRetryableThunk<
  void,
  { accessToken: string; id: string }
>(UNBLOCK_USER_ACTION_NAME, async ({ accessToken, id }, idempotencyKey) =>
  AuthAPI.unblockUser(accessToken, id, idempotencyKey),
);
