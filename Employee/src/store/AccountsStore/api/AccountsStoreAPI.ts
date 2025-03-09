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
): Promise<Omit<Account, 'operations'>[]> => {
  const { data } = await axiosInstance.get<Omit<Account, 'operations'>[]>(
    `${GET_USER_ACCOUNTS}?ClientId=${id}`,
    {
      baseURL: `${BASE_URL}:8080`,
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    },
  );

  return data;
};

export const getAccountOperations = async (
  accessToken: string,
  id: string,
): Promise<Operation[]> => {
  const { data } = await axiosInstance.get<Operation[]>(
    GET_ACCOUNT_OPERATIONS(id),
    {
      baseURL: `${BASE_URL}:8080`,
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    },
  );

  return data;
};
