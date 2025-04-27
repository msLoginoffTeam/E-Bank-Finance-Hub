import {
  GET_ACCOUNT_OPERATIONS,
  GET_USER_ACCOUNTS,
} from './AccountsStoreAPI.const';

import { axiosInstance } from '~/api/axiosInstance';
import { BASE_URL } from '~/constants/apiURL';
import { Operation, Account } from '~/store/AccountsStore';

export const getClientAccounts = async (
  accessToken: string,
  id: string,
  idempotencyKey?: string,
): Promise<Omit<Account, 'operations'>[]> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.get<Omit<Account, 'operations'>[]>(
    `${GET_USER_ACCOUNTS}?ClientId=${id}`,
    {
      baseURL: `${BASE_URL}:8080`,
      headers,
    },
  );

  return data;
};

export const getAccountOperations = async (
  accessToken: string,
  id: string,
  idempotencyKey?: string,
): Promise<Operation[]> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.get<Operation[]>(
    GET_ACCOUNT_OPERATIONS(id),
    {
      baseURL: `${BASE_URL}:8080`,
      headers,
    },
  );

  return data;
};
