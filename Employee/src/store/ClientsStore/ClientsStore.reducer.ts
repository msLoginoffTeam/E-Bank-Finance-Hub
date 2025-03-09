import { createSlice } from '@reduxjs/toolkit';

import { CLIENTS_SLICE_NAME } from './ClientsStore.const';
import { ClientsState } from './ClientsStore.types';
import { getClient, getClients, getEmployees } from './ClientStore.action';

const initialState: ClientsState = {
  clients: [],
  employees: [],
  client: {
    id: '',
    email: '',
    fullName: '',
    isBlocked: false,
  },
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
      })
      .addCase(getClient.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(getClient.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.client = payload;
        state.error = undefined;
      })
      .addCase(getClient.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      })
      .addCase(getEmployees.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(getEmployees.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.employees = payload;
        state.error = undefined;
      })
      .addCase(getEmployees.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      });
  },
});

//export const { } = ClientsSlice.actions;

export const ClientsReducer = ClientsSlice.reducer;
