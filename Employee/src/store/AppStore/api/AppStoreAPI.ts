import { EDIT_SETTINGS, GET_SETTINGS } from './AppStoreAPI.const';

import { axiosInstance } from '~/api/axiosInstance';
import { BASE_URL } from '~/constants/apiURL';
import {
  SettingsEdit,
  SettingsResponse,
} from '~/store/AppStore/AppStore.types';

export const getSettings = async (id: string): Promise<SettingsResponse> => {
  const { data } = await axiosInstance.get<SettingsResponse>(GET_SETTINGS(id), {
    baseURL: `${BASE_URL}:3000`,
  });

  return data;
};

export const editSettings = async (
  id: string,
  newSettings: Partial<SettingsEdit>,
): Promise<SettingsResponse> => {
  const { data } = await axiosInstance.put<SettingsResponse>(
    EDIT_SETTINGS(id),
    newSettings,
    {
      baseURL: `${BASE_URL}:3000`,
    },
  );

  return data;
};
