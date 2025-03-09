import { createSlice } from '@reduxjs/toolkit';

import {
  getAccountOperations,
  getClientAccounts,
} from './AccountsStore.action';
import { ACCOUNTS_SLICE_NAME } from './AccountsStore.const';
import { AccountState } from './AccountsStore.types';

const initialState: AccountState = {
  accounts: [],
  isLoading: false,
  error: undefined,
};

export const AccountsSlice = createSlice({
  name: ACCOUNTS_SLICE_NAME,
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getClientAccounts.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(getClientAccounts.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.accounts = payload.map((account) => ({
          ...account,
          operations: [],
        }));
        state.error = undefined;
      })
      .addCase(getClientAccounts.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      })
      .addCase(getAccountOperations.pending, (state, { meta }) => {
        const accountId = meta.arg.id;
        const account = state.accounts.find((acc) => acc.id === accountId);

        if (account) {
          account.isLoadingOperations = false;
        }
        state.error = undefined;
      })
      .addCase(getAccountOperations.fulfilled, (state, { payload, meta }) => {
        const accountId = meta.arg.id;
        state.isLoading = false;
        const account = state.accounts.find((acc) => acc.id === accountId);

        if (account) {
          account.isLoadingOperations = true;
          account.operations = payload;
        }
        state.error = undefined;
      })
      .addCase(getAccountOperations.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error = payload;
      });
  },
});

//export const { } = AccountsSlice.actions;

export const AccountsReducer = AccountsSlice.reducer;
