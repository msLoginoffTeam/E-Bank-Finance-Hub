import { createSlice } from '@reduxjs/toolkit';

import { editSettings, getSettings } from './AppStore.action';
import { APP_SLICE_NAME } from './AppStore.const';
import { AppState, Theme } from './AppStore.types';

const initialState: AppState = {
  theme: Theme.LIGHT,
  hiddenAccounts: [],
  isLoading: false,
  error: undefined,
};

export const AppSlice = createSlice({
  name: APP_SLICE_NAME,
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getSettings.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(getSettings.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.theme = payload.theme;
        state.hiddenAccounts = payload.hiddenAccounts;
        state.error = undefined;
      })
      .addCase(getSettings.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      })
      .addCase(editSettings.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(editSettings.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.theme = payload.theme;
        state.hiddenAccounts = payload.hiddenAccounts;
        state.error = undefined;
      })
      .addCase(editSettings.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      });
  },
});

//export const { } = ClientsSlice.actions;

export const AppReducer = AppSlice.reducer;
