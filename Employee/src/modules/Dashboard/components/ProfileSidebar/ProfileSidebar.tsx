import { Avatar, Card, Group, Stack, Text, Title } from '@mantine/core';
import { useEffect, useState } from 'react';

export const ProfileSidebar = () => {
  const [currentTime, setCurrentTime] = useState(new Date());

  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);

    return () => clearInterval(timer);
  }, []);

  return (
    <Card padding="lg" radius="xl" maw={300}>
      <Stack gap="md" align="center" justify="center">
        <Avatar size="xl" />
        <Title order={4}>Иванов Иван Иванович</Title>
        <Text c="dimmed">Аккаунт активен</Text>
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
