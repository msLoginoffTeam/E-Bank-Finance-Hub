import { createSlice } from '@reduxjs/toolkit';

import { ACCOUNTS_SLICE_NAME } from './AccountsStore.const';
import { AccountState } from './AccountsStore.types';

import { testUser } from '~/dummyData';

const initialState: AccountState = {
  accounts: [testUser],
  isLoading: false,
  error: undefined,
};

export const AccountsSlice = createSlice({
  name: ACCOUNTS_SLICE_NAME,
  initialState,
  reducers: {},
  /*extraReducers: (builder) => {
    builder
  },*/
});

//export const { } = AccountsSlice.actions;

export const AccountsReducer = AccountsSlice.reducer;
