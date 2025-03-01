import { createAsyncThunk } from '@reduxjs/toolkit';
import { AxiosError } from 'axios';

import { AuthAPI } from './api';
import { LOGIN_EMPLOYEE_ACTION_NAME } from './AuthStore.const';
import { AuthCredentials, LoginResponse } from './AuthStore.types';

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
