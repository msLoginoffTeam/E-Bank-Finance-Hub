import { AppShell, Flex, useMantineColorScheme } from '@mantine/core';
import { useEffect } from 'react';

import { RouterComponent } from '~/components/RouterComponent';
import { useAppSelector } from '~/hooks/redux';
import { Theme } from '~/store/AppStore/AppStore.types';

export const MainWrapper = () => {
  const { theme } = useAppSelector((state) => state.app);
  const { colorScheme, setColorScheme } = useMantineColorScheme();

  useEffect(() => {
    if (theme === Theme.DARK) {
      setColorScheme('dark');
    } else {
      setColorScheme('light');
    }
  }, [theme]);

  return (
    <AppShell.Main
      style={{ background: colorScheme === 'light' ? '#f6f6f6' : '#1f1f1f' }}
    >
      <Flex justify="center" align="center" direction="column">
        <RouterComponent />
      </Flex>
    </AppShell.Main>
  );
};
