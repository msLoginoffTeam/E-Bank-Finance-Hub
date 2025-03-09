import { Avatar, Badge, Button, Card, Group, Stack, Text } from '@mantine/core';

import { useAppDispatch, useAppSelector } from '~/hooks/redux';
import { useNotification } from '~/hooks/useNotification';
import { blockUser, unblockUser } from '~/store/AuthStore';
import { Client, getClients, getEmployees } from '~/store/ClientsStore';

export const UserItem = ({
  id,
  fullName,
  email,
  isBlocked,
  role,
}: Client & { role?: string }) => {
  const dispatch = useAppDispatch();
  const { accessToken } = useAppSelector((state) => state.auth);
  const { showSuccess } = useNotification();

  const handleBlock = async () => {
    if (accessToken) {
      const result = await dispatch(blockUser({ accessToken, id }));

      if (result.meta.requestStatus === 'fulfilled') {
        showSuccess('Пользователь заблокирован');
        dispatch(getClients(accessToken));

        if (role && role === 'Manager') {
          dispatch(getEmployees(accessToken));
        }
      }
    }
  };

  const handleUnblock = async () => {
    if (accessToken) {
      const result = await dispatch(unblockUser({ accessToken, id }));

      if (result.meta.requestStatus === 'fulfilled') {
        showSuccess('Пользователь разблокирован');
        dispatch(getClients(accessToken));

        if (role && role === 'Manager') {
          dispatch(getEmployees(accessToken));
        }
      }
    }
  };

  return (
    <Card padding="md" radius="xl" shadow="md" withBorder w="100%">
      <Group gap="md">
        <Avatar size="lg" name={fullName} color="initials" />
        <Stack gap="xs">
          <Group>
            <Text c="dimmed">ФИО:</Text>
            <Text style={{ wordBreak: 'break-all' }}>{fullName}</Text>
          </Group>
          <Group>
            <Text c="dimmed">Email:</Text>
            <Text>{email}</Text>
          </Group>
          {isBlocked && <Badge color="red">Заблокирован</Badge>}
        </Stack>
        {!isBlocked && (
          <Button
            variant="outline"
            size="md"
            radius="xl"
            color="red"
            onClick={handleBlock}
          >
            Заблокировать
          </Button>
        )}
        {isBlocked && (
          <Button
            variant="outline"
            size="md"
            radius="xl"
            color="green"
            onClick={handleUnblock}
          >
            Разблокировать
          </Button>
        )}
      </Group>
    </Card>
  );
};
