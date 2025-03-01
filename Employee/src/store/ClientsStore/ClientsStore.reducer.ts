import { createSlice } from '@reduxjs/toolkit';

import { CLIENTS_SLICE_NAME } from './ClientsStore.const';
import { ClientsState } from './ClientsStore.types';
import { getClients } from './ClientStore.action';

// import { testClient } from '~/dummyData';

const initialState: ClientsState = {
  clients: [],
  isLoading: false,
  error: undefined,
};

export const ClientsSlice = createSlice({
  name: CLIENTS_SLICE_NAME,
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getClients.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(getClients.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.clients = payload;
        state.error = undefined;
      })
      .addCase(getClients.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      });
  },
});

//export const { } = ClientsSlice.actions;

export const ClientsReducer = ClientsSlice.reducer;
