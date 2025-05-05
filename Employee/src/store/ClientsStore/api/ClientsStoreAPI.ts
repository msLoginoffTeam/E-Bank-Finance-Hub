import {
  GET_EMPLOYEES,
  GET_USER_PROFILE,
  GET_USERS,
} from './ClientsStoreAPI.const';

import { axiosInstance } from '~/api/axiosInstance';
import { BASE_URL } from '~/constants/apiURL';
import { Client } from '~/store/ClientsStore';

export const getClients = async (
  accessToken: string,
  idempotencyKey?: string,
): Promise<Client[]> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.get<Client[]>(GET_USERS, {
    baseURL: `${BASE_URL}:8082`,
    headers,
  });

  return data;
};

export const getClientProfile = async (
  accessToken: string,
  id: string,
  idempotencyKey?: string,
): Promise<Client> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.get<Client>(
    `${GET_USER_PROFILE}?ClientId=${id}`,
    {
      baseURL: `${BASE_URL}:8082`,
      headers,
    },
  );

  return data;
};

export const getEmployees = async (
  accessToken: string,
  idempotencyKey?: string,
): Promise<Client[]> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.get<Client[]>(GET_EMPLOYEES, {
    baseURL: `${BASE_URL}:8082`,
    headers,
  });

  return data;
};
