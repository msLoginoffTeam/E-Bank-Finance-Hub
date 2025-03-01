import { GET_USERS } from './ClientsStoreAPI.const';

import { axiosInstance } from '~/api/axiosInstance';
import { Client } from '~/store/ClientsStore';

export const getClients = async (accessToken: string): Promise<Client[]> => {
  const { data } = await axiosInstance.post<Client[]>(
    GET_USERS,
    {},
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    },
  );

  return data;
};
