import { Avatar, Card, Group, Loader, Stack, Text, Title } from '@mantine/core';
import { useEffect, useState } from 'react';

import { useAppSelector } from '~/hooks/redux';

export const ProfileSidebar = () => {
  const [currentTime, setCurrentTime] = useState(new Date());
  const { profile, isLoading } = useAppSelector((state) => state.auth);

  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);

    return () => clearInterval(timer);
  }, []);

  if (isLoading) {
    return <Loader color="blue" />;
  }

  return (
    <Card padding="lg" radius="xl" w="100%" maw={300}>
      <Stack gap="md" align="center" justify="center">
        <Avatar size="xl" />
        <Title order={4}>{profile.fullName}</Title>
        <Text c="dimmed">
          {profile.isBlocked ? 'Аккаунт заблокирован' : 'Аккаунт активен'}
        </Text>
        <Group>
          <Text c="dimmed">Текущее время:</Text>
          <Text>
            {currentTime.toLocaleTimeString([], {
              hour: '2-digit',
              minute: '2-digit',
              second: '2-digit',
            })}
          </Text>
        </Group>
        <Group>
          <Text c="dimmed">Дата:</Text>
          <Text>{currentTime.toLocaleDateString('ru-RU')}</Text>
        </Group>
      </Stack>
    </Card>
  );
};
