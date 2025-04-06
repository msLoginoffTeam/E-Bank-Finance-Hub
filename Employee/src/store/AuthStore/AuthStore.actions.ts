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

export const createUser = createAsyncThunk<
  void,
  { accessToken: string; userData: CreateUser },
  { rejectValue: string }
>(
  CREATE_USER_ACTION_NAME,
  async ({ accessToken, userData }, { rejectWithValue }) => {
    try {
      await AuthAPI.register(accessToken, userData);
    } catch (e) {
      console.log(e);

      if (e instanceof AxiosError) {
        return rejectWithValue(
          e.response?.data?.message || e.response?.data?.errors.FullName,
        );
      }

      return rejectWithValue('Произошла ошибка');
    }
  },
);

export const getEmployeeProfile = createAsyncThunk<
  EmployeeProfile,
  { accessToken: string; id: string },
  { rejectValue: string }
>(
  GET_EMPLOYEE_PROFILE_ACTION_NAME,
  async ({ accessToken, id }, { rejectWithValue }) => {
    try {
      return await AuthAPI.getEmployeeProfile(accessToken, id);
    } catch (e) {
      console.log(e);

      if (e instanceof AxiosError) {
        return rejectWithValue(e.response?.data?.message);
      }

      return rejectWithValue('Произошла ошибка');
    }
  },
);

export const blockUser = createAsyncThunk<
  void,
  { accessToken: string; id: string },
  { rejectValue: string }
>(BLOCK_USER_ACTION_NAME, async ({ accessToken, id }, { rejectWithValue }) => {
  try {
    await AuthAPI.blockUser(accessToken, id);
  } catch (e) {
    console.log(e);

    if (e instanceof AxiosError) {
      return rejectWithValue(e.response?.data?.message || 'Произошла ошибка');
    }

    return rejectWithValue('Произошла ошибка');
  }
});

export const unblockUser = createAsyncThunk<
  void,
  { accessToken: string; id: string },
  { rejectValue: string }
>(
  UNBLOCK_USER_ACTION_NAME,
  async ({ accessToken, id }, { rejectWithValue }) => {
    try {
      await AuthAPI.unblockUser(accessToken, id);
    } catch (e) {
      console.log(e);

      if (e instanceof AxiosError) {
        return rejectWithValue(e.response?.data?.message || 'Произошла ошибка');
      }

      return rejectWithValue('Произошла ошибка');
    }
  },
);
