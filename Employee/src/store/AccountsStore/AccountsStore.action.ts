import {
  GET_CLIENT_ACCOUNT_OPERATIONS_ACTION_NAME,
  GET_CLIENT_ACCOUNTS_ACTION_NAME,
} from './AccountsStore.const';
import { Account, Operation } from './AccountsStore.types';
import { AccountsAPI } from './api';

import { createRetryableThunk } from '~/store/retryableThunk';

export const getClientAccounts = createRetryableThunk<
  Omit<Account, 'operations'>[],
  { accessToken: string; id: string }
>(
  GET_CLIENT_ACCOUNTS_ACTION_NAME,
  async ({ accessToken, id }, idempotencyKey) =>
    AccountsAPI.getClientAccounts(accessToken, id, idempotencyKey),
);

export const getAccountOperations = createRetryableThunk<
  Operation[],
  { accessToken: string; id: string }
>(
  GET_CLIENT_ACCOUNT_OPERATIONS_ACTION_NAME,
  async ({ accessToken, id }, idempotencyKey) =>
    AccountsAPI.getAccountOperations(accessToken, id, idempotencyKey),
);
