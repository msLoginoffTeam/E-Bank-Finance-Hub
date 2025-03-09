import {
  BLOCK_USER,
  GET_EMPLOYEE_PROFILE,
  LOGIN,
  REGISTER,
  UNBLOCK_USER,
} from './AuthStoreAPI.const';

import { axiosInstance } from '~/api/axiosInstance';
import { BASE_URL } from '~/constants/apiURL';
import {
  AuthCredentials,
  CreateUser,
  EmployeeProfile,
  LoginResponse,
} from '~/store/AuthStore/AuthStore.types';

export const login = async (
  credenitals: AuthCredentials,
): Promise<LoginResponse> => {
  const { data } = await axiosInstance.post<LoginResponse>(LOGIN, credenitals, {
    baseURL: `${BASE_URL}:8082`,
  });

  return data;
};

export const register = async (data: CreateUser): Promise<void> => {
  await axiosInstance.post<void>(
    REGISTER(data.userRole),
    {
      email: data.email,
      password: data.password,
      fullName: data.fullName,
    },
    {
      baseURL: `${BASE_URL}:8082`,
    },
  );
};

export const getEmployeeProfile = async (
  accessToken: string,
  id: string,
): Promise<EmployeeProfile> => {
  const { data } = await axiosInstance.get<EmployeeProfile>(
    `${GET_EMPLOYEE_PROFILE}?ClientId=${id}`,
    {
      baseURL: `${BASE_URL}:8082`,
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    },
  );

  return data;
};

export const blockUser = async (
  accessToken: string,
  id: string,
): Promise<void> => {
  await axiosInstance.post<void>(
    BLOCK_USER(id),
    {},
    {
      baseURL: `${BASE_URL}:8082`,
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    },
  );
};

export const unblockUser = async (
  accessToken: string,
  id: string,
): Promise<void> => {
  await axiosInstance.post<void>(
    UNBLOCK_USER(id),
    {},
    {
      baseURL: `${BASE_URL}:8082`,
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    },
  );
};
