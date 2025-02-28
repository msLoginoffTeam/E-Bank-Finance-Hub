import { Group, Title } from '@mantine/core';

import { ButtonsPanel } from './components/ButtonsPanel';
import { ProfileSidebar } from './components/ProfileSidebar';

export const Dashboard = () => {
  return (
    <>
      <Title>Админ панель</Title>
      <Group w="100%" mt="xl" gap="lg" justify="center">
        <ProfileSidebar />
        <ButtonsPanel />
      </Group>
    </>
  );
};
