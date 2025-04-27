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

export const register = async (
  accessToken: string,
  data: CreateUser,
  idempotencyKey?: string,
): Promise<void> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  await axiosInstance.post<void>(
    REGISTER(data.userRole),
    {
      email: data.email,
      password: data.password,
      fullName: data.fullName,
    },
    {
      baseURL: `${BASE_URL}:8082`,
      headers,
    },
  );
};

export const getEmployeeProfile = async (
  accessToken: string,
  id: string,
  idempotencyKey?: string,
): Promise<EmployeeProfile> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  const { data } = await axiosInstance.get<EmployeeProfile>(
    `${GET_EMPLOYEE_PROFILE}?ClientId=${id}`,
    {
      baseURL: `${BASE_URL}:8082`,
      headers,
    },
  );

  return data;
};

export const blockUser = async (
  accessToken: string,
  id: string,
  idempotencyKey?: string,
): Promise<void> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  await axiosInstance.post<void>(
    BLOCK_USER(id),
    {},
    {
      baseURL: `${BASE_URL}:8082`,
      headers,
    },
  );
};

export const unblockUser = async (
  accessToken: string,
  id: string,
  idempotencyKey?: string,
): Promise<void> => {
  const headers: Record<string, string> = {
    Authorization: `Bearer ${accessToken}`,
  };

  if (idempotencyKey) {
    headers['Idempotency-Key'] = idempotencyKey;
  }

  await axiosInstance.post<void>(
    UNBLOCK_USER(id),
    {},
    {
      baseURL: `${BASE_URL}:8082`,
      headers,
    },
  );
};
