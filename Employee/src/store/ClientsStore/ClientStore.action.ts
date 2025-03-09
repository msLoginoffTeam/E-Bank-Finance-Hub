import { createAsyncThunk } from '@reduxjs/toolkit';
import { AxiosError } from 'axios';

import { ClientsAPI } from './api';
import {
  GET_CLIENT_ACTION_NAME,
  GET_CLIENTS_ACTION_NAME,
  GET_EMPLOYEES_ACTION_NAME,
} from './ClientsStore.const';
import { Client } from './ClientsStore.types';

export const getClients = createAsyncThunk<
  Client[],
  string,
  { rejectValue: string }
>(GET_CLIENTS_ACTION_NAME, async (accessToken, { rejectWithValue }) => {
  try {
    return await ClientsAPI.getClients(accessToken);
  } catch (e) {
    console.log(e);

    if (e instanceof AxiosError) {
      return rejectWithValue(e.response?.data?.message);
    }

    return rejectWithValue('Произошла ошибка');
  }
});

export const getClient = createAsyncThunk<
  Client,
  { accessToken: string; id: string },
  { rejectValue: string }
>(GET_CLIENT_ACTION_NAME, async ({ accessToken, id }, { rejectWithValue }) => {
  try {
    return await ClientsAPI.getClientProfile(accessToken, id);
  } catch (e) {
    console.log(e);

    if (e instanceof AxiosError) {
      return rejectWithValue(e.response?.data?.message);
    }

    return rejectWithValue('Произошла ошибка');
  }
});

export const getEmployees = createAsyncThunk<
  Client[],
  string,
  { rejectValue: string }
>(GET_EMPLOYEES_ACTION_NAME, async (accessToken, { rejectWithValue }) => {
  try {
    return await ClientsAPI.getEmployees(accessToken);
  } catch (e) {
    console.log(e);

    if (e instanceof AxiosError) {
      return rejectWithValue(e.response?.data?.message);
    }

    return rejectWithValue('Произошла ошибка');
  }
});
