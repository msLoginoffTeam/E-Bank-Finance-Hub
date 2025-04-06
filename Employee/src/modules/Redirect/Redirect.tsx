import { Loader, Stack } from '@mantine/core';
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import { useAppDispatch } from '~/hooks/redux';
import { setTokens } from '~/store/AuthStore';

export const Redirect = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search);
    const accessToken = urlParams.get('accessToken');
    const refreshToken = urlParams.get('refreshToken');

    if (accessToken && refreshToken) {
      dispatch(setTokens({ accessToken, refreshToken }));
      navigate('/');
      //window.history.replaceState({}, document.title, window.location.pathname);
    }
  }, []);

  return (
    <Stack gap="md" align="center" w="100%">
      <Loader color="blue" />
    </Stack>
  );
};
