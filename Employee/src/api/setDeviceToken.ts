import { axiosInstance } from './axiosInstance';

import { BASE_URL } from '~/constants/apiURL';

export const setDeviceToken = async (
  deviceToken: string,
  accessToken: string,
  idempotencyKey?: string,
) => {
  try {
    const headers: Record<string, string> = {
      Authorization: `Bearer ${accessToken}`,
    };

    if (idempotencyKey) {
      headers['Idempotency-Key'] = idempotencyKey;
    }

    await axiosInstance.post<void>(
      `/api/users/set/deviceToken?DeviceToken=${deviceToken}`,
      {},
      {
        baseURL: `${BASE_URL}:8082`,
        headers,
      },
    );
  } catch (error) {
    console.error('Ошибка при выполнении запроса:', error);
    throw error;
  }
};
