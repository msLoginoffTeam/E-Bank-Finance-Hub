import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import {
  getAccountOperations,
  getClientAccounts,
} from './AccountsStore.action';
import { ACCOUNTS_SLICE_NAME } from './AccountsStore.const';
import { AccountState, Operation } from './AccountsStore.types';

const initialState: AccountState = {
  accounts: [],
  isLoading: false,
  error: undefined,
};

export const AccountsSlice = createSlice({
  name: ACCOUNTS_SLICE_NAME,
  initialState,
  reducers: {
    setOperation: (
      state,
      action: PayloadAction<{ accountId: string; operations: Operation[] }>,
    ) => {
      const account = state.accounts.find(
        (acc) => acc.id === action.payload.accountId,
      );

      if (account) {
        account.isLoadingOperations = true;
        account.operations = [
          ...action.payload.operations,
          ...account.operations,
        ];
      }
      state.error = undefined;
    },
    changeBalance: (
      state,
      action: PayloadAction<{ accountId: string; amount: number }>,
    ) => {
      const account = state.accounts.find(
        (acc) => acc.id === action.payload.accountId,
      );

      if (account) {
        account.balance = account.balance + action.payload.amount;
      }
    },
  },
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

export const { setOperation, changeBalance } = AccountsSlice.actions;

export const AccountsReducer = AccountsSlice.reducer;
