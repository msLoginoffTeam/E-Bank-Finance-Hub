import '@mantine/core/styles.css';
import '@mantine/notifications/styles.css';
import './App.css';

import { MantineProvider, AppShell } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { Notifications } from '@mantine/notifications';
import { useEffect } from 'react';
import { BrowserRouter as Router } from 'react-router-dom';

import { retryRequest } from './api/retryRequest';
import { setDeviceToken } from './api/setDeviceToken';
import { Header } from './components/Header';
import { MainWrapper } from './components/MainWrapper';
import { Navbar } from './components/Navbar';
import { requestNotificationPermission } from './firebaseNotifications';
import { useAppDispatch, useAppSelector } from './hooks/redux';
import { useNotification } from './hooks/useNotification';
import { AxiosInterceptorProvider } from './providers/AxiosInterceptorProvider';
import { setupOnMessageListener } from './setupOnMessageListener';
import { getSettings } from './store/AppStore';
import { getEmployeeProfile } from './store/AuthStore';
import { decodeJwt } from './utils';

const App = () => {
  const [opened, { toggle }] = useDisclosure();
  const dispatch = useAppDispatch();
  const { accessToken } = useAppSelector((state) => state.auth);
  const { showMessage } = useNotification();

  useEffect(() => {
    if (accessToken) {
      const id = decodeJwt(accessToken)['Id'];
      dispatch(getEmployeeProfile({ accessToken, id }));
      dispatch(getSettings({ id }));
    }
  }, [accessToken]);

  useEffect(() => {
    const setupNotifications = async () => {
      try {
        const token = await requestNotificationPermission();
        console.log('FCM Token:', token);

        if (token && accessToken) {
          await retryRequest((idempotencyKey) =>
            setDeviceToken(token, accessToken, idempotencyKey),
          );
        }
      } catch (error) {
        console.error('Ошибка при настройке уведомлений:', error);
      }
    };

    setupNotifications();
    setupOnMessageListener(showMessage);
  }, [accessToken]);

  return (
    <AxiosInterceptorProvider>
      <Router
        future={{
          v7_startTransition: true,
          v7_relativeSplatPath: true,
        }}
      >
        <MantineProvider theme={{ fontFamily: 'Montserrat, sans-serif' }}>
          <Notifications />
          <AppShell
            header={{ height: 60 }}
            navbar={{
              width: 300,
              breakpoint: 'sm',
              collapsed: { desktop: true, mobile: !opened },
            }}
            padding="md"
            withBorder={false}
          >
            <AppShell.Header>
              <Header opened={opened} onToggle={toggle} />
            </AppShell.Header>
            <AppShell.Navbar py="md" px={2}>
              <Navbar />
            </AppShell.Navbar>
            <MainWrapper />
          </AppShell>
        </MantineProvider>
      </Router>
    </AxiosInterceptorProvider>
  );
};

export default App;
