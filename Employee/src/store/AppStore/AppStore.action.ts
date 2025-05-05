import { createAsyncThunk } from '@reduxjs/toolkit';
import { AxiosError } from 'axios';

import { AppAPI } from './api';
import {
  EDIT_SETTINGS_ACTION_NAME,
  GET_SETTINGS_ACTION_NAME,
} from './AppStore.const';
import { SettingsEdit, SettingsResponse } from './AppStore.types';

export const getSettings = createAsyncThunk<
  SettingsResponse,
  { id: string },
  { rejectValue: string }
>(GET_SETTINGS_ACTION_NAME, async ({ id }, { rejectWithValue }) => {
  try {
    return await AppAPI.getSettings(id);
  } catch (e) {
    console.log(e);

    if (e instanceof AxiosError) {
      return rejectWithValue(e.response?.data?.message);
    }

    return rejectWithValue('Произошла ошибка');
  }
});

export const editSettings = createAsyncThunk<
  SettingsResponse,
  { id: string; newSettings: Partial<SettingsEdit> },
  { rejectValue: string }
>(
  EDIT_SETTINGS_ACTION_NAME,
  async ({ id, newSettings }, { rejectWithValue }) => {
    try {
      return await AppAPI.editSettings(id, newSettings);
    } catch (e) {
      console.log(e);

      if (e instanceof AxiosError) {
        return rejectWithValue(e.response?.data?.message);
      }

      return rejectWithValue('Произошла ошибка');
    }
  },
);
