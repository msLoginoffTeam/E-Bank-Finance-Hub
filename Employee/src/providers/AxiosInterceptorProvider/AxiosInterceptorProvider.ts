import { useEffect, useState } from 'react';

import { axiosInstance } from '~/api/axiosInstance';
import { BASE_URL } from '~/constants/apiURL';
import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { logout, setTokens } from '~/store/AuthStore';

export const AxiosInterceptorProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const [isInitialized, setIsInitialized] = useState(false);
  const dispatch = useAppDispatch();
  const { accessToken, refreshToken } = useAppSelector((state) => state.auth);

  useEffect(() => {
    const responseInterceptor = axiosInstance.interceptors.response.use(
      (response) => {
        return response;
      },
      async (error) => {
        console.log(error.response);

        if (
          error.response?.status === 401 &&
          refreshToken &&
          !error.config.url.includes('/api/users/refresh')
        ) {
          try {
            const { data } = await axiosInstance.post(
              `${BASE_URL}:8082/api/users/refresh`,
              {},
              {
                headers: {
                  Authorization: `Bearer ${refreshToken}`,
                },
              },
            );
            dispatch(
              setTokens({
                accessToken: data.accessToken,
                refreshToken: data.refreshToken,
              }),
            );
            error.config.headers.Authorization = `Bearer ${data.accessToken}`;

            return axiosInstance(error.config);
          } catch (refreshError) {
            dispatch(logout());

            return Promise.reject(refreshError);
          }
        }

        return Promise.reject(error);
      },
    );

    setIsInitialized(true);

    return () => {
      // axiosInstance.interceptors.request.eject(requestInterceptor);
      axiosInstance.interceptors.response.eject(responseInterceptor);
    };
  }, [accessToken, refreshToken, dispatch]);

  if (!isInitialized) return null;

  return children;
};
