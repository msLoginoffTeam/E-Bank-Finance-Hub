import { createAsyncThunk } from '@reduxjs/toolkit';
import { AxiosError } from 'axios';

import { CreditsAPI } from './api';
import {
  CLOSE_CREDIT_PLAN_ACTION_NAME,
  CREATE_CREDIT_PLAN_ACTION_NAME,
  GET_CLIENT_CREDIT_HISTORY_ACTION_NAME,
  GET_CLIENT_CREDITS_ACTION_NAME,
  GET_CREDITS_PLANS_ACTION_NAME,
} from './CreditsStore.const';
import {
  ClientCreditForEmployeeResponse,
  CreditPlan,
  GetCredidtsPlansResponse,
} from './CreditsStore.types';

export const getCreditsPlans = createAsyncThunk<
  GetCredidtsPlansResponse,
  string,
  { rejectValue: string }
>(GET_CREDITS_PLANS_ACTION_NAME, async (accessToken, { rejectWithValue }) => {
  try {
    return await CreditsAPI.getCreditsPlan(accessToken);
  } catch (e) {
    console.log(e);

    if (e instanceof AxiosError) {
      return rejectWithValue(e.response?.data?.message);
    }

    return rejectWithValue('Произошла ошибка');
  }
});

export const createCreditPlan = createAsyncThunk<
  CreditPlan,
  { accessToken: string; planName: string; planPercent: number },
  { rejectValue: string }
>(
  CREATE_CREDIT_PLAN_ACTION_NAME,
  async ({ accessToken, planName, planPercent }, { rejectWithValue }) => {
    try {
      return await CreditsAPI.createCreditsPlan({
        accessToken,
        planName,
        planPercent,
      });
    } catch (e) {
      console.log(e);

      if (e instanceof AxiosError) {
        return rejectWithValue(e.response?.data?.message);
      }

      return rejectWithValue('Произошла ошибка');
    }
  },
);

export const getClientCredits = createAsyncThunk<
  ClientCreditForEmployeeResponse[],
  { accessToken: string; id: string },
  { rejectValue: string }
>(
  GET_CLIENT_CREDITS_ACTION_NAME,
  async ({ accessToken, id }, { rejectWithValue }) => {
    try {
      return await CreditsAPI.getClientCredits(accessToken, id);
    } catch (e) {
      console.log(e);

      if (e instanceof AxiosError) {
        return rejectWithValue(e.response?.data?.message);
      }

      return rejectWithValue('Произошла ошибка');
    }
  },
);

export const closeCreditPlan = createAsyncThunk<
  void,
  { accessToken: string; id: string },
  { rejectValue: string }
>(
  CLOSE_CREDIT_PLAN_ACTION_NAME,
  async ({ accessToken, id }, { rejectWithValue }) => {
    try {
      await CreditsAPI.closeCreditsPlan(accessToken, id);
    } catch (e) {
      console.log(e);

      if (e instanceof AxiosError) {
        return rejectWithValue(e.response?.data?.message);
      }

      return rejectWithValue('Произошла ошибка');
    }
  },
);

export const getClientCreditHistory = createAsyncThunk<
  Omit<ClientCreditForEmployeeResponse, 'clientId' | 'accountId'>,
  { accessToken: string; id: string },
  { rejectValue: string }
>(
  GET_CLIENT_CREDIT_HISTORY_ACTION_NAME,
  async ({ accessToken, id }, { rejectWithValue }) => {
    try {
      return await CreditsAPI.getCreditHistory(accessToken, id);
    } catch (e) {
      console.log(e);

      if (e instanceof AxiosError) {
        return rejectWithValue(e.response?.data?.message);
      }

      return rejectWithValue('Произошла ошибка');
    }
  },
);
