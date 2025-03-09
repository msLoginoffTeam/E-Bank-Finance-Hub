import {
  GET_EMPLOYEES,
  GET_USER_PROFILE,
  GET_USERS,
} from './ClientsStoreAPI.const';

import { axiosInstance } from '~/api/axiosInstance';
import { BASE_URL } from '~/constants/apiURL';
import { Client } from '~/store/ClientsStore';

export const getClients = async (accessToken: string): Promise<Client[]> => {
  const { data } = await axiosInstance.get<Client[]>(GET_USERS, {
    baseURL: `${BASE_URL}:8082`,
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });

  return data;
};

export const getClientProfile = async (
  accessToken: string,
  id: string,
): Promise<Client> => {
  const { data } = await axiosInstance.get<Client>(
    `${GET_USER_PROFILE}?ClientId=${id}`,
    {
      baseURL: `${BASE_URL}:8082`,
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    },
  );

  return data;
};

export const getEmployees = async (accessToken: string): Promise<Client[]> => {
  const { data } = await axiosInstance.get<Client[]>(GET_EMPLOYEES, {
    baseURL: `${BASE_URL}:8082`,
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });

  return data;
};
