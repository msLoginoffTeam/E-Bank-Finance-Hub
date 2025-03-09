import { createAsyncThunk } from '@reduxjs/toolkit';
import { AxiosError } from 'axios';

import {
  GET_CLIENT_ACCOUNT_OPERATIONS_ACTION_NAME,
  GET_CLIENT_ACCOUNTS_ACTION_NAME,
} from './AccountsStore.const';
import { Account, Operation } from './AccountsStore.types';
import { AccountsAPI } from './api';

export const getClientAccounts = createAsyncThunk<
  Omit<Account, 'operations'>[],
  { accessToken: string; id: string },
  { rejectValue: string }
>(
  GET_CLIENT_ACCOUNTS_ACTION_NAME,
  async ({ accessToken, id }, { rejectWithValue }) => {
    try {
      return await AccountsAPI.getClientAccounts(accessToken, id);
    } catch (e) {
      console.log(e);

      if (e instanceof AxiosError) {
        return rejectWithValue(e.response?.data?.message);
      }

      return rejectWithValue('Произошла ошибка');
    }
  },
);

export const getAccountOperations = createAsyncThunk<
  Operation[],
  { accessToken: string; id: string },
  { rejectValue: string }
>(
  GET_CLIENT_ACCOUNT_OPERATIONS_ACTION_NAME,
  async ({ accessToken, id }, { rejectWithValue }) => {
    try {
      return await AccountsAPI.getAccountOperations(accessToken, id);
    } catch (e) {
      console.log(e);

      if (e instanceof AxiosError) {
        return rejectWithValue(e.response?.data?.message);
      }

      return rejectWithValue('Произошла ошибка');
    }
  },
);
