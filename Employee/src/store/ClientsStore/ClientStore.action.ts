import { ClientsAPI } from './api';
import {
  GET_CLIENT_ACTION_NAME,
  GET_CLIENTS_ACTION_NAME,
  GET_EMPLOYEES_ACTION_NAME,
} from './ClientsStore.const';
import { Client } from './ClientsStore.types';

import { createRetryableThunk } from '~/store/retryableThunk';

export const getClients = createRetryableThunk<Client[], string>(
  GET_CLIENTS_ACTION_NAME,
  async (accessToken, idempotencyKey) =>
    ClientsAPI.getClients(accessToken, idempotencyKey),
);

export const getClient = createRetryableThunk<
  Client,
  { accessToken: string; id: string }
>(GET_CLIENT_ACTION_NAME, async ({ accessToken, id }, idempotencyKey) =>
  ClientsAPI.getClientProfile(accessToken, id, idempotencyKey),
);

export const getEmployees = createRetryableThunk<Client[], string>(
  GET_EMPLOYEES_ACTION_NAME,
  async (accessToken, idempotencyKey) =>
    ClientsAPI.getEmployees(accessToken, idempotencyKey),
);
