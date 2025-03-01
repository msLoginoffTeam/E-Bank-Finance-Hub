import { LOGIN } from './AuthStoreAPI.const';

import { axiosInstance } from '~/api/axiosInstance';
import {
  AuthCredentials,
  LoginResponse,
} from '~/store/AuthStore/AuthStore.types';

export const login = async (
  credenitals: AuthCredentials,
): Promise<LoginResponse> => {
  const { data } = await axiosInstance.post<LoginResponse>(LOGIN, credenitals);

  return data;
};
